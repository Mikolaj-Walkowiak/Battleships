using System;
using System.Collections.Generic;

class GameLogicService : IGameLogicService
{
    Player player1 = new Player(PlayMode.Diagonal);
    Player player2 = new Player(PlayMode.Random);
    private GameLogicModelHelper MakeSnapshot()
    {
        GameLogicModelHelper toRet = new();
        toRet.DiagonalBoard = player1.ReturnState()[0];
        toRet.DiagonalBoardEnemy = player1.ReturnState()[1];
        toRet.RandomBoard = player2.ReturnState()[0];
        toRet.RandomBoardEnemy = player2.ReturnState()[1];
        return toRet;

    }
    public GameLogicModel Move()
    {
        GameLogicModel toRet = new();
        toRet.GameHistory = new();
        Result result = Result.Miss;
        while (true)
        {
            result = player2.GetHit(player1.MakeAMove());
            player1.GetResult(result);
            if (result == Result.GameOver) { break; }

            result = player1.GetHit(player2.MakeAMove());
            player2.GetResult(result);
            if (result == Result.GameOver) {  break; }
            toRet.GameHistory.Add(MakeSnapshot());
        }
        toRet.GameHistory.Add(MakeSnapshot());
        return toRet;
    }
}

public interface IGameLogicService
{
    GameLogicModel Move();
}