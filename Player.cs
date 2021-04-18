using System;
using System.Collections.Generic;
public enum PlayMode : int
{
    Random = 0,
    Diagonal = 1 // apparently going in a diagonal line when firing
                 // is +ev for the battleship game
}
enum CurrentBehaviour : int
{
    Searching = 0,
    Found = 1,
    Up = 2,
    Right = 3,
    Down = 4,
    Left = 5
}
public enum Result : int
{
    Miss = 0,
    Hit = 1,
    Kill = 2,
    GameOver = 3
}
enum PointType : int
{
    Empty = 0,
    KilledShip = 1,
    DamagedShip = 2,
    Ship = 3,
    Unknown = 4,
    Forbidden = 5
}
enum Direction : int
{
    Up = 2,
    Right = 3,
    Down = 4,
    Left = 5,
    None = -1
}


public class Player
{
    Random random = new Random();

    PlayMode playMode;
    CurrentBehaviour currentBehaviour;
    PointType[,] playerBoard = new PointType[10, 10];
    PointType[,] enemyBoard = new PointType[10, 10];
    Direction lastDirection;

    int currentX, currentY, changeX, changeY;
    Tuple<int, int> lastMove, firstHit = new(0, 0);
    List<Direction> possibleDirections = new();
    List<int> shipsToPlace = new List<int>() { 2, 3, 3, 4, 5 };

