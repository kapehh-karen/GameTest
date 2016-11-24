using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Test.Enums;

namespace Test.Models
{
    public class GameInstance
    {
        public GameInstance()
        {
            // Initialize map
            for (int x = 0; x <= Map.GetUpperBound(1); x++)
                for (int y = 0; y <= Map.GetUpperBound(0); y++)
                    Map[y, x] = PositionInfo.NONE;
        }

        public bool Step(GameStep step)
        {
            if (State != GameState.WAIT_PLAYER)
                return false;

            // Out of range
            if ((step.X > Map.GetUpperBound(1)) || (step.Y > Map.GetUpperBound(0)) ||
                (step.X < 0) || (step.Y < 0))
                return false;

            // Not none
            if (Map[step.Y, step.X] != PositionInfo.NONE)
                return false;

            Map[step.Y, step.X] = PositionInfo.BLUE;

            // if no winners
            if (!CheckWinner())
            {
                // AI step
                State = GameState.WAIT_AI;
                HostingEnvironment.QueueBackgroundWorkItem(ct => DoStep());
            }
            return true;
        }

        public PositionInfo[,] Map { get; set; } = new PositionInfo[3, 3];

        public GameState State { get; set; } = GameState.WAIT_PLAYER;

        private void DoStep()
        {
            var gameStepForWin = GetWinnerStep();

            if (gameStepForWin != null)
            {
                // Если есть возможность выиграть, или не дать победить игроку,
                // то делаем туда ход
                Map[gameStepForWin.Y, gameStepForWin.X] = PositionInfo.RED;
            }
            else
            {
                // Если ситуация нейтральна, делаем рандомный ход
                RandomStep();
            }

            // Если нет победителей
            if (!CheckWinner())
                State = GameState.WAIT_PLAYER;
        }

        private void RandomStep()
        {
            int rounds = new Random().Next(10, 100);
            bool endRound = false;

            while (!endRound)
            {
                for (int x = 0; x <= Map.GetUpperBound(1) && !endRound; x++)
                    for (int y = 0; y <= Map.GetUpperBound(0) && !endRound; y++)
                        if (Map[y, x] == PositionInfo.NONE)
                            if (rounds > 0)
                            {
                                rounds--;
                            }
                            else
                            {
                                Map[y, x] = PositionInfo.RED;
                                endRound = true;
                            }
            }
        }

        private bool CheckWinner()
        {
            var winner = GetWinner();

            switch (winner)
            {
                case PositionInfo.RED:
                    State = GameState.AI_WIN;
                    break;

                case PositionInfo.BLUE:
                    State = GameState.PLAYER_WIN;
                    break;

                case PositionInfo.NONE:
                default:
                    return false;
            }

            return true;
        }

        private PositionInfo GetWinner()
        {
            if (Map[0, 0] == Map[1, 1] && Map[0, 0] == Map[2, 2])
                return Map[0, 0];

            if (Map[2, 0] == Map[1, 1] && Map[2, 0] == Map[0, 2])
                return Map[2, 0];


            for (int i = 0; i <= 2; i++)
                if (Map[i, 0] == Map[i, 1] && Map[i, 0] == Map[i, 2])
                    return Map[i, 0];

            for (int i = 0; i <= 2; i++)
                if (Map[0, i] == Map[1, i] && Map[0, i] == Map[2, i])
                    return Map[0, i];


            return PositionInfo.NONE;
        }

        private bool ExistsWinStep(PositionInfo a, PositionInfo b, PositionInfo c)
        {
            bool existNone = (a == PositionInfo.NONE) || (b == PositionInfo.NONE) || (c == PositionInfo.NONE);
            bool correntWinLine = (a != PositionInfo.NONE && a == b) || (a != PositionInfo.NONE && a == c) || (b != PositionInfo.NONE && b == c);
            return correntWinLine && existNone;
        }

        private GameStep GetWinnerStep()
        {
            if (ExistsWinStep(Map[0, 0], Map[1, 1], Map[2, 2]))
            {
                if (Map[0, 0] == PositionInfo.NONE)
                    return new GameStep() { Y = 0, X = 0 };
                if (Map[1, 1] == PositionInfo.NONE)
                    return new GameStep() { Y = 1, X = 1 };
                if (Map[2, 2] == PositionInfo.NONE)
                    return new GameStep() { Y = 2, X = 2 };
            }

            if (ExistsWinStep(Map[2, 0], Map[1, 1], Map[0, 2]))
            {
                if (Map[2, 0] == PositionInfo.NONE)
                    return new GameStep() { Y = 2, X = 0 };
                if (Map[1, 1] == PositionInfo.NONE)
                    return new GameStep() { Y = 1, X = 1 };
                if (Map[0, 2] == PositionInfo.NONE)
                    return new GameStep() { Y = 0, X = 2 };
            }

            for (int i = 0; i <= 2; i++)
                if (ExistsWinStep(Map[i, 0], Map[i, 1], Map[i, 2]))
                {
                    if (Map[i, 0] == PositionInfo.NONE)
                        return new GameStep() { Y = i, X = 0 };
                    if (Map[i, 1] == PositionInfo.NONE)
                        return new GameStep() { Y = i, X = 1 };
                    if (Map[i, 2] == PositionInfo.NONE)
                        return new GameStep() { Y = i, X = 2 };
                }

            for (int i = 0; i <= 2; i++)
                if (ExistsWinStep(Map[0, i], Map[1, i], Map[2, i]))
                {
                    if (Map[0, i] == PositionInfo.NONE)
                        return new GameStep() { Y = 0, X = i };
                    if (Map[1, i] == PositionInfo.NONE)
                        return new GameStep() { Y = 1, X = i };
                    if (Map[2, i] == PositionInfo.NONE)
                        return new GameStep() { Y = 2, X = i };
                }

            return null;
        }
    }
}