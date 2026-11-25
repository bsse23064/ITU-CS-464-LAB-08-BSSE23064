using System;

// Events with no particular sender. They are static, so they outlive every scene:
// whatever subscribes must unsubscribe in OnDisable.
public static class GameEvents
{
    public static event Action<int, int> PlayerDamaged;
    public static event Action<int> CoinCollected;
    public static event Action PlayerDied;     // ADDED
    public static event Action GameStarted;    // ADDED
    public static event Action GameWon;        // ADDED
    public static event Action GameLost;       // ADDED

    // Only this class can invoke its events, so the rest of the game raises them through these.
    public static void RaisePlayerDamaged(int current, int max) => PlayerDamaged?.Invoke(current, max);
    public static void RaiseCoinCollected(int value) => CoinCollected?.Invoke(value);
    public static void RaisePlayerDied() => PlayerDied?.Invoke();
    public static void RaiseGameStarted() => GameStarted?.Invoke();
    public static void RaiseGameWon() => GameWon?.Invoke();
    public static void RaiseGameLost() => GameLost?.Invoke();
}