    /*HASBRO FIX*/
    bool isDeadlocked = false;
    bool hasbroFixRequired = false;
    List<Tuple<int, int>> possibleFakeShips = new List<Tuple<int, int>>();
    List<List<Tuple<int, int>>> hasbroShipList = new();
    /*HASBRO FIX*/
    private void CreateBoardHelper(Tuple<int, int> startingPoint, int shipToPlace)
    {
        possibleDirections.Clear();
        bool canBePlaced;
        int shipX, shipY;
        Direction shipDirection;
        shipX = startingPoint.Item1;
        shipY = startingPoint.Item2;
        if (shipY >= shipToPlace - 1) possibleDirections.Add(Direction.Up); //won't be out of bounds
        if (shipY + shipToPlace - 1 <= 9) possibleDirections.Add(Direction.Down);
        if (shipX >= shipToPlace - 1) possibleDirections.Add(Direction.Left);
        if (shipX + shipToPlace - 1 <= 9) possibleDirections.Add(Direction.Right);


        while (possibleDirections.Count > 0)
        {
            shipDirection = possibleDirections[random.Next(possibleDirections.Count)];
            canBePlaced = true;
            switch (shipDirection)
            {
                case Direction.Up:
                    for (int i = 0; i < shipToPlace; ++i)
                    {
                        if (playerBoard[shipX, shipY - i] != PointType.Empty)
                        {
                            possibleDirections.Remove(shipDirection);
                            canBePlaced = false;
                        }
                    }
                    if (canBePlaced)
                    {
                        List<Tuple<int, int>> shipList = new();
                        for (int i = 0; i < shipToPlace; ++i)
                        {
                            playerBoard[shipX, shipY - i] = PointType.Ship;
                            shipList.Add(Tuple.Create(shipX, shipY - i));
                        }
                        shipsToPlace.Remove(shipToPlace);
                        hasbroShipList.Add(shipList);
                    }
                    break;
                case Direction.Down:
                    for (int i = 0; i < shipToPlace; ++i)
                    {
                        if (playerBoard[shipX, shipY + i] != PointType.Empty)
                        {
                            possibleDirections.Remove(shipDirection);
                            canBePlaced = false;
                        }
                    }
                    if (canBePlaced)
                    {
                        List<Tuple<int, int>> shipList = new();
                        for (int i = 0; i < shipToPlace; ++i)
                        {
                            playerBoard[shipX, shipY + i] = PointType.Ship;
                            shipList.Add(Tuple.Create(shipX, shipY + i));
                        }
                        shipsToPlace.Remove(shipToPlace);
                        hasbroShipList.Add(shipList);
                    }
                    break;
                case Direction.Left:
                    for (int i = 0; i < shipToPlace; ++i)
                    {
                        if (playerBoard[shipX - i, shipY] != PointType.Empty)
                        {
                            possibleDirections.Remove(shipDirection);
                            canBePlaced = false;
                        }
                    }
                    if (canBePlaced)
                    {
                        List<Tuple<int, int>> shipList = new();
                        for (int i = 0; i < shipToPlace; ++i)
                        {
                            playerBoard[shipX - i, shipY] = PointType.Ship;
                            shipList.Add(Tuple.Create(shipX - i, shipY));
                        }
                        shipsToPlace.Remove(shipToPlace);
                        hasbroShipList.Add(shipList);
                    }
                    break;
                case Direction.Right:
                    for (int i = 0; i < shipToPlace; ++i)
                    {
                        if (playerBoard[shipX + i, shipY] != PointType.Empty)
                        {
                            possibleDirections.Remove(shipDirection);
                            canBePlaced = false;
                        }
                    }
                    if (canBePlaced)
                    {
                        List<Tuple<int, int>> shipList = new();
                        for (int i = 0; i < shipToPlace; ++i)
                        {
                            playerBoard[shipX + i, shipY] = PointType.Ship;
                            shipList.Add(Tuple.Create(shipX + i, shipY));
                        }
                        shipsToPlace.Remove(shipToPlace);
                        hasbroShipList.Add(shipList);
                    }
                    break;
            }
        }
    }
    private void CreateBoard()
    {
        int shipToPlace, shipX, shipY;
        Direction shipDirection;
        Tuple<int, int> startingPoint = new(0, 0);
        List<Tuple<int, int>> possibleMoves = new List<Tuple<int, int>>();
        /*Player board*/
        while (shipsToPlace.Count > 0)
        {
            for (int x = 0; x < 10; ++x)
            {
                for (int y = 0; y < 10; ++y)
                {
                    possibleMoves.Add(Tuple.Create(x, y));
                }
            }
            startingPoint = possibleMoves[random.Next(possibleMoves.Count)];
            shipToPlace = shipsToPlace[random.Next(shipsToPlace.Count)];
            CreateBoardHelper(startingPoint, shipToPlace);
        }

        /*Enemy Board*/
        for (int i = 0; i < enemyBoard.GetLength(0); ++i)
        {
            for (int j = 0; j < enemyBoard.GetLength(1); ++j)
            {
                enemyBoard[i, j] = PointType.Unknown;
            }
        }
    }
    private void UpdateKill(int mode) //0 = self 1 = enemy
    {
        if (mode == 0)
        {
            for (int i = 0; i < playerBoard.GetLength(0); ++i)
            {
                for (int j = 0; j < playerBoard.GetLength(1); ++j)
                {
                    if (playerBoard[i, j] == PointType.DamagedShip) playerBoard[i, j] = PointType.KilledShip; // can be done due to the bot's behaviour
                }
            }
        }
        else if (mode == 1)
        {
            if (currentBehaviour == CurrentBehaviour.Left || currentBehaviour == CurrentBehaviour.Right)
            {
                for (int i = 0; i < enemyBoard.GetLength(0); ++i)
                {
                    if (enemyBoard[i, lastMove.Item2] == PointType.DamagedShip) enemyBoard[i, lastMove.Item2] = PointType.KilledShip;
                }
            }
            if (currentBehaviour == CurrentBehaviour.Down || currentBehaviour == CurrentBehaviour.Up)
            {
                for (int i = 0; i < enemyBoard.GetLength(0); ++i)
                {
                    if (enemyBoard[lastMove.Item1, i] == PointType.DamagedShip) enemyBoard[lastMove.Item1, i] = PointType.KilledShip;
                }
            }
        }
    }

