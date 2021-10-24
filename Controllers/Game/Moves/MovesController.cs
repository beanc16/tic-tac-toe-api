﻿using System;
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
         *      - Can't remove moves
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
                // Initialize out variables
                List<Board> movesToMake;
                List<Game> games;
                FilterDefinition<Game> filter;
                BadRequestObjectResult badRequest;

                // Get data needed to make a move or a bad request
                GetBadRequestOrData(
                    gameId, 
                    reqStr, 
                    out movesToMake, 
                    out games, 
                    out filter, 
                    out badRequest
                );

                // An error occurred or a moveHistory rule wasn't followed
                if (badRequest != null)
                {
                    return badRequest;
                }
                
                return MakeAMoveInGame(movesToMake, games[0], filter);
            }

            // Sent form data
            else
            {
                return BadRequest("Must post data as content-type application/json");
            }
        }



        /*
         * HELPERS
         */
        
        private void GetBadRequestOrData(
            string gameId,
            string reqStr,
            out List<Board> movesToMake,
            out List<Game> games,
            out FilterDefinition<Game> filter,
            out BadRequestObjectResult badRequest
        )
        {
            // Set default values
            movesToMake = null;
            games = null;
            filter = null;
            badRequest = null;

            JObject obj = JsonConvert.DeserializeObject<JObject>(reqStr);
            GetMoveHistoryNotFoundError(obj, out badRequest);

            movesToMake = obj["moveHistory"].ToObject<List<Board>>();
            GetNoMoveWasMadeError(movesToMake, badRequest, out badRequest);

            filter = Game.GetFilterById(gameId);
            games = MongoConnection<Game>.Find(Collections.GAMES, filter);
            GetNoGameExistsWithThatIdError(gameId, games, badRequest, out badRequest);

            GetGameIsNotInProgressError(gameId, games, badRequest, out badRequest);
            GetOneMoveAtATimeError(movesToMake, games, badRequest, out badRequest);
        }
        
        private void GetMoveHistoryNotFoundError(
            JObject obj,
            out BadRequestObjectResult badRequest
        )
        {
            // Set default values
            badRequest = null;

            if (obj == null || obj["moveHistory"] == null)
            {
                badRequest = BadRequest("moveHistory not found");
                return;
            }
        }
        
        private void GetNoMoveWasMadeError(
            List<Board> movesToMake,
            BadRequestObjectResult curBadRequest,
            out BadRequestObjectResult badRequest
        )
        {
            // Set default values
            badRequest = curBadRequest;

            // Don't overwrite current badRequest
            if (curBadRequest != null)
            {
                return;
            }

            if (movesToMake == null || movesToMake.Count == 0)
            {
                badRequest = BadRequest("No move was made");
                return;
            }
        }
        
        private void GetNoGameExistsWithThatIdError(
            string gameId,
            List<Game> games,
            BadRequestObjectResult curBadRequest,
            out BadRequestObjectResult badRequest
        )
        {
            // Set default values
            badRequest = curBadRequest;
            if (curBadRequest != null)
            {
                return;
            }

            if (games.Count == 0)
            {
                badRequest = BadRequest("No game exists with the ID: " + gameId);
                return;
            }
        }
        
        private void GetGameIsNotInProgressError(
            string gameId,
            List<Game> games,
            BadRequestObjectResult curBadRequest,
            out BadRequestObjectResult badRequest
        )
        {
            // Set default values
            badRequest = curBadRequest;

            // Don't overwrite current badRequest
            if (curBadRequest != null)
            {
                return;
            }

            if (games[0].Status == GameStatus.HAS_WINNER)
            {
                badRequest = BadRequest("The game with ID " + gameId + 
                                        " already has a winner. " + 
                                        "No more moves can be made.");
            }

            else if (games[0].Status == GameStatus.HAS_TIE)
            {
                badRequest = BadRequest("The game with ID " + gameId + 
                                        " has a tie. " + 
                                        "No more moves can be made.");
            }
        }

        private void GetOneMoveAtATimeError(
            List<Board> movesToMake,
            List<Game> games,
            BadRequestObjectResult curBadRequest,
            out BadRequestObjectResult badRequest
        )
        {
            // Set default values
            badRequest = curBadRequest;

            // Don't overwrite current badRequest
            if (curBadRequest != null)
            {
                return;
            }

            for (int i = 1; i <= movesToMake.Count; i++)
            {
                // Move did not go up by one at a time
                if (games[0].MoveHistory[0].NumOfMarks + i != movesToMake[i - 1].NumOfMarks)
                {
                    // Set the current board
                    Board curBoard = movesToMake[i - 1];

                    // Set the previous board
                    Board prevBoard = games[0].MoveHistory[0];
                    if (i != 1)
                    {
                        prevBoard = movesToMake[i - 2];
                    }

                    // The two boards have the same number of moves, thus no move was made
                    if (prevBoard.NumOfMarks == curBoard.NumOfMarks)
                    {
                        // Force a no moves error to occur
                        GetNoMoveWasMadeError(null, badRequest, out badRequest);
                        return;
                    }

                    // Send error
                    string err = String.Format(
                        "You can only make one move at a time. " + 
                        "The previous board has {0} moves and " + 
                        "the current board has {1} moves.\n\n" +
                        "Previous board:\n{2}\n\n" + 
                        "Current board:\n{3}",
                        prevBoard.NumOfMarks,
                        curBoard.NumOfMarks,
                        prevBoard,
                        curBoard
                    );
                    badRequest = BadRequest(err);
                    return;
                }
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
