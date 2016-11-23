using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public enum GameState
    {
        NONE = -1, // Нету игры с таким токеном
        WAIT_PLAYER = 1, // Ожидает хода от игрока
        WAIT_AI = 2, // Думает комп
        AI_WIN = 3, // Компьютер победитель
        PLAYER_WIN = 4 // Игрок победитель
    }
}