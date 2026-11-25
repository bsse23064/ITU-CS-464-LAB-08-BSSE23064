using UnityEngine;
using UnityEngine.Audio;

public class PauseAudio : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;        // optional: only used if you create a "Paused" snapshot

    void OnEnable()  { AudioListener.pause = true;  Snap("Paused"); }
    void OnDisable() { AudioListener.pause = false; Snap("Snapshot"); }   // your default snapshot is named "Snapshot"

    void Snap(string name)
    {
        if (mixer == null) return;
        var s = mixer.FindSnapshot(name);
        if (s != null) s.TransitionTo(0.3f);
    }
}