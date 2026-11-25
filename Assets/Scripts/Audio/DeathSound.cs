using UnityEngine;

public class DeathSound : MonoBehaviour          // listens to Health.Died
{
    [SerializeField] Health health;
    [SerializeField] AudioClip clip;

    void OnEnable() => health.Died += Play;
    void OnDisable() => health.Died -= Play;

    void Play() { if (SfxPlayer.Instance) SfxPlayer.Instance.Play(clip); }
}