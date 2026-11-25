using UnityEngine;

public class HurtSound : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] AudioClip clip;

    void OnEnable() => health.Damaged += Play;
    void OnDisable() => health.Damaged -= Play;

    void Play(int current, int max) { if (SfxPlayer.Instance) SfxPlayer.Instance.Play(clip); }
}