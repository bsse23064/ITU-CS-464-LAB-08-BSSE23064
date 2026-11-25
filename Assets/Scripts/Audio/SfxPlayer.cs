using UnityEngine;

// Every sound effect goes through here: a few AudioSources, a random pitch and volume each time.
// Pitch belongs to the source, not the sound, so each sound takes the next source in turn.
public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance { get; private set; }

    [SerializeField] AudioSource[] sources;      // four or so, all with Output set to the mixer's SFX group
    [SerializeField] float pitchRange = 2f;      // ± semitones
    [SerializeField] float minVolume = 0.85f;
    int next;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);           // a reload doesn't cut off the sound that caused it
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource source = sources[next];
        next = (next + 1) % sources.Length;
        source.pitch = Mathf.Pow(2f, Random.Range(-pitchRange, pitchRange) / 12f);
        source.PlayOneShot(clip, volume * Random.Range(minVolume, 1f));
    }
}