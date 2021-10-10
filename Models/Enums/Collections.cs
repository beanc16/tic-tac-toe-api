using DotEnvHelpers;

namespace TicTacToeApi.Models.Enums
{
    public struct Collections
    {
        public static readonly string GAMES = DotEnvHelper.GetEnvVariable("COLLECTION_GAMES");
    }
}