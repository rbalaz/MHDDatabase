using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHDDatabase
{
    class DataSaver
    {
        public void saveVehicles(List<Vehicle> vehicles)
        {
            vehicles.OrderByDescending(vehicle => vehicle.amount);
            FileStream stream = new FileStream("vehicles" + DateTime.Now.Year + ".txt", FileMode.Create, FileAccess.Write);
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
            routes.OrderByDescending(route => route.amount);
            FileStream stream = new FileStream("routes" + DateTime.Now.Year + ".txt", FileMode.Create, FileAccess.Write);
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
            FileStream stream = new FileStream("data" + DateTime.Now.Year + ".txt", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine("Rate of passes at Magistrate crossroads:");
            writer.WriteLine("Number of times passed: " + passingData[0]);
            writer.WriteLine("Number of times not passed: " + passingData[1]);
        }
    }
}
