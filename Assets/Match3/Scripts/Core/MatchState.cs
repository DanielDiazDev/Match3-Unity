namespace Match3.Scripts.Core
{
    public enum MatchState
    {
        None,
        Idle,
        GeneratePieces,
        Swap,
        FindMatch,
        Destroy,
        PowerUpGeneration,
        PowerUpActivate,
        Gravity,
        Refill,
        Shuffle,
        GameOver,
        LevelWin
    }
}
