using UnityEngine;
using UnityEngine.Audio;

// Applies saved volumes at launch. Without it, saved values only apply once the Settings screen is opened,
// so persistence after a restart would not work.
public class VolumeLoader : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] string[] parameters = { "MasterVol", "MusicVol", "SFXVol", "UIVol" };

    void Start() { foreach (var p in parameters) VolumeSlider.Apply(mixer, p); }   // Start, not Awake: SetFloat can fail in Awake
}