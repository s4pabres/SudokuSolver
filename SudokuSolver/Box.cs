using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;

/**     - Jede Zelle enthält ein Array der Größe 9
        - Der Inhalt dieses Arrays sind die Zahlen 1 - 9
        - Immer wenn ein Input des Managers kommt, welches den (Nicht)-Wert der Zelle enthält,
          wird das jeweilige Array durchlaufen, der Wert wird gesucht und gelöscht
        - Sollte das Array nur noch eine Größe von "1" haben, 
          wird der Inhalt dieses Arrays an den Manager gesendet
**/

namespace SudokuSolver
{
    internal class Box
    {
        private readonly Cell[,] _cells = new Cell[3, 3];
        private static ConcurrentDictionary<string, TcpClient> NeighbourClients = new ConcurrentDictionary<string, TcpClient>();

        public string Name { get; private set; }

        public Box(string name, string Init)
        {
            Name = name;

            for (ushort i = 0; i < 3; i++)
                for (ushort j = 0; j < 3; j++)
                    _cells[i, j] = new Cell(this,i,j,0);

            foreach (var cell in Init.Split(','))
            {
                var x = (ushort)(cell[0] - '0');
                var y = (ushort)(cell[1] - '0');
                var value = (ushort)(cell[3] - '0');

                _cells[x, y].Value = value;
            }
        }
        
        public void DetermineValues()
        {
            foreach (var cell in _cells)
                if (cell.Value != 0)
                    foreach (var cell2 in _cells)
                    {
                        if (cell == cell2)
                            continue;

                        cell2.PossibleValues.Remove(cell.Value);
                    }

            foreach (var cell in _cells)
                if (cell.PossibleValues.Count == 1)
                    cell.SetValue(cell.PossibleValues.Last.Value);
        }

        public void RemoveValue(ushort x, ushort y, ushort value)
        {
            var cell = _cells[x, y];
            if (cell.Value != 0)
                foreach (var cell2 in _cells)
                {
                    if (cell == cell2 || (cell.X != cell2.X && cell.Y != cell2.Y)  )
                        continue;

                    cell2.PossibleValues.Remove(cell.Value);
                }

            foreach (var c in _cells)
                if (c.PossibleValues.Count == 1)
                    c.SetValue(c.PossibleValues.Last.Value);
        }

        public override string ToString()
        {
            var s = "";
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++) s += _cells[j, i].Value + " ";
                s += "\n";
            }

            return s;
        }

        private class Cell
        {
            public Cell(Box box, ushort x, ushort y, ushort value)
            {
                Box = box;
                Value = value;
                X = x;
                Y = y;
                for (ushort i = 1; i < 10; i++)
                    if (i != value)
                        PossibleValues.AddLast(i);
            }

            public LinkedList<ushort> PossibleValues { get; } = new LinkedList<ushort>();
            public Box Box { get; }
            public ushort Value;
            public ushort X { get; }
            public ushort Y { get; }

            public void SetValue(ushort value)
            {
                PossibleValues.Remove(value);
                Value = value;

                foreach ( var s in Helper.Neighbours[Box.Name] )
                {
                    Message.SendMessage( NeighbourClients[s], Box.Name + ","+X+","+Y+":"+Value );
                }

                Box.DetermineValues();
            }
        }
    }
}
