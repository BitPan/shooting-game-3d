using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GameData;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace ShootingGame.Data
{
    class DataLoader
    {
        private string path { get; set; }
        public DataLoader()
        {
           
        }

        //public Level LoadLevel(int levelNumber)
        //{

        //    XDocument document = XDocument.Load("Level.xml");
        //    var levels = document.Document.Descendants(XName.Get("Level"));
        //    Level newLevel = new Level(); ;
            

        //}


        public int[,] loadMap(string path)
        {
            int[,] a = new int[20, 15];
            String numbers = "";

            int lineNum = 0;
            using (StreamReader sr = new StreamReader(TitleContainer.OpenStream(path)))
            {
                while (sr.Peek() >= 0)
                {
                    numbers = sr.ReadLine().Replace(" ", "");

                    for (int i = 0; i < 15; i++)
                    {
                        a[lineNum, i] = int.Parse("" + numbers[i]);
                    }
                    lineNum++;
                }
            }

            return a;

        }


    }
}
