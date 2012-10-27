using System;
using System.Collections.Generic;
using System.Linq;
using ShootingGame.Core;
using ShootingGame.GameComponent;
using GameData;
using ShootingGame.Data;

namespace ShootingGame
{
    class LevelData
    {

        private List<int[]> levelData;
 
        public LevelData(List<LevelN> levels)
        {
            levelData = new List<int[]>();
            foreach (var level in levels)
            {
                levelData.Add(iniLevelData(level.enemySpawnCd, level.enemyShootCd, level.enemyBulletSpeed, level.enemyMovingSpeed, level.enemyAttackDeviationFactor
                    , level.enemyAttackChanceFactor, level.deviationRange, level.enemyAttackRange, level.enemyTurnAroundFactor));
            }
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
