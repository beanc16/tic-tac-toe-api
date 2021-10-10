using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TicTacToeApi.Models;
using TicTacToeApi.Models.Enums;
using MongoDBHelpers;

namespace TicTacToeApi.Controllers
{
    [Route("game")]
    public class GameController : Controller
    {
        // /game
        [HttpGet]
        public ActionResult Index()
        {
            return Redirect("/game/all");
        }

        // /game/all
        [HttpGet("all")]
        public ActionResult All(string id)
        {
            List<Game> games = MongoConnection<Game>.FindAll(Collections.GAMES);
            return Ok(games);
        }

        /* TODO: Start endpoint for:
         * - Get - Passing in no player names (auto-generate both names; neither is CPU)
         * - Post - Passing in no player names (auto-generate both names; default to same settings as get unless told otherwise)
         * - Get - Passing in one player name (auto-generate second; auto-generated name is CPU, given name is real player)
         * - Post - Passing in one player name (auto-generate second; default to same settings as get unless told otherwise)
         * - Get - Passing in two player names (neither is CPU)
         * - Post - Passing in two player names (default to same settings as get unless told otherwise))
         * 
         * RULES:
         * - Game cannot consist of two CPUs (must have at least one player)
         */

        // /game/start
        [HttpGet("start")]
        public ActionResult Start()
        {
            Game game = new Game();
            MongoConnection<Game>.InsertOne(Collections.GAMES, game);
            return Ok(game);
        }

        // /game/{gameId}
        [HttpGet("{gameId}")]
        public ActionResult Id(string gameId)
        {
            FilterDefinition<Game> filter = Game.GetFilterById(gameId);
            List<Game> games = MongoConnection<Game>.Find(Collections.GAMES, filter);
            return Ok(games);
        }
    }
}
