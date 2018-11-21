using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            string Name = args[0];
            string ManagerURI = args[2];
            ushort[,] Cells = new ushort[3,3];
            foreach (var cell in args[1].Split(','))
            {
                Cells[(ushort)(cell[0] - '0'), (ushort) (cell[1] - '0')] = (ushort) (cell[3] -'0');
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.WriteLine(Cells[i,j]);
                }
            }

            Console.ReadKey();
        }
    }
}
