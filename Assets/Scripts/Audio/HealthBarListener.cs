using UnityEngine;
using UnityEngine.UI;

public class HealthBarListener : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Image fill;                  // Image Type: Filled

    void OnEnable() { health.Damaged += Refresh; health.Healed += Refresh; }
    void OnDisable() { health.Damaged -= Refresh; health.Healed -= Refresh; }
    void Start() => Refresh(health.Current, health.Max);

    void Refresh(int current, int max) => fill.fillAmount = (float)current / max;
}