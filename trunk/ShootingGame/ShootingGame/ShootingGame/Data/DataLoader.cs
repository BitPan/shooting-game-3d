using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShootingGame.Data
{
    class DataLoader
    {
        private string path { get; set; }
        public DataLoader(string path)
        {
            this.path = path;
        }


        public int[,] loadMap(string path)
        {
            int[,] a = new int[20, 15];
            String numbers = "";

            int lineNum = 0;
            using (StreamReader sr = new StreamReader("Map.txt"))
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
