using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Data
{
    class Message
    {
        public Member source { get; }
        public Member target { get; }

        public string text = "";

        public string type;
    }
}
