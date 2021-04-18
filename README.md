# Battleships
Battleships game with 2 bots playing with each other. Game logic is located in GameLogicService.cs along with Player.cs.

# Rules
The game assumes Hasbro rules i.e. there is no restriction on ships touching each other and there is always player change after a move, regardless of the outcome of the last move.

# The task
Based on the rules of the Battleship gameboard (https://en.wikipedia.org/wiki/Battleship_(game)) implement a program that randomly places ships on two boards and simulate the gameplay between 2 players.

# Frontend
![Page](https://github.com/Mikolaj-Walkowiak/Battleships/blob/main/screenshots/frontend.png?raw=true)
The frontend is a simple react app made using a part of the example project provided by Microsoft. There is a landing page describing the application and a simulation page where you can see the outcome of the battleship game played by the bots. There is currently no play history, although it is going to change.

# Backend
There were a few design choices, mostly due to the Hasbro rules making thing difficult.
### Boards
Each player, like in real life, has two boards, one containing their own ships, one containing their guesses. They are 2D tables, although a 1D table combined with an enumerable move type could make a small performance bonus. Alas, it was decided that readability was more important.
### Placing the ships
The game begins with each player placing their ships randomly, it was achieved by getting a random spot on the board and checking if any of the four directions are possible. It is a basic-but-good-enough algorithm for 5 ships and a rather large board. Each ship coordinates are also kept in order to declare a ship dead. It is necessary due to the Hasbro rules.
### Making a move
There are three (or four, more on that later) game states a player can be in.
- Searching, where the player is going diagonally or randomly searching for a ship
- Found, which is a state where the player made the first hit and is trying to determine where to shoot
- Direction, when the orientation of the ship is known and the player follows along
- hasbroFix, due to the Hasbro rules, it is possible for a player to kill e.g. 3 cells and have a miss on both sides. It means that each of the three cells is actually a different ship and for each cell, we need to go perpendicularly to kill the ships.

### Getting results
After making a move player sends a Tuple to the enemy player, where he checks whether he has a ship in that location or not. He then sends that as a result to the opposite player where he has to update his state depending on the result:

If Miss, he will, depending on the previous state, continue searching, change direction in a {found} state, or try to go the opposite way in a directional state. If in a directional state he then receives two misses. it is presumed that the player found a fake ship created by the Hasbro rules
If Hit, he will go to the next state (search -> found -> directional)
If Kill, he will mark all damaged ships along the direction of the last hit. It is the only way to limit fake markings created by the Hasbro rules, although it might be impossible to be better even for humans in some cases
If GameOver, the player killed the last ship remaining and the game is stopped by the game controller.
