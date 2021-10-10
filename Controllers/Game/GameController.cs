using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Data;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TicTacToeApi.Models;
using TicTacToeApi.Models.Enums;
using MongoDBHelpers;
using HttpRequestHelpers;

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
         * RULES:
         * - Game cannot consist of two CPUs (must have at least one player)
         */

        // /game/start
        [HttpPost("start")]
        public async Task<ActionResult> Start(string formKey)
        {
            string reqStr = await Request.GetRawBodyStringAsync();

            // Sent raw data
            if (reqStr.Length > 0)
            {
                Game game = JsonConvert.DeserializeObject<Game>(reqStr);
                MongoConnection<Game>.InsertOne(Collections.GAMES, game);
                return Ok(game);
            }

            // Sent form data
            else
            {
                return BadRequest("Must post data as content-type application/json");
            }
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
