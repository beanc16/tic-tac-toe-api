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
         *      - Can only make a move if the game is IN_PROGRESS
         *      - Can only make one move at a time
         *      - Can only make a move if it's your turn
         *          - Update who's turn it is in player data
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

                List<Board> moveHistory = obj["moveHistory"].ToObject<List<Board>>();
                if (moveHistory == null || moveHistory.Count == 0)
                {
                    return BadRequest("No move was made");
                }

                FilterDefinition<Game> filter = Game.GetFilterById(gameId);
                List<Game> games = MongoConnection<Game>.Find(Collections.GAMES, filter);
                if (games.Count == 0)
                {
                    return BadRequest("No game exists with the ID: " + gameId);
                }

                // Display the most recent moves first
                moveHistory.Reverse();

                // Add given moves to game
                games[0].MoveHistory.InsertRange(0, moveHistory);
                games[0].UpdateAfterMove();

                // Add moves to database
                MongoConnection<Game>.ReplaceOne(Collections.GAMES, games[0], filter);

                return Ok(games[0]);
            }

            // Sent form data
            else
            {
                return BadRequest("Must post data as content-type application/json");
            }
        }
    }
}
