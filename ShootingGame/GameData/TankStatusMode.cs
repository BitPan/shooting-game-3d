using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameData
{
    public class TankStatusMode
    {
        public TankStatusMode(Mode mode)
        {
            this.status_wander = mode.statu[0].ToString();
            this.status_STOP = mode.statu[1].ToString();
            this.status_FOLLOW = mode.statu[2].ToString();
            this.status_fire = mode.statu[3].ToString();
            this.name = mode.name;
            this.hp = mode.hp;

        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private int hp;

        public int Hp
        {
            get { return hp; }
            set { hp = value; }
        }

        private string status_wander;

        public string Status_wander
        {
            get { return status_wander; }
            set { status_wander = value; }
        }
        private string status_STOP;

        public string Status_STOP
        {
            get { return status_STOP; }
            set { status_STOP = value; }
        }
        private string status_FOLLOW;

        public string Status_FOLLOW
        {
            get { return status_FOLLOW; }
            set { status_FOLLOW = value; }
        }
        private string status_fire;

        public string Status_fire
        {
            get { return status_fire; }
            set { status_fire = value; }
        }

 
    }
}
