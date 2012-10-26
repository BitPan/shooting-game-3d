using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameData
{
    public class Level
    {

        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

    
        public int enemySpawnCd;

        public int EnemySpawnCd
        {
            get { return enemySpawnCd; }
            set { enemySpawnCd = value; }
        }
        private int enemyShootCd;

        public int EnemyShootCd
        {
            get { return enemyShootCd; }
            set { enemyShootCd = value; }
        }
        private int enemyBulletSpeed;

        public int EnemyBulletSpeed
        {
            get { return enemyBulletSpeed; }
            set { enemyBulletSpeed = value; }
        }
        private int enemyMovingSpeed;

        public int EnemyMovingSpeed
        {
            get { return enemyMovingSpeed; }
            set { enemyMovingSpeed = value; }
        }
        private int enemyAttackDeviationFactor;

        public int EnemyAttackDeviationFactor
        {
            get { return enemyAttackDeviationFactor; }
            set { enemyAttackDeviationFactor = value; }
        }
        private int enemyAttackChanceFactor;

        public int EnemyAttackChanceFactor
        {
            get { return enemyAttackChanceFactor; }
            set { enemyAttackChanceFactor = value; }
        }
        private int deviationRange;

        public int DeviationRange
        {
            get { return deviationRange; }
            set { deviationRange = value; }
        }
        private int enemyAttackRange;

        public int EnemyAttackRange
        {
            get { return enemyAttackRange; }
            set { enemyAttackRange = value; }
        }
        private int enemyTurnAroundFactor;

        public int EnemyTurnAroundFactor
        {
            get { return enemyTurnAroundFactor; }
            set { enemyTurnAroundFactor = value; }
        }
        

        public Level() { 
        
        
        }

    }


}
