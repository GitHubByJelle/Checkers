using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAC;
using GameLoopC;

namespace Checkers
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set up game loop
            GameLoop gl = new GameLoop();
            //gl.setupSimulate(new double[] { .1});

            //gl.playGames(1);

            gl.start();

            //GA ga = new GA(16, 168);
            //ga.run(1000, 100);

            Console.WriteLine("Test");
        }
    }
}
