using UnityEngine;

public class ScreenMusic : MonoBehaviour         // add to a screen: it crossfades to this clip when shown
{
    public AudioClip clip;
    void OnEnable() { if (clip != null && MusicPlayer.Instance != null) MusicPlayer.Instance.CrossfadeTo(clip); }
}