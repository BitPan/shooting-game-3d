using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameData;
using Microsoft.Xna.Framework.Content;

namespace ShootingGame.GameComponent
{
    public class GameLevelHandler
    {
        private int[] enemyData;
        public enum GameState { START,INITIALIZE, PLAY, FINISHING ,END };
        public enum GameLevel { Level1, Level2, Level3, Level4, Level5 };
        

        GameLevel currentGameLevel;
        GameState currentGameState;
        LevelData levelData;

        public int[] GetEmemyData { get { return enemyData; } }
        public GameState GetGameState { get { return currentGameState; } }
        public GameState SetGameState { set { currentGameState = value; } }
        public GameLevel GetGameLevel { get { return currentGameLevel; } }

        public GameLevelHandler(ContentManager content)
        {
            List<LevelN> levels = new List<LevelN>();
            levels.Add(content.Load<LevelN>("Configuration/Level/Level1"));
            levels.Add(content.Load<LevelN>("Configuration/Level/Level2"));
            levels.Add(content.Load<LevelN>("Configuration/Level/Level3"));
            levels.Add(content.Load<LevelN>("Configuration/Level/Level4"));
            levels.Add(content.Load<LevelN>("Configuration/Level/Level5"));

            levelData = new LevelData(levels);
            currentGameState = GameState.START;
            currentGameLevel = GameLevel.Level1;
        }
        
        public void UpdateGameStatus(int playerScore, int playerHealth)
        {
            if (playerHealth <= 0)
                currentGameState = GameState.FINISHING;
            else
            {
                if (playerScore < 100)
                    currentGameLevel = GameLevel.Level1;
                else if (playerScore >= 100 && playerScore < 250)
                    currentGameLevel = GameLevel.Level2;
                else if (playerScore >= 250 && playerScore < 450)
                    currentGameLevel = GameLevel.Level3;
                else if (playerScore >= 450 && playerScore < 750)
                    currentGameLevel = GameLevel.Level4;
                else if (playerScore >= 750)
                    currentGameLevel = GameLevel.Level5;

                enemyData = levelData.loadLevelData(currentGameLevel);
            }
        }
    }
}