    public Tuple<int, int> MakeAMove()
    {

        if (currentBehaviour == CurrentBehaviour.Searching)
        {
            List<Tuple<int, int>> possibleMoves = new List<Tuple<int, int>>(); //maybe Point?
            if (playMode == PlayMode.Random)
            {
                for (int x = 0; x < 10; ++x)
                {
                    for (int y = 0; y < 10; ++y)
                    {
                        if (enemyBoard[x, y] == PointType.Unknown)
                        {
                            possibleMoves.Add(Tuple.Create(x, y));
                        }
                    }
                }
                lastMove = possibleMoves[random.Next(possibleMoves.Count)];
                currentX = lastMove.Item1;
                currentY = lastMove.Item2;
            }
            else
            {
                while (enemyBoard[currentX, currentY] != PointType.Unknown)
                {
                    currentX += changeX;
                    currentY += changeY;
                    if (currentX == 10)
                    {
                        if (currentX == currentY)
                        {
                            currentX = 0;
                            currentY = 9;
                            changeY = -1;
                        }
                        else
                        {
                            playMode = PlayMode.Random;
                            for (int x = 0; x < 10; ++x)
                            {
                                for (int y = 0; y < 10; ++y)
                                {
                                    if (enemyBoard[x, y] == PointType.Unknown)
                                    {
                                        possibleMoves.Add(Tuple.Create(x, y));
                                    }
                                }
                            }
                            lastMove = possibleMoves[random.Next(possibleMoves.Count)];
                            currentX = lastMove.Item1;
                            currentY = lastMove.Item2;
                            break;
                        }
                    }
                }
                lastMove = Tuple.Create(currentX, currentY);
            }
        }

        else if (currentBehaviour == CurrentBehaviour.Found)
        {
            lastDirection = possibleDirections[random.Next(possibleDirections.Count)];
            switch (lastDirection)
            {
                case Direction.Left:
                    lastMove = Tuple.Create(currentX - 1, currentY);
                    break;
                case Direction.Right:
                    lastMove = Tuple.Create(currentX + 1, currentY);
                    break;
                case Direction.Up:
                    lastMove = Tuple.Create(currentX, currentY - 1);
                    break;
                case Direction.Down:
                    lastMove = Tuple.Create(currentX, currentY + 1);
                    break;
            }
        }
        else
        {
            switch (currentBehaviour)
            {
                case CurrentBehaviour.Left:
                    lastMove = Tuple.Create(lastMove.Item1 - 1, lastMove.Item2);
                    if (lastMove.Item1 < 0 )
                    {
                        if (isDeadlocked) { hasbroFixRequired = true; isDeadlocked = false; currentBehaviour = CurrentBehaviour.Right; lastMove = firstHit = possibleFakeShips[0]; }
                        else
                        {
                            isDeadlocked = true;
                            lastDirection = Direction.Right;
                            lastMove = Tuple.Create(firstHit.Item1 + 1, firstHit.Item2);
                        }
                    }
                    break;
                case CurrentBehaviour.Right:
                    lastMove = Tuple.Create(lastMove.Item1 + 1, lastMove.Item2);
                    if (lastMove.Item1 > 9)
                    {
                        if (isDeadlocked) { hasbroFixRequired = true; isDeadlocked = false; currentBehaviour = CurrentBehaviour.Right; lastMove = firstHit = possibleFakeShips[0]; }
                        else
                        {
                            isDeadlocked = true;
                            lastDirection = Direction.Left;
                            lastMove = Tuple.Create(firstHit.Item1 - 1, firstHit.Item2);
                        }
                    }
                    break;
                case CurrentBehaviour.Up:
                    lastMove = Tuple.Create(lastMove.Item1, lastMove.Item2 - 1);
                    if (lastMove.Item2 < 0)
                    {
                        if (isDeadlocked) { hasbroFixRequired = true; isDeadlocked = false; currentBehaviour = CurrentBehaviour.Right; lastMove = firstHit = possibleFakeShips[0]; }
                        else
                        {
                            isDeadlocked = true;
                            lastDirection = Direction.Down;
                            lastMove = Tuple.Create(firstHit.Item1, firstHit.Item2 + 1);
                        }
                    }
                    break;
                case CurrentBehaviour.Down:
                    lastMove = Tuple.Create(lastMove.Item1, lastMove.Item2 + 1);
                    if (lastMove.Item2 > 9)
                    {
                        if (isDeadlocked) { hasbroFixRequired = true; isDeadlocked = false; currentBehaviour = CurrentBehaviour.Right; lastMove = firstHit = possibleFakeShips[0]; }
                        else
                        {
                            isDeadlocked = true;
                            lastDirection = Direction.Up;
                            lastMove = Tuple.Create(firstHit.Item1, firstHit.Item2 - 1);
                        }
                    }
                    break;
            }

        }
        return lastMove;
    }

