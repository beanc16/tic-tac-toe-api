using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TicTacToeApi.Models;
using TicTacToeApi.Models.Enums;
using MongoDBHelpers;
using HttpRequestHelpers;

namespace TicTacToeApi.Controllers
{
    // TODO: Better error handling
    public class MovesController : Controller
    {
        // /game/{gameId}/moves
        [HttpGet]
        public ActionResult Index(string gameId)
        {
            FilterDefinition<Game> filter = Game.GetFilterById(gameId);
            List<Game> games = MongoConnection<Game>.Find(Collections.GAMES, filter);

            if (games.Count > 0)
            {
                List<Board> moveHistory = games[0].MoveHistory;
                return Ok(moveHistory);
            }

            else
            {
                return BadRequest(games);
            }
        }

        /* TODO:
         * - moveHistory Rules
         *      - Can only make one move at a time
         *      - Can only make a move if it's your turn
         *          - Update who's turn it is in player data
         *      - Can't update move of player with useApiCpu set to true
         *      - Must pass moves in the order that they occurred (1 move, 2 moves, 3 moves, etc.)
         * - AI
         *      - Add useApiCpu to player
         *      - Make CPU take a turn after non-CPU player goes if useApiCpu is true
         */
        // /game/{gameId}/moves
        [HttpPatch]
        public async Task<ActionResult> Index(string gameId, string _)
        {
            string reqStr = await Request.GetRawBodyStringAsync();

            // Sent raw data
            if (reqStr.Length > 0)
            {
                JObject obj = JsonConvert.DeserializeObject<JObject>(reqStr);
                if (obj == null || obj["moveHistory"] == null)
                {
                    return BadRequest("moveHistory not found");
                }

                List<Board> movesToMake = obj["moveHistory"].ToObject<List<Board>>();
                if (movesToMake == null || movesToMake.Count == 0)
                {
                    return BadRequest("No move was made");
                }

                FilterDefinition<Game> filter = Game.GetFilterById(gameId);
                List<Game> games = MongoConnection<Game>.Find(Collections.GAMES, filter);
                if (games.Count == 0)
                {
                    return BadRequest("No game exists with the ID: " + gameId);
                }

                // Game is not in progress
                if (games[0].Status == GameStatus.HAS_WINNER)
                {
                    return BadRequest("The game with ID " + gameId + 
                                      " already has a winner. " + 
                                      "No more moves can be made.");
                }
                else if (games[0].Status == GameStatus.HAS_TIE)
                {
                    return BadRequest("The game with ID " + gameId + 
                                      " has a tie. " + 
                                      "No more moves can be made.");
                }
                
                return MakeAMoveInGame(movesToMake, games[0], filter);
            }

            // Sent form data
            else
            {
                return BadRequest("Must post data as content-type application/json");
            }
        }

        private OkObjectResult MakeAMoveInGame(
            List<Board> movesToMake, 
            Game game, 
            FilterDefinition<Game> filter
        )
        {
            // Display the most recent moves first
            movesToMake.Reverse();

            // Add given moves to game
            game.MoveHistory.InsertRange(0, movesToMake);
            game.UpdateAfterMove();

            // Add moves to database
            MongoConnection<Game>.ReplaceOne(Collections.GAMES, game, filter);

            return Ok(game);
        }
    }
}
