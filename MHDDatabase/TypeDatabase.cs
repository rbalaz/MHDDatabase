using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MHDDatabase
{
    class TypeDatabase
    {
        public class DatabaseError : Exception { }

        private string filePath;
        private List<string> busTypes;
        private List<string> trolleyTypes;
        private List<string> tramTypes;
        private List<string> electroTypes;

        public TypeDatabase(string filePath)
        {
            this.filePath = filePath;
        }

        public void loadDatabase()
        {
            if (File.Exists(filePath) == false)
                throw new FileNotFoundException();
            else
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(stream);

                string line;
                busTypes = new List<string>();
                tramTypes = new List<string>();
                trolleyTypes = new List<string>();
                electroTypes = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] segments = line.Split(' ');
                    if (segments[1] == Types.Bus.ToString())
                        busTypes.Add(segments[0]);
                    if (segments[1] == Types.Electrobus.ToString())
                        electroTypes.Add(segments[0]);
                    if (segments[1] == Types.Tram.ToString())
                        tramTypes.Add(segments[0]);
                    if (segments[1] == Types.Trolleybus.ToString())
                        trolleyTypes.Add(segments[0]);
                }
            }
        }

        public Types getType(string item)
        {            
            if (busTypes.Exists(s => s.Equals(item)))
                return Types.Bus;
            if (tramTypes.Exists(s => s.Equals(item)))
                return Types.Tram;
            if (electroTypes.Exists(s => s.Equals(item)))
                return Types.Electrobus;
            if (trolleyTypes.Exists(s => s.Equals(item)))
                return Types.Trolleybus;

            throw new DatabaseError();
        }

        public void updateDatabase(string[] entry)
        {
            FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine(entry);

            writer.Close();
            stream.Close();
        }

        public void deleteFromDatabase(string[] entry)
        {
            List<string> lines = new List<string>(File.ReadAllLines(filePath));
            lines.RemoveAt(lines.IndexOf(entry[0] + " " + entry[1]));
            File.WriteAllLines(filePath, lines);
        }

        public void listDatabase()
        {
            Console.WriteLine("Listing chosen part of database:");
            Console.WriteLine(">>>Buses<<<");
            if (busTypes.Count == 0)
                Console.WriteLine("No entries available.");
            else
            {
                foreach (string s in busTypes)
                    Console.WriteLine(s);
            }

            Console.WriteLine(">>>Trolleybuses<<<");
            if (trolleyTypes.Count == 0)
                Console.WriteLine("No entries available.");
            else
            {
                foreach (string s in trolleyTypes)
                    Console.WriteLine(s);
            }

            Console.WriteLine(">>>Trams<<<");
            if (tramTypes.Count == 0)
                Console.WriteLine("No entries available.");
            else
            {
                foreach (string s in tramTypes)
                    Console.WriteLine(s);
            }
            if (filePath.ToLower().Contains("vehicle"))
            {
                Console.WriteLine(">>>Electrobuses<<<");
                if (electroTypes.Count == 0)
                    Console.WriteLine("No entries available.");
                else
                {
                    foreach (string s in electroTypes)
                        Console.WriteLine(s);
                }
            }
        }
    }
}
