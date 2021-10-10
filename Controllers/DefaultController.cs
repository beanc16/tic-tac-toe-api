using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TicTacToeApi.Controllers
{
    public class DefaultController : Controller
    {
        // /game
        [HttpGet]
        public ActionResult Index()
        {
            return Redirect("/game");
        }
    }
}
