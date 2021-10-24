using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
        public List<Player> Players { get; set; }

        [BsonElement("expireAt")]
        public DateTime ExpireAt { get; set; }

        [BsonElement("winningMove")]
        public Board WinningMove { get; set; }

        public Game()
        {
            // Generate a random ID
            Id = Guid.NewGuid().ToString();

            // Game is in progress
            Status = GameStatus.IN_PROGRESS;

            // Add a blank board to the history
            MoveHistory = new List<Board>(new Board[] {
                new Board(),
            });

            // Add two new players to the list
            Players = new List<Player>(new Player[] {
                Player.FromBoardMarkAndIsTurn(BoardMark.X, true),
                Player.FromBoardMarkAndIsTurn(BoardMark.O, false),
            });

            // Expire 5 minutes from the time of creation
            ExpireAt = DateTime.Now.AddMinutes(5);
        }

        [JsonConstructor]   // Called on JsonConvert.DeserializeObject
        public Game(string id, string status, List<Board> moveHistory,
                    List<Player> players, DateTime expireAt, 
                    Board winningMove)
        {
            // Id
            if (id != null && id.Length > 0)
            {
                Id = id;
            }
            else
            {
                Id = Guid.NewGuid().ToString();
            }

            // Status
            if (status != null && status.Length > 0)
            {
                Status = status;
            }
            else
            {
                Status = GameStatus.IN_PROGRESS;
            }

            // MoveHistory
            if (moveHistory != null && moveHistory.Count > 0)
            {
                MoveHistory = moveHistory;
            }
            else
            {
                MoveHistory = new List<Board>(new Board[] {
                    new Board(),
                });
            }

            // Players
            if (players != null && players.Count == 2)
            {
                if (players[0].IsCpu && players[1].IsCpu)
                {
                    Players = new List<Player>(new Player[] {
                        Player.FromBoardMarkAndIsTurn(BoardMark.X, true),
                        Player.FromBoardMarkAndIsTurn(BoardMark.O, false),
                    });
                }
                else
                {
                    Players = players;
                }
            }
            else if (players != null && players.Count == 1)
            {
                Players = new List<Player>(new Player[] {
                    players[0],
                    Player.FromBoardMarkIsCpuAndIsTurn(BoardMark.O, !players[0].IsCpu, false),
                });
            }
            else
            {
                Players = new List<Player>(new Player[] {
                    Player.FromBoardMarkAndIsTurn(BoardMark.X, true),
                    Player.FromBoardMarkAndIsTurn(BoardMark.O, false),
                });
            }

            // Ignore expireAt parameter, only use auto-generated ExpireAt
            // Expire 5 minutes from the time of creation
            ExpireAt = DateTime.Now.AddMinutes(5);

            // WinningMove
            if (winningMove != null)
            {
                WinningMove = winningMove;
            }
            else
            {
                winningMove = null;
            }
        }



        public void MarkColumn(BoardMark mark, int rowNum, int columnNum)
        {
            Board lastBoard = MoveHistory.FindLast(_ => true);
            Board newBoard = lastBoard.Clone();

            newBoard.MarkColumn(mark, rowNum, columnNum);
            MoveHistory.Add(newBoard);

            UpdateAfterMove();
        }

        private void UpdateAfterMove()
        {
            Board lastBoard = MoveHistory.FindLast(_ => true);

            // There's a winner
            if (lastBoard.HasMatch())
            {
                // Update status
                Status = GameStatus.HAS_WINNER;

                // Update winning move
                WinningMove = lastBoard.Clone();
                WinningMove.ClearAllButWinningMarks();

                // Update players
                string winningMark = lastBoard.GetWinningMark();
                foreach (Player player in Players)
                {
                    player.SetIsWinnerBasedOnMark(winningMark);
                }
            }

            // There's no matches and no more moves, thus there's a tie
            else if (lastBoard.HasNoMoreMoves())
            {
                // Update status
                Status = GameStatus.HAS_TIE;

                // Update players
                foreach (Player player in Players)
                {
                    player.SetIsWinnerBasedOnMark(BoardMark.EMPTY);
                }
            }
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