using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameData
{
    [Serializable]
    public class LevelN
    {
        public int enemySpawnCd;
        public int enemyShootCd;
        public int enemyBulletSpeed;
        public int enemyMovingSpeed;
        public int enemyAttackDeviationFactor;
        public int enemyAttackChanceFactor;
        public int deviationRange;
        public int enemyAttackRange;
        public int enemyTurnAroundFactor;
    }
}
