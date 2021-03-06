﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MHDDatabase
{
    class DataSaver
    {
        private int year = DateTime.Now.Year;

        public DataSaver(int year)
        {
            this.year = year;
        }

        public void saveVehicles(List<Vehicle> vehicles)
        {
            vehicles = vehicles.OrderByDescending(vehicle => vehicle.amount).ToList();
            FileStream stream = new FileStream(year + @"\vehicles" + year + ".txt", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            foreach (Vehicle vehicle in vehicles)
            {
                writer.WriteLine(vehicle);
            }
            writer.Close();
            stream.Close();
        }

        public void saveRoutes(List<Route> routes)
        {
            routes = routes.OrderByDescending(route => route.amount).ToList();
            FileStream stream = new FileStream(year + @"\routes" + year + ".txt", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            foreach (Route route in routes)
            {
                writer.WriteLine(route);
            }
            writer.Close();
            stream.Close();
        }

        public void savePercentage(int[] passingData)
        {
            FileStream stream = new FileStream(year + @"\data" + year + ".txt", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine("Rate of passes at Magistrate crossroads:");
            writer.WriteLine("Number of times passed: " + passingData[0]);
            writer.WriteLine("Number of times not passed: " + passingData[1]);

            writer.Close();
            stream.Close();
        }

        public void saveProcessedEntries(List<string> processedEntries)
        {
            if (File.Exists(year + @"\raw" + year + ".txt") == false)
                File.Create(year + @"\raw" + year + ".txt");
            FileStream stream = new FileStream(year + @"\raw" + year + ".txt", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            foreach (string entry in processedEntries)
                writer.WriteLine(entry);

            writer.Close();
            stream.Close();
        }
    }
}
