using System.Collections.Generic;

namespace SudokuSolver
{
    internal class Box
    {
        private readonly Cell[ , ] _cells = new Cell[ 3, 3 ];

        private readonly Dictionary < ushort, Dictionary < ushort, LinkedList < ushort > > > _possibleValues =
            new Dictionary < ushort, Dictionary < ushort, LinkedList < ushort > > >();


        public Box( string Init )
        {
            foreach ( var cell in Init.Split( ',' ) )
            {
                var x = (ushort) ( cell[0] - '0' );
                var y = (ushort) ( cell[1] - '0' );
                var value = (ushort) ( cell[3] - '0' );

                _cells[x, y] = new Cell( this, x, y, value );
            }
        }

        private ushort RemPossibleValue( ushort x, ushort y, ushort value )
        {
            if ( _possibleValues[x][y].Contains( value ) )
            {

            }

            return 0;
        }

        public override string ToString()
        {
            var s = "";
            for ( var i = 0; i < 3; i++ )
            {
                for ( var j = 0; j < 3; j++ ) s += _cells[i, j] + " ";
                s += "\n";
            }

            return s;
        }

        private class Cell
        {
            public Cell( Box box, ushort x, ushort y, ushort value )
            {
                Box = box;
                Value = value;
                X = x;
                Y = y;
                for ( ushort i = 1; i < 10; i++ )
                    if ( i != value )
                        PossibleValues.AddLast( i );
            }

            public LinkedList < ushort > PossibleValues { get; } = new LinkedList < ushort >();
            public Box Box { get; }
            public ushort Value { get; private set; }
            public ushort X { get; }
            public ushort Y { get; }

            public void SetValue( ushort value )
            {
                Value = value;
            }
        }
    }
}