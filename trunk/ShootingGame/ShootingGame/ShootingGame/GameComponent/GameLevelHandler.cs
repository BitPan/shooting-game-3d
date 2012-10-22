using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootingGame.GameComponent
{
    public class GameLevelHandler
    {
        private int[] enemyData;
        private int playerScore;
        public enum GameState { START,INITIALIZE, PLAY, END };
        public enum GameLevel { Level1, Level2, Level3, Level4, Level5 };
        

        GameLevel currentGameLevel;
        GameState currentGameState;
        LevelData levelData;

        public int GetPlayerScore { get { return playerScore; } }
        public int[] GetEmemyData { get { return enemyData; } }
        public GameState GetGameState { get { return currentGameState; } }
        public GameState SetGameState { set { currentGameState = value; } }
        public GameLevel GetGameLevel { get { return currentGameLevel; } }

        public GameLevelHandler()
        {
            levelData = new LevelData();
            playerScore = 0;
            currentGameState = GameState.START;
            currentGameLevel = GameLevel.Level1;
        }

        public void AddPlayerScore(int points)
        {
            this.playerScore += 10;
        }

        public void UpdateGameStatus()
        {
            if (playerScore < 50)
                currentGameLevel = GameLevel.Level1;
            else if (playerScore >= 50 && playerScore < 100)
                currentGameLevel = GameLevel.Level2;
            else if (playerScore >= 100 && playerScore < 200)
                currentGameLevel = GameLevel.Level3;
            else if (playerScore >= 200 && playerScore < 400)
                currentGameLevel = GameLevel.Level4;
            else if (playerScore >= 400 )
                currentGameLevel = GameLevel.Level5;

            enemyData = levelData.loadLevelData(currentGameLevel);
        }
    }
}
