using System.Collections.Generic;

public class GameLogicModelHelper
{
    public string DiagonalBoard { get; set; }

    public string DiagonalBoardEnemy { get; set; }
    public string RandomBoard { get; set; }

    public string RandomBoardEnemy { get; set; }
}

public class GameLogicModel
{
    public List<GameLogicModelHelper> GameHistory { get; set; }
}
