using UnityEngine;

public class HitSparks : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] ParticleSystem sparks;

    void OnEnable() => health.Damaged += Burst;
    void OnDisable() => health.Damaged -= Burst;

    void Burst(int current, int max) => sparks.Play();
}