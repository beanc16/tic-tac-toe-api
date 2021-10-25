namespace TicTacToeApi.Models.Enums
{
    public struct GameStatus
    {
        public static readonly string IN_PROGRESS = "In Progress";
        public static readonly string HAS_WINNER = "Complete";
        public static readonly string HAS_TIE = "Tie";
    }
}