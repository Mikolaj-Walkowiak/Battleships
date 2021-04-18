import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public gameState: GameLogicModel[];
  public DiagonalBoard: string[][];
  public DiagonalBoardEnemy: string[][];
  public RandomBoard: string[][];
  public RandomBoardEnemy: string[][];
  public iterationCount: number = 0;

  init(history: number) {
    for (var i = 0; i < 10; ++i) {
      this.DiagonalBoard[i] = [];
      this.DiagonalBoardEnemy[i] = [];
      this.RandomBoard[i] = [];
      this.RandomBoardEnemy[i] = [];
      for (var j = 0; j < 10; ++j) {
        this.DiagonalBoard[i][j] = this.gameState[0].gameHistory[history].diagonalBoard[i * 10 + j];
        this.DiagonalBoardEnemy[i][j] = this.gameState[0].gameHistory[history].diagonalBoardEnemy[i * 10 + j];
        this.RandomBoard[i][j] = this.gameState[0].gameHistory[history].randomBoard[i * 10 + j];
        this.RandomBoardEnemy[i][j] = this.gameState[0].gameHistory[history].randomBoardEnemy[i * 10 + j];
      }
    }
  }
  next(howMuch: number): void {
    if (this.iterationCount + howMuch < this.gameState[0].gameHistory.length) {
      this.DiagonalBoard[0][0] = "4";
      this.iterationCount = this.iterationCount + howMuch;
      this.init(this.iterationCount);
    }
  }

  back(howMuch: number): void {
    if (this.iterationCount -howMuch > 0) {
      this.iterationCount = this.iterationCount - howMuch;
      this.init(this.iterationCount);
    }
  }
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.DiagonalBoard = [];
    this.DiagonalBoardEnemy = [];
    this.RandomBoard = [];
    this.RandomBoardEnemy = [];
    http.get<GameLogicModel[]>(baseUrl + 'GameLogic').subscribe(result => {
      this.gameState = result;
      this.init(this.iterationCount);
    }, error => console.error(error));
  }
}

interface GameLogicModelHelper {
  diagonalBoard: string;
  diagonalBoardEnemy: string;
  randomBoard: string;
  randomBoardEnemy: string;
}

interface GameLogicModel {
  gameHistory: Array<GameLogicModelHelper>;
}
