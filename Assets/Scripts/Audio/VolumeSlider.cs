using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// A volume slider for one mixer group. The slider runs 0.0001 to 1; the mixer wants decibels.
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] string parameter = "MusicVol";   // the group's exposed Volume: MasterVol, MusicVol, SFXVol, UIVol
    [SerializeField] Slider slider;                   // Min Value 0.0001, Max Value 1
    [SerializeField] Toggle muteToggle;               // ADDED, optional: ON = muted

    public const float MinValue = 0.0001f;

    void Start()
    {
        slider.minValue = MinValue;
        slider.maxValue = 1f;
        slider.value = PlayerPrefs.GetFloat(parameter, 1f);
        slider.onValueChanged.AddListener(Set);

        if (muteToggle != null)                                       // ADDED
        {
            muteToggle.SetIsOnWithoutNotify(IsMuted(parameter));
            muteToggle.onValueChanged.AddListener(SetMuted);
        }
        Apply(mixer, parameter);
    }

    void OnDisable() => PlayerPrefs.Save();

    void Set(float value)
    {
        PlayerPrefs.SetFloat(parameter, value);
        Apply(mixer, parameter);
    }

    void SetMuted(bool muted)
    {
        PlayerPrefs.SetInt("mute_" + parameter, muted ? 1 : 0);
        Apply(mixer, parameter);
    }

    // ADDED: shared with VolumeLoader. Log10(value) * 20: 1 -> 0 dB, 0.5 -> -6 dB, 0.0001 -> -80 dB.
    public static bool IsMuted(string p) => PlayerPrefs.GetInt("mute_" + p, 0) == 1;

    public static void Apply(AudioMixer m, string p)
    {
        float v = Mathf.Max(PlayerPrefs.GetFloat(p, 1f), MinValue);    // never Log10(0)
        m.SetFloat(p, IsMuted(p) ? -80f : Mathf.Log10(v) * 20f);
    }
}