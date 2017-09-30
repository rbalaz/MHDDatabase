using System;
using System.Collections.Generic;
using System.IO;

namespace MHDDatabase
{
    class DataLoader
    {
        public List<Route> loadRoutes(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                FileStream stream = File.Create(filePath);
                stream.Close();
                return new List<Route>();
            }
            else
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(stream);

                List<Route> routes = new List<Route>();
                TypeDatabase database = new TypeDatabase("routesDatabase.txt");
                database.loadDatabase();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim() == "")
                        continue;
                    string[] segments = line.Split(' ');

                    Route route = new Route(segments[0], database.getType(segments[0]), int.Parse(segments[1]));
                    routes.Add(route);
                }
                reader.Close();
                stream.Close();

                return routes;
            }
        }

        public List<Route> loadRoutes(string[] years)
        {
            List<Route> result = new List<Route>();
            for (int i = 0; i < years.Length; i++)
            {
                string filePath = years[i] + @"\routes" + years[i] + ".txt";
                if (File.Exists(filePath) == false)
                {
                    FileStream stream = File.Create(filePath);
                    stream.Close();
                    return new List<Route>();
                }
                else
                {
                    FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(stream);

                    List<Route> routes = new List<Route>();
                    TypeDatabase database = new TypeDatabase("routesDatabase.txt");
                    database.loadDatabase();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Trim() == "")
                            continue;
                        string[] segments = line.Split(' ');

                        Route route = new Route(segments[0], database.getType(segments[0]), int.Parse(segments[1]));
                        routes.Add(route);
                    }
                    reader.Close();
                    stream.Close();

                    result = mergeRouteLists(result, routes);
                }
            }
            return result;
        }

        public List<Vehicle> loadVehicles(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                FileStream stream = File.Create(filePath);
                stream.Close();
                return new List<Vehicle>();
            }
            else
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(stream);

                List<Vehicle> vehicles = new List<Vehicle>();
                TypeDatabase database = new TypeDatabase("vehiclesDatabase.txt");
                database.loadDatabase();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim() == "")
                        continue;
                    string[] segments = line.Split(' ');

                    Vehicle vehicle = new Vehicle(segments[0], database.getType(segments[0]), int.Parse(segments[1]));
                    vehicles.Add(vehicle);
                }
                reader.Close();
                stream.Close();

                return vehicles;
            }
        }

        public List<Vehicle> loadVehicles(string[] years)
        {
            List<Vehicle> result = new List<Vehicle>();
            for (int i = 0; i < years.Length; i++)
            {
                string filePath = years[i] + @"\vehicles" + years[i] + ".txt";
                if (File.Exists(filePath) == false)
                {
                    FileStream stream = File.Create(filePath);
                    stream.Close();
                    return new List<Vehicle>();
                }
                else
                {
                    FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(stream);

                    List<Vehicle> vehicles = new List<Vehicle>();
                    TypeDatabase database = new TypeDatabase("vehiclesDatabase.txt");
                    database.loadDatabase();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Trim() == "")
                            continue;
                        string[] segments = line.Split(' ');

                        Vehicle vehicle = new Vehicle(segments[0], database.getType(segments[0]), int.Parse(segments[1]));
                        vehicles.Add(vehicle);
                    }
                    reader.Close();
                    stream.Close();

                    result = mergeVehicleLists(result, vehicles);
                }
            }

            return result;
        }

        private List<Route> mergeRouteLists(List<Route> first, List<Route> second)
        {
            List<Route> result = new List<Route>();
            foreach (Route route in first)
                result.Add(route);
            foreach (Route route in second)
            {
                int index = result.FindIndex(r => r.route == route.route);
                if (index == -1)
                    result.Add(route);
                else
                    result[index].amount += route.amount;
            }

            return result;
        }

        private List<Vehicle> mergeVehicleLists(List<Vehicle> first, List<Vehicle> second)
        {
            List<Vehicle> result = new List<Vehicle>();
            foreach (Vehicle vehicle in first)
                result.Add(vehicle);
            foreach (Vehicle vehicle in second)
            {
                int index = result.FindIndex(v => v.vehicle == vehicle.vehicle);
                if (index == -1)
                    result.Add(vehicle);
                else
                    result[index].amount += vehicle.amount;
            }

            return result;
        }


        public int[] loadPercentage(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                FileStream stream = File.Create(filePath);
                stream.Close();
                return new int[] { 0, 0 };
            }
            else
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(stream);

                try
                {
                    reader.ReadLine(); // skipping first line
                    string passedLine = reader.ReadLine();
                    string[] passedLineParts = passedLine.Split(' ');
                    int passed = int.Parse(passedLineParts[passedLineParts.Length - 1]);
                    string notPassedLine = reader.ReadLine();
                    string[] notPassedLineParts = notPassedLine.Split(' ');
                    int notPassed = int.Parse(notPassedLineParts[notPassedLineParts.Length - 1]);
                    reader.Close();
                    stream.Close();

                    return new int[] { passed, notPassed };
                }
                catch (NullReferenceException)
                {
                    reader.Close();
                    stream.Close();
                    return new int[] { 0, 0 };
                }
            }
        }

        public int[] loadPercentage(string[] years)
        {
            int resultPassed = 0, resultNotPassed = 0;
            for (int i = 0; i < years.Length; i++)
            {
                string filePath = years[i] + @"\data" + years[i] + ".txt";
                if (File.Exists(filePath) == false)
                {
                    FileStream stream = File.Create(filePath);
                    stream.Close();
                    return new int[] { 0, 0 };
                }
                else
                {
                    FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(stream);

                    try
                    {
                        reader.ReadLine(); // skipping first line
                        string passedLine = reader.ReadLine();
                        string[] passedLineParts = passedLine.Split(' ');
                        int passed = int.Parse(passedLineParts[passedLineParts.Length - 1]);
                        string notPassedLine = reader.ReadLine();
                        string[] notPassedLineParts = notPassedLine.Split(' ');
                        int notPassed = int.Parse(notPassedLineParts[notPassedLineParts.Length - 1]);
                        reader.Close();
                        stream.Close();

                        resultPassed += passed;
                        resultNotPassed += notPassed;
                    }
                    catch (NullReferenceException)
                    {
                        reader.Close();
                        stream.Close();
                    }
                }
            }

            return new int[] { resultPassed, resultNotPassed };
        }
    }
}
