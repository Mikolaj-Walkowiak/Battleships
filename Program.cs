using System;
using System.Collections.Generic;

namespace Battleships
{
    class Program
    {
        static void Main(string[] args)
        {
            Player player1 = new Player(PlayMode.Diagonal);
            Player player2 = new Player(PlayMode.Random);
            Result result = Result.Miss;
            int winner = -1;
            int cnt = 0;
            while (true)
            {
                result = player2.GetHit(player1.MakeAMove());
                player1.GetResult(result);
                //player1.DrawMap();
                cnt++;
                if(cnt %20 == 0)
                {

                }
                if (result == Result.GameOver) { winner = 1; break; }

                result = player1.GetHit(player2.MakeAMove());
                player2.GetResult(result);
                //player2.DrawMap();
                if (result == Result.GameOver) { winner = 2; break; }
            }
            Console.Clear();
            Console.WriteLine("Player{0} won!", winner);
            Console.WriteLine("Player1 Map:");
            player1.DrawMap();
            Console.WriteLine("\nPlayer2 Map:");
            player2.DrawMap();
        }
    }
}
