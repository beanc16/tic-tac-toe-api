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
                Player.FromBoardMarkAndIsTurn(BoardMark.X, true),
                Player.FromBoardMarkAndIsTurn(BoardMark.O, false),
            };

            // Expire 5 minutes from the time of creation
            ExpireAt = DateTime.Now.AddMinutes(5);
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



        public override string ToString()
        {
            string template = "Game: {{\n" +
                "\t\"ID\": {0}\n" +
                "\t\"Status\": {1}\n" +
                "\t\"MoveHistory\": {2}\n" +
                "\t\"Players\": [\n" +
                    "\t\t{3},\n" +
                    "\t\t{4}\n" +
                "\t]\n" +
                "\t\"ExpireAt\": {5}\n" +
            "}}";

            string moveHistoryStr = "[\n";
            foreach (Board board in MoveHistory)
            {
                moveHistoryStr += board.ToStringTabbed() + "\n";
            }
            moveHistoryStr += "\t]";

            return string.Format(
                template, 
                Id, 
                Status, 
                moveHistoryStr, 
                Players[0].ToStringTabbed(), 
                Players[1].ToStringTabbed(), 
                ExpireAt
            );
        }
    }
}