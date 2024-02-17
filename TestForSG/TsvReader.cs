using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestForSG
{
    internal class TsvReader
    {
        private readonly StreamReader streamReader;
        private int lineCounter = 0;
        public TsvReader(StreamReader streamReader)
        {
            this.streamReader = streamReader;
        }

        public string[]? ReadNextRecord()
        {
            var line = streamReader.ReadLine();
            if (line == null)
            {
                return null;
            }
            if (lineCounter == 0)
            {
                line = streamReader.ReadLine();
            }
            var lineSplit = line.Split("\t");
            for (int i = 0; i < lineSplit.Length; i++)
            {
                lineSplit[i] = lineSplit[i].Trim();
            }
            lineCounter++;

            return lineSplit;
        }
    }
}
