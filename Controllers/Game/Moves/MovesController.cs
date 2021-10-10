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

        // TODO: Post - Make a move in a game (only if it's your turn)
        // /game/{gameId}/moves
    }
}
