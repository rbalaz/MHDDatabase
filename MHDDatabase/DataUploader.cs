using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHDDatabase
{
    class DataUploader
    {
        private Queue queue;
        public DataUploader()
        {
            queue = new Queue();
        }

        public Queue enteringMode()
        {
            string line = "";
            Queue queue = new Queue();
            while(true)
            {
                Console.WriteLine("Please enter one or multiple entries:");
                line = Console.ReadLine();
                if (line.Equals("Terminate") || line.Equals("terminate"))
                    break;
                string[] segments = line.Split(' ');
                List<int> vehicleIndices = detectVehicles(segments);
                int actualIndex = 0;
                while (actualIndex < segments.Length - 1)
                {
                    if (vehicleIndices[vehicleIndices.IndexOf(actualIndex)] < vehicleIndices.Count - 1)
                    {
                        int endIndex = vehicleIndices[vehicleIndices.IndexOf(actualIndex) + 1];
                        Entry entry = new Entry(segments.Skip(actualIndex).Take(endIndex - actualIndex).ToArray());
                        queue.queuedEntries.Add(entry);
                        actualIndex = endIndex;
                    }
                    else
                    {
                        int endIndex = segments.Length;
                        Entry entry = new Entry(segments.Skip(actualIndex).Take(endIndex - actualIndex).ToArray());
                        queue.queuedEntries.Add(entry);
                        actualIndex = endIndex;
                    }
                }
            }

            return queue;
        }

        private List<int> detectVehicles(string[] segments)
        {
            List<int> vehicleIndices = new List<int>();
            for (int i = 0; i < segments.Length; i++)
            {
                int number;
                if (int.TryParse(segments[i], out number))
                {
                    if (number > 100)
                        vehicleIndices.Add(i);
                }
            }

            return vehicleIndices;
        }
    }
}
