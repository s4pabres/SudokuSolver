using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Box
    {
        public ushort[,] Cells { get; private set; } = new ushort[3, 3] ;
        private Dictionary<ushort, Dictionary<ushort, LinkedList<ushort>>> PossibleValues = new Dictionary<ushort, Dictionary<ushort, LinkedList<ushort>>>();


        public Box(string Init)
        {

            for(ushort i = 0;i<3;i++)
            {
                for (ushort j = 0; j < 3; j++)
                {
                    for (ushort v = 1; v < 10; v++)
                    {
                        PossibleValues[i][j].AddLast(v);
                    }
                }
            }

            foreach (var cell in Init.Split(','))
            {
                ushort x = (ushort)(cell[0] - '0');
                ushort y = (ushort)(cell[1] - '0');
                ushort value = (ushort)(cell[3] - '0');

                Cells[x, y] = value;
                RemPossibleValue(x, y, value);

            }
        }

        private ushort RemPossibleValue(ushort x, ushort y, ushort value)
        {
            if (PossibleValues[x][y].Contains(value))
            {
                
            }
            return 0;
        }
    }
}
