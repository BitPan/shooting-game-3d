using System;
using System.Collections.Generic;
using System.Linq;
using ShootingGame.Core;
using ShootingGame.GameComponent;

namespace ShootingGame
{
    class LevelData
    {

        private List<int[]> levelData;
 
        public LevelData()
        {
            levelData = new List<int[]>();

            levelData.Add(iniLevelData(2500, 2000, 6, 4, 8, 1000, 100, 800, 400));
            levelData.Add(iniLevelData(2400, 1450, 6, 4, 8, 13, 100, 900, 120));
            levelData.Add(iniLevelData(2300, 1400, 6, 4, 8, 11, 100, 1000, 110));
            levelData.Add(iniLevelData(2200, 1350, 7, 4, 8, 8, 100, 1100, 100));
            levelData.Add(iniLevelData(2100, 1300, 7, 4, 8, 6, 100, 1200, 90));
        }

        public int[] loadLevelData(GameLevelHandler.GameLevel gameLevel )
        {
            switch (gameLevel)
            {
                case GameLevelHandler.GameLevel.Level1:
                    return levelData[0];
                case GameLevelHandler.GameLevel.Level2:
                    return levelData[1];
                case GameLevelHandler.GameLevel.Level3:
                    return levelData[2];
                case GameLevelHandler.GameLevel.Level4:
                    return levelData[3];
                case GameLevelHandler.GameLevel.Level5:
                    return levelData[4];
            }
            return null;
        }

        private int[] iniLevelData(int enemySpawnCd, 
            int enemyShootCd, 
            int enemyBulletSpeed, 
            int enemyMovingSpeed,
            int enemyAttackDeviationFactor,
            int enemyAttackChanceFactor,
            int deviationRange,
            int enemyAttackRange,
            int enemyTurnAroundFactor)
        {
            int[] level = { enemySpawnCd, enemyShootCd, enemyBulletSpeed, enemyMovingSpeed, enemyAttackDeviationFactor, enemyAttackChanceFactor, deviationRange, enemyAttackRange, enemyTurnAroundFactor };
            return level;
        }



    }
}
