using UnityEngine;

public static class ScoreManager
{
    public static int Score { get; private set; } = 0;
    public static event System.Action<int> OnScoreChanged;

    public static void AddPoints(int delta)
    {
        Score += delta;
        OnScoreChanged?.Invoke(Score);
        Debug.Log($"Score updated: {Score}");
    }

    public static void Reset()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);
    }
}
