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
using DotEnvHelpers;

namespace TicTacToeApi.Controllers
{
    // TODO: Better error handling
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

        /* TODO:
         * - Player
         *      - Can't make player with existing ID unless name matches
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

                try
                {
                    MongoConnection<Game>.InsertOne(Collections.GAMES, game);
                    return Ok(game);
                }
                catch (Exception ex)
                {
                    if (ex is MongoBulkWriteException || ex is MongoWriteException)
                    {
                        int code = -1;
                        if (ex is MongoBulkWriteException)
                        {
                            code = ((MongoBulkWriteException)ex).WriteConcernError.Code;
                        }
                        else if (ex is MongoWriteException)
                        {
                            code = ((MongoWriteException)ex).WriteError.Code;
                        }

                        if (code == 11000)
                        {
                            return Conflict("Error: A game with that ID already exists");
                        }
                    }

                    return BadRequest("An unknown error occurred " +
                                        "when inserting game into " +
                                        "database");
                }
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
