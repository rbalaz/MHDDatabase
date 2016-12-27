using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHDDatabase
{
    class Entry
    {
        public string[] arguments { get; private set; }

        public Entry(string[] arguments)
        {
            this.arguments = arguments;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < arguments.Length; i++)
                s = string.Concat(s, arguments[i] + " ");

            return s;
        }
    }
}
