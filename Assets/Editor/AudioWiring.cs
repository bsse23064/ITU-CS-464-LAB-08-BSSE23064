using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public static class AudioWiring
{
    const string UI = "Assets/Audio/UI", SFX = "Assets/Audio/SFX", MUSIC = "Assets/Audio/Music";

    [MenuItem("Tools/Audio Wiring/Setup Audio System")]
    static void Setup()
    {
        if (Application.isPlaying) { EditorUtility.DisplayDialog("Audio Wiring", "Stop Play mode first.", "OK"); return; }

        var mixer = AssetDatabase.FindAssets("t:AudioMixer")
            .Select(g => AssetDatabase.LoadAssetAtPath<AudioMixer>(AssetDatabase.GUIDToAssetPath(g))).FirstOrDefault();
        if (mixer == null) { Debug.LogError("Audio Wiring: no AudioMixer asset found."); return; }
        var gMusic = Group(mixer, "Master/Music"); var gSfx = Group(mixer, "Master/SFX"); var gUi = Group(mixer, "Master/UI");
        if (gMusic == null || gSfx == null || gUi == null)
        { Debug.LogError("Audio Wiring: the mixer needs groups Master/Music, Master/SFX and Master/UI."); return; }

        ImportSettings(UI, false); ImportSettings(SFX, false); ImportSettings(MUSIC, true);

        // ---------- SfxPlayer: four sources on the SFX group ----------
        var sfx = GetOrCreate<SfxPlayer>("SfxPlayer");
        var sfxSources = Sources(sfx.gameObject, 4);
        foreach (var s in sfxSources) Configure(s, gSfx, false, 128);
        SetArray(sfx, "sources", sfxSources);

        // ---------- MusicPlayer: two looping sources on the Music group ----------
        var music = GetOrCreate<MusicPlayer>("MusicPlayer");
        var mSources = Sources(music.gameObject, 2);
        foreach (var s in mSources) Configure(s, gMusic, true, 0);
        SetRef(music, "a", mSources[0]); SetRef(music, "b", mSources[1]);

        // ---------- UiSound: one source on the UI group ----------
        var ui = GetOrCreate<UiSound>("UiSound");
        var uiSrc = Sources(ui.gameObject, 1)[0];
        Configure(uiSrc, gUi, false, 128);
        var uiSo = new SerializedObject(ui);
        uiSo.FindProperty("source").objectReferenceValue = uiSrc;
        Fill(uiSo, "hover", UI, 1, "tick", "select");
        Fill(uiSo, "click", UI, 3, "click");
        Fill(uiSo, "back", UI, 2, "back");
        Fill(uiSo, "open", UI, 1, "open", "maximize");
        Fill(uiSo, "close", UI, 1, "close", "minimize");
        Fill(uiSo, "success", UI, 1, "confirmation", "success");
        Fill(uiSo, "error", UI, 1, "error");
        uiSo.ApplyModifiedProperties();

        // ---------- VolumeLoader ----------
        var loader = GetOrCreate<VolumeLoader>("VolumeLoader");
        SetRef(loader, "mixer", mixer);

        // ---------- Music clips (first two tracks by name; change them in the Inspector later) ----------
        var tracks = Clips(MUSIC);
        AudioClip menuTrack = tracks.Length > 0 ? tracks[0] : null;
        AudioClip gameTrack = tracks.Length > 1 ? tracks[1] : menuTrack;

        // ---------- Buttons ----------
        int n = 0;
        foreach (var b in Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (b.GetComponent<UiButtonSound>() == null) { Undo.AddComponent<UiButtonSound>(b.gameObject); n++; }

        // ---------- Screens ----------
        foreach (var s in Object.FindObjectsByType<ScreenBase>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (s.name == "Pause" && s.GetComponent<PauseAudio>() == null)
            {
                var pa = Undo.AddComponent<PauseAudio>(s.gameObject);
                SetRef(pa, "mixer", mixer);
            }
            AudioClip clip = s.name == "Title" || s.name == "MainMenu" || s.name == "Results" ? menuTrack
                           : s.name == "Gameplay" ? gameTrack : null;
            if (clip != null)
            {
                var sm = s.GetComponent<ScreenMusic>() ?? Undo.AddComponent<ScreenMusic>(s.gameObject);
                if (sm.clip == null) { sm.clip = clip; EditorUtility.SetDirty(sm); }
            }
        }

        EditorSceneManager.MarkSceneDirty(sfx.gameObject.scene);
        Debug.Log("Audio Wiring done. UiButtonSound added to " + n + " buttons. Check UiSound for placeholder clips.");
    }

    // ---------- helpers ----------
    static T GetOrCreate<T>(string name) where T : Component
    {
        var c = Object.FindFirstObjectByType<T>();
        if (c != null) return c;
        var go = new GameObject(name);                       // root object: DontDestroyOnLoad needs a root
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return go.AddComponent<T>();
    }

    static AudioMixerGroup Group(AudioMixer m, string path)
    {
        var g = m.FindMatchingGroups(path);
        return g != null && g.Length > 0 ? g[0] : null;
    }

    static AudioSource[] Sources(GameObject go, int count)
    {
        var list = go.GetComponents<AudioSource>().ToList();
        while (list.Count < count) list.Add(go.AddComponent<AudioSource>());
        return list.Take(count).ToArray();
    }

    static void Configure(AudioSource s, AudioMixerGroup group, bool loop, int priority)
    {
        s.playOnAwake = false; s.loop = loop; s.spatialBlend = 0f;
        s.priority = priority; s.outputAudioMixerGroup = group;
    }

    static void SetRef(Object target, string prop, Object val)
    {
        var so = new SerializedObject(target);
        so.FindProperty(prop).objectReferenceValue = val;
        so.ApplyModifiedProperties();
    }

    static void SetArray(Object target, string prop, Object[] items)
    {
        var so = new SerializedObject(target);
        var p = so.FindProperty(prop);
        p.arraySize = items.Length;
        for (int i = 0; i < items.Length; i++) p.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
        so.ApplyModifiedProperties();
    }

    static AudioClip[] Clips(string folder) =>
        AssetDatabase.FindAssets("t:AudioClip", new[] { folder })
            .Select(g => AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(g)))
            .OrderBy(c => c.name).ToArray();

    // Fills an EMPTY clip array by keyword. Existing choices are never overwritten.
    static void Fill(SerializedObject so, string prop, string folder, int max, params string[] keys)
    {
        var p = so.FindProperty(prop);
        if (p.arraySize > 0) return;
        var all = Clips(folder);
        AudioClip[] pick = null;
        foreach (var k in keys)
        {
            var m = all.Where(c => c.name.ToLower().Contains(k)).Take(max).ToArray();
            if (m.Length > 0) { pick = m; break; }
        }
        if (pick == null && all.Length > 0)
        {
            pick = new[] { all[0] };
            Debug.LogWarning("Audio Wiring: nothing matched [" + string.Join(", ", keys) + "] for '" + prop +
                             "'. Using '" + all[0].name + "' as a placeholder.");
        }
        if (pick == null) return;
        p.arraySize = pick.Length;
        for (int i = 0; i < pick.Length; i++) p.GetArrayElementAtIndex(i).objectReferenceValue = pick[i];
    }

    static void ImportSettings(string folder, bool music)
    {
        foreach (var g in AssetDatabase.FindAssets("t:AudioClip", new[] { folder }))
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            var imp = AssetImporter.GetAtPath(path) as AudioImporter;
            if (imp == null) continue;
            var s = imp.defaultSampleSettings;
            var want = music ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
            bool changed = false;
            if (s.loadType != want) { s.loadType = want; changed = true; }
            if (music && s.compressionFormat != AudioCompressionFormat.Vorbis)
            { s.compressionFormat = AudioCompressionFormat.Vorbis; s.quality = 0.7f; changed = true; }
            if (changed) { imp.defaultSampleSettings = s; imp.SaveAndReimport(); }
        }
    }
}