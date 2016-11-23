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
            for (int x = 0; x <= Map.GetUpperBound(0); x++)
                for (int y = 0; y <= Map.GetUpperBound(1); y++)
                    Map[x, y] = PositionInfo.NONE;
        }

        public bool Step(GameStep step)
        {
            if (State != GameState.WAIT_PLAYER)
                return false;

            // Out of range
            if ((step.X > Map.GetUpperBound(1)) || (step.Y > Map.GetUpperBound(0)) ||
                (step.X < 0) || (step.Y < 0))
                return false;

            Map[step.Y, step.X] = PositionInfo.BLUE;

            // AI step
            State = GameState.WAIT_AI;
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoStep());
            return true;
        }

        public PositionInfo[,] Map { get; set; } = new PositionInfo[3, 3];
        public GameState State { get; set; } = GameState.WAIT_PLAYER;

        private void DoStep()
        {
            Thread.Sleep(5000);
            State = GameState.WAIT_PLAYER;
        }
    }
}