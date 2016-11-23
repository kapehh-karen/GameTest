using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Test.Models;

namespace Test.Controllers
{
    public class GameController : ApiController
    {
        private static Dictionary<String, GameInstance> games = new Dictionary<String, GameInstance>();

        private GameInstance FindGame(string token)
        {
            var dictItem = games.FirstOrDefault(e => e.Key.Equals(token));

            if (dictItem.Value == null)
                return null;
            else
                return dictItem.Value;
        }

        // Create game: api/Game
        public string GetNew()
        {
            var gameToken = DateTime.Now.Ticks.ToString("x"); // Random string
            var gameInstance = new GameInstance();

            games.Add(gameToken, gameInstance);
            return gameToken;
        }

        // Status game by token: api/Game/<token>
        public GameInstance GetStatus(string token)
        {
            var currentGame = FindGame(token);
            if (currentGame == null)
                return null;

            return currentGame;
        }

        // Do step: POST api/Game/<token> | BODY {"X": 0, "Y": 0}
        public bool PostDoStep(string token, [FromBody]GameStep step)
        {
            var currentGame = FindGame(token);
            if (currentGame == null)
                return false;
            
            return currentGame.Step(step);
        }
    }
}
