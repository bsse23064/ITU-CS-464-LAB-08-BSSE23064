using UnityEngine;

public class PickupSound : MonoBehaviour         // listens to the STATIC GameEvents.CoinCollected
{
    [SerializeField] AudioClip clip;

    void OnEnable() => GameEvents.CoinCollected += Play;
    void OnDisable() => GameEvents.CoinCollected -= Play;   // static event: this line is mandatory

    void Play(int value) { if (SfxPlayer.Instance) SfxPlayer.Instance.Play(clip); }
}