﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MHDDatabase
{
    class DataLoader
    {
        public List<Route> loadRoutes(string filePath)
        {
            if (File.Exists(filePath) == false)
                throw new FileNotFoundException();
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
                    string[] segments = line.Split(' ');

                    Route route = new Route(segments[0], database.getType(segments[0]), int.Parse(segments[1]));
                    routes.Add(route);
                }
                reader.Close();
                stream.Close();

                return routes;
            }
        }

        public List<Vehicle> loadVehicles(string filePath)
        {
            if (File.Exists(filePath) == false)
                throw new FileNotFoundException();
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
                    string[] segments = line.Split(' ');

                    Vehicle vehicle = new Vehicle(segments[0], database.getType(segments[0]), int.Parse(segments[1]));
                    vehicles.Add(vehicle);
                }
                reader.Close();
                stream.Close();

                return vehicles;
            }
        }

        public int[] loadPercentage(string filePath)
        {
            if (File.Exists(filePath) == false)
                throw new FileNotFoundException();
            else
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(stream);

                string passedLine = reader.ReadLine();
                string[] passedLineParts = passedLine.Split(' ');
                int passed = int.Parse(passedLineParts[passedLineParts.Length - 1]);
                string notPassedLine = reader.ReadLine();
                string[] notPassedLineParts = notPassedLine.Split(' ');
                int notPassed = int.Parse(notPassedLineParts[notPassedLineParts.Length - 1]);
                reader.Close();
                stream.Close();

                return new int[] { passed, notPassed};
            }
        }
    }
}
