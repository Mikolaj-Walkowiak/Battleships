using System;
using System.Collections.Generic;

class GameLogicService : IGameLogicService
{
    public GameLogicModel Move()
    {
        Player player1 = new Player(PlayMode.Diagonal);
        Player player2 = new Player(PlayMode.Random);
        GameLogicModel toRet = new();
        Result result = Result.Miss;
        while (true)
        {
            result = player2.GetHit(player1.MakeAMove());
            player1.GetResult(result);
            if (result == Result.GameOver) { break; }

            result = player1.GetHit(player2.MakeAMove());
            player2.GetResult(result);
            if (result == Result.GameOver) {  break; }
        }
        toRet.DiagonalBoard = player1.ReturnState()[0];
        toRet.DiagonalBoardEnemy = player1.ReturnState()[1];
        toRet.RandomBoard = player2.ReturnState()[0];
        toRet.RandomBoardEnemy = player2.ReturnState()[1];
        return toRet;
    }
}

public interface IGameLogicService
{
    GameLogicModel Move();
}