    private Result CheckKill(Tuple<int, int> pos)
    {
        bool isKilled, isGameOver;
        isKilled = isGameOver = true;
        for (int i = 0; i < hasbroShipList.Count; ++i)
        {
            hasbroShipList[i].Remove(pos);
            if (hasbroShipList[i].Count == 0)
            {
                hasbroShipList.RemoveAt(i);
                isKilled = true;
                break;
            }
            else isKilled = false;
        }
        playerBoard[pos.Item1, pos.Item2] = PointType.DamagedShip;
        if (isKilled)
        {
            UpdateKill(0);
        }
        /*Check if there are even any ships left*/
        isGameOver = hasbroShipList.Count == 0;
        if (isGameOver) return Result.GameOver;
        else if (isKilled) return Result.Kill;
        else return Result.Hit;
    }
    public Result GetHit(Tuple<int, int> pos)
    {
        if (playerBoard[pos.Item1, pos.Item2] == PointType.Empty) return Result.Miss;
        if (playerBoard[pos.Item1, pos.Item2] == PointType.KilledShip) return Result.Miss;
        else return CheckKill(pos);
    }

    public void GetResult(Result result)
    {
        if (result == Result.Miss)
        {
            enemyBoard[lastMove.Item1, lastMove.Item2] = PointType.Empty;
            switch (currentBehaviour)
            {
                case CurrentBehaviour.Found:
                    possibleDirections.Remove(lastDirection);
                    break;
                case CurrentBehaviour.Up:
                    if (isDeadlocked) { hasbroFixRequired = true; isDeadlocked = false; currentBehaviour = CurrentBehaviour.Right; lastMove = firstHit = possibleFakeShips[0]; }
                    else
                    {
                        isDeadlocked = true;
                        currentBehaviour = CurrentBehaviour.Down;
                        lastMove = firstHit;
                    }
                    break;
                case CurrentBehaviour.Down:
                    if (isDeadlocked) { hasbroFixRequired = true; isDeadlocked = false; currentBehaviour = CurrentBehaviour.Right; lastMove = firstHit = possibleFakeShips[0]; }
                    else
                    {
                        isDeadlocked = true;
                        currentBehaviour = CurrentBehaviour.Up;
                        lastMove = firstHit;
                    }
                    break;
                case CurrentBehaviour.Left:
                    if (isDeadlocked) { hasbroFixRequired = true; isDeadlocked = false; currentBehaviour = CurrentBehaviour.Down; lastMove = firstHit = possibleFakeShips[0]; }
                    else
                    {
                        isDeadlocked = true;
                        currentBehaviour = CurrentBehaviour.Right;
                        lastMove = firstHit;
                    }
                    break;
                case CurrentBehaviour.Right:
                    if (isDeadlocked) { hasbroFixRequired = true; isDeadlocked = false; currentBehaviour = CurrentBehaviour.Down; lastMove = firstHit = possibleFakeShips[0]; }
                    else
                    {
                        isDeadlocked = true;
                        currentBehaviour = CurrentBehaviour.Left;
                        lastMove = firstHit;
                    }
                    break;
            }
        }
        else if (result == Result.Hit)
        {
            enemyBoard[lastMove.Item1, lastMove.Item2] = PointType.DamagedShip;
            if (!hasbroFixRequired) possibleFakeShips.Add(lastMove);

            if (currentBehaviour == CurrentBehaviour.Searching)
            {
                firstHit = Tuple.Create(lastMove.Item1, lastMove.Item2);
                possibleDirections.Clear();
                if (lastMove.Item1 > 1 && enemyBoard[lastMove.Item1-1,lastMove.Item2] == PointType.Unknown) possibleDirections.Add(Direction.Left);
                if (lastMove.Item1 < 9 && enemyBoard[lastMove.Item1 + 1, lastMove.Item2] == PointType.Unknown) possibleDirections.Add(Direction.Right);
                if (lastMove.Item2 > 1 && enemyBoard[lastMove.Item1, lastMove.Item2 - 1] == PointType.Unknown) possibleDirections.Add(Direction.Up);
                if (lastMove.Item2 < 9 && enemyBoard[lastMove.Item1, lastMove.Item2 + 1] == PointType.Unknown) possibleDirections.Add(Direction.Down);
                if (possibleDirections.Count == 0) UpdateKill(1);
                currentBehaviour = CurrentBehaviour.Found;
            }
            else if (currentBehaviour == CurrentBehaviour.Found)
            {
                currentBehaviour = (CurrentBehaviour)lastDirection;
            }
        }
        else if (result == Result.Kill)
        {
            enemyBoard[lastMove.Item1, lastMove.Item2] = PointType.KilledShip;
            UpdateKill(1);
            if (hasbroFixRequired)
            {
                isDeadlocked = false;
                possibleFakeShips.RemoveAt(0);
                if (possibleFakeShips.Count > 0)
                {
                    lastMove = possibleFakeShips[0];
                }
                else
                {
                    hasbroFixRequired = false;
                    currentBehaviour = CurrentBehaviour.Searching;
                }
            }
            else
            {
                isDeadlocked = false;
                possibleFakeShips.Clear();
                currentBehaviour = CurrentBehaviour.Searching;
            }

        }
    }
    public void DrawMap()
    {
        Console.WriteLine("\n\nPlayer: {0}", playMode);
        for (int i = 0; i < playerBoard.GetLength(0); ++i)
        {
            Console.Write('\n');
            for (int j = 0; j < playerBoard.GetLength(1); ++j)
            {
                switch (playerBoard[j, i])
                {
                    case PointType.Empty:
                        Console.Write(".");
                        break;
                    case PointType.Ship:
                        Console.Write("O");
                        break;
                    case PointType.KilledShip:
                        Console.Write("X");
                        break;
                    case PointType.DamagedShip:
                        Console.Write("U");
                        break;
                }
            }
        }
        Console.WriteLine("\n=============");
        for (int i = 0; i < enemyBoard.GetLength(0); ++i)
        {
            Console.Write('\n');
            for (int j = 0; j < enemyBoard.GetLength(1); ++j)
            {
                switch (enemyBoard[j, i])
                {
                    case PointType.Empty:
                        Console.Write(".");
                        break;
                    case PointType.Ship:
                        Console.Write("O");
                        break;
                    case PointType.KilledShip:
                        Console.Write("X");
                        break;
                    case PointType.DamagedShip:
                        Console.Write("U");
                        break;
                    case PointType.Unknown:
                        Console.Write(".");
                        break;
                }
            }
        }
    }
    public Player(PlayMode playMode)
    {
        this.playMode = playMode;
        currentBehaviour = CurrentBehaviour.Searching;
        lastDirection = Direction.None;
        if (playMode == PlayMode.Diagonal)
        {
            currentX = currentY = 0;
            changeX = changeY = 1;
        }
        CreateBoard();
    }
}
