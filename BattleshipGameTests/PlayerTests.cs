using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class PlayerTests
    {
        private const char shipRepresentation = '1';
        private const char unknownRepresentation = '0';
        private const char missRepresentation = '4';

        [TestMethod()]
        public void ReturnState_InitialStateTest()
        {
            Player player1 = new Player(PlayMode.Diagonal);
            List<string> result = player1.ReturnState();
            int counter = 0;
            int expectedShips = 5 + 4 + 3 + 3 + 2;
            foreach (char field in result[0])
            {
                if (field == shipRepresentation)
                {
                    counter += 1;
                }
            }
            counter = 0;
            foreach (char field in result[1])
            {
                if (field == unknownRepresentation)
                {
                    counter += 1;
                }
            }
            Assert.AreEqual(100, counter);
        }

        [TestMethod()]
        public void MakeAMove_DiagonalPlayerTest()
        {
            Player player1 = new Player(PlayMode.Diagonal);
            Player player2 = new Player(PlayMode.Random);
            string initialState = player2.ReturnState()[0];
            string finalState;
            Result result = Result.Miss;
            while (initialState[0] == shipRepresentation || initialState[11] == shipRepresentation)
            {
                player2 = new Player(PlayMode.Random);
                initialState = player2.ReturnState()[0];
            }
            result = player2.GetHit(player1.MakeAMove());
            player1.GetResult(result);
            result = player2.GetHit(player1.MakeAMove());
            player1.GetResult(result);
            finalState = player1.ReturnState()[1];
            Assert.IsTrue(finalState[0] == missRepresentation && finalState[11] == missRepresentation);

        }

        [TestMethod()]
        public void MakeAMove_MissTest()
        {
            Player player1 = new Player(PlayMode.Diagonal);
            Player player2 = new Player(PlayMode.Random);
            string initialState = player2.ReturnState()[0];
            Result result = Result.Miss;
            while (initialState[0] == shipRepresentation)
            {
                player2 = new Player(PlayMode.Random);
                initialState = player2.ReturnState()[0];
            }
            result = player2.GetHit(player1.MakeAMove());
            player1.GetResult(result);
            Assert.AreEqual(Result.Miss, result);
        }
        [TestMethod()]
        public void MakeAMove_HitTest()
        {
            Player player1 = new Player(PlayMode.Diagonal);
            Player player2 = new Player(PlayMode.Random);
            string initialState = player2.ReturnState()[0];
            Result result = Result.Miss;
            while (initialState[0] != shipRepresentation)
            {
                player2 = new Player(PlayMode.Random);
                initialState = player2.ReturnState()[0];
            }
            result = player2.GetHit(player1.MakeAMove());
            player1.GetResult(result);
            Assert.AreEqual(Result.Hit, result);
        }
    }
}