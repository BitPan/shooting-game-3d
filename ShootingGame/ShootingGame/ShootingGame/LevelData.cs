using System;
using System.Collections.Generic;
using System.Linq;

namespace ShootingGame
{
    class LevelData
    {

        private List<int[]> levelData;
 
        public LevelData()
        {
            levelData = new List<int[]>();

            levelData.Add(iniLevelData(2500, 1500, 6, 4, 8, 15, 100, 800));
            levelData.Add(iniLevelData(2400, 1450, 6, 4, 8, 13, 100, 900));
            levelData.Add(iniLevelData(2300, 1400, 6, 4, 8, 11, 100, 1000));
            levelData.Add(iniLevelData(2200, 1350, 7, 4, 8, 8, 100, 1100));
            levelData.Add(iniLevelData(2100, 1300, 7, 4, 8, 6, 100, 1200));
        }

        public int[] loadLevelData(Game1.GameLevel gameLevel )
        {
            switch (gameLevel)
            {
                case Game1.GameLevel.LEVEL1:
                    return levelData[0];
                case Game1.GameLevel.LEVEL2:
                    return levelData[1];
                case Game1.GameLevel.LEVEL3:
                    return levelData[2];
                case Game1.GameLevel.LEVEL4:
                    return levelData[3];
                case Game1.GameLevel.LEVEL5:
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
            int enemyAttackRange)
        {
            int[] level = { enemySpawnCd, enemyShootCd, enemyBulletSpeed, enemyMovingSpeed, enemyAttackDeviationFactor, enemyAttackChanceFactor, deviationRange, enemyAttackRange };
            return level;
        }



    }
}
