using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TicTacToeApi.Models.Enums;

namespace TicTacToeApi.Models
{
    public class Game
    {
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("moveHistory")]
        public List<Board> MoveHistory { get; set; }

        [BsonElement("players")]
        public Player[] Players { get; set; }

        [BsonElement("expireAt")]
        public DateTime ExpireAt { get; set; }

        public Game()
        {
            // Generate a random ID
            Id = Guid.NewGuid().ToString();

            // Game is in progress
            Status = GameStatus.IN_PROGRESS;

            // Add a blank board to the history
            MoveHistory = new List<Board>();
            Board board = new Board();
            MoveHistory.Add(board);

            // Add two new players to the tuple
            Players = new Player[] {
                Player.FromBoardMark(BoardMark.X),
                Player.FromBoardMark(BoardMark.O),
            };

            // Expire 1 minute from the time of creation
            ExpireAt = DateTime.Now;
            ExpireAt.AddMinutes(1);
        }



        public void MarkColumn(BoardMark mark, int rowNum, int columnNum)
        {
            Board lastBoard = MoveHistory.FindLast(_ => true);
            Board newBoard = lastBoard.Clone();

            newBoard.MarkColumn(mark, rowNum, columnNum);
            MoveHistory.Add(newBoard);
        }



        public static FilterDefinition<Game> GetFilterById(string id)
        {
            FilterDefinitionBuilder<Game> filterBuilder = 
                Builders<Game>.Filter;
            return filterBuilder.Eq("_id", id);
        }
    }
}