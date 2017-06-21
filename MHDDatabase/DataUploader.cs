using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
                if (vehicleIndices.Count == 0 || vehicleIndices[vehicleIndices.Count - 1] == segments.Length - 1) 
                {
                    Console.WriteLine("Most recent entry was not queued due to bad format.");
                    continue;
                }
                int actualIndex = 0;
                while (actualIndex < segments.Length - 1)
                {
                    if (vehicleIndices.IndexOf(actualIndex) < vehicleIndices.Count - 1)
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
                else
                {
                    string[] splits = segments[i].Split('+');
                    int secondNumber;
                    if (int.TryParse(splits[0], out number) && int.TryParse(splits[1], out secondNumber))
                    {
                        if (number > 100 && secondNumber > 100)
                            vehicleIndices.Add(i);
                    }
                }
            }

            return vehicleIndices;
        }

        public Queue fileLoadingMode(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);
            string line;
            Queue queue = new Queue();
            while ((line = reader.ReadLine()) != null)
            {
                string[] segments = line.Split(' ');
                List<int> vehicleIndices = detectVehicles(segments);
                if (vehicleIndices.Count == 0 || vehicleIndices[vehicleIndices.Count - 1] == segments.Length - 1)
                {
                    Console.WriteLine("Most recent entry was not queued due to bad format.");
                    continue;
                }
                int actualIndex = 0;
                while (actualIndex < segments.Length - 1)
                {
                    if (vehicleIndices.IndexOf(actualIndex) < vehicleIndices.Count - 1)
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
    }
}
