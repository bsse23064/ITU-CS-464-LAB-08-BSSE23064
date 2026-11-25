using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int max = 5;
    [SerializeField] float iFrames = 1f;
    public bool shielded;
    public event Action<int, int> Damaged;
    public event Action<int, int> Healed;
    public event Action Died;                 // ADDED: code-driven death event
    public UnityEvent onDied;                 // lecture: Inspector wiring for the designer
    int current;
    float invulnerableUntil;

    public int Current => current;
    public int Max => max;

    void Awake() => current = max;

    public bool TakeDamage(int amount)
    {
        if (current <= 0 || Time.time < invulnerableUntil) return false;
        invulnerableUntil = Time.time + iFrames;
        if (shielded)
        {
            shielded = false;
            return false;
        }
        current -= amount;
        Damaged?.Invoke(current, max);
        GameEvents.RaisePlayerDamaged(current, max);
        if (current <= 0)
        {
            Died?.Invoke();                   // ADDED
            GameEvents.RaisePlayerDied();     // ADDED
            onDied.Invoke();
        }
        return true;
    }

    public void Restore(float protection)
    {
        current = max;
        invulnerableUntil = Time.time + protection;
        Healed?.Invoke(current, max);
    }
}