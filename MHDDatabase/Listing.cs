using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MHDDatabase
{
    class Listing
    {
        private List<Route> routes;
        private List<Vehicle> vehicles;
        private int[] passingData;
        public Listing(List<Route> routes, List<Vehicle> vehicles, int[] passingData)
        {
            this.routes = routes.OrderByDescending(route => route.amount).ToList();
            this.vehicles = vehicles.OrderByDescending(vehicle => vehicle.amount).ToList(); 
            this.passingData = passingData;
        }

        public void listVehicles()
        {
            Console.WriteLine("Listing vehicles in descending order from the most used vehicle:");
            
            List<Vehicle> buses = vehicles.Where(vehicle => vehicle.type == Types.Bus).ToList();
            Console.WriteLine(">>>>Buses<<<<");
            foreach (Vehicle vehicle in buses)
                Console.WriteLine("Evidence number: " + vehicle.vehicle + " Times travelled: " + vehicle.amount);

            List<Vehicle> trams = vehicles.Where(vehicle => vehicle.type == Types.Tram).ToList();
            Console.WriteLine(">>>>Trams<<<<");
            foreach (Vehicle vehicle in trams)
                Console.WriteLine("Evidence number: " + vehicle.vehicle + " Times travelled: " + vehicle.amount);

            List<Vehicle> trolleys = vehicles.Where(vehicle => vehicle.type == Types.Trolleybus).ToList();
            Console.WriteLine(">>>>Trolleybuses<<<<");
            foreach (Vehicle vehicle in trolleys)
                Console.WriteLine("Evidence number: " + vehicle.vehicle + " Times travelled: " + vehicle.amount);

            List<Vehicle> electrobuses = vehicles.Where(vehicle => vehicle.type == Types.Electrobus).ToList();
            Console.WriteLine(">>>>Electrobuses<<<<");
            foreach (Vehicle vehicle in electrobuses)
                Console.WriteLine("Evidence number: " + vehicle.vehicle + " Times travelled: " + vehicle.amount);
        }

        public void listRoutes()
        {
            Console.WriteLine("Listing routes in descending order from the most used route:");

            List<Route> busRoutes = routes.Where(vehicle => vehicle.type == Types.Bus).ToList();
            Console.WriteLine(">>>>Bus routes<<<<");
            foreach (Route route in busRoutes)
                Console.WriteLine("Route number: " + route.route + " Times travelled: " + route.amount);

            List<Route> tramRoutes = routes.Where(vehicle => vehicle.type == Types.Tram).ToList();
            Console.WriteLine(">>>>Tram routes<<<<");
            foreach (Route route in tramRoutes)
                Console.WriteLine("Route number: " + route.route + " Times travelled: " + route.amount);

            List<Route> trolleyRoutes = routes.Where(vehicle => vehicle.type == Types.Trolleybus).ToList();
            Console.WriteLine(">>>>Trolleybus routes<<<<");
            foreach (Route route in trolleyRoutes)
                Console.WriteLine("Route number: " + route.route + " Times travelled: " + route.amount);
        }

        public void listData()
        {
            Console.WriteLine("Listing data about passing through Magistrate crossroads:");
            Console.WriteLine("Times passed: " + passingData[0]);
            Console.WriteLine("Times not passed: " + passingData[1]);
            if (passingData[0] + passingData[1] > 0)
                Console.WriteLine("Chance to pass: " + passingData[0] * 100 / (passingData[0] + passingData[1]) + "%");
            else
                Console.WriteLine("Chance to pass: N/A");
        }

        public void generateReport()
        {
            string currentDate = DateTime.Today.Day + "." + DateTime.Today.Month + "." + DateTime.Today.Year;
            string filename = DateTime.Today.Day + "_" + DateTime.Today.Month + "_" + DateTime.Today.Year + ".txt";
            FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine("Report from MHDDatabase on " + currentDate + ".");
            writer.WriteLine(">>>Leaders in routes sector<<<");
            if (routes.Count > 0)
            {
                List<Route> busRoutes = routes.Where(route => route.type == Types.Bus).ToList();
                if (busRoutes.Count == 0)
                    writer.WriteLine("No bus routes available.");
                else
                {
                    Route busRouteLeader = busRoutes[0];
                    writer.WriteLine("Buses: " + busRouteLeader.route + " Times travelled: " + busRouteLeader.amount + ".");
                }
            }
            else
                writer.WriteLine("No bus routes available.");

            if (routes.Count > 0)
            {
                List<Route> tramRoutes = routes.Where(route => route.type == Types.Tram).ToList();
                if (tramRoutes.Count == 0)
                    writer.WriteLine("No tram routes available");
                else
                {
                    Route tramRouteLeader = tramRoutes[0];
                    writer.WriteLine("Trams: " + tramRouteLeader.route + " Times travelled: " + tramRouteLeader.amount + ".");
                }
            }
            else
                writer.WriteLine("No tram routes available");

            if (routes.Count > 0)
            {
                List<Route> trolleyRoutes = routes.Where(route => route.type == Types.Trolleybus).ToList();
                if (trolleyRoutes.Count == 0)
                    writer.WriteLine("No trolleybus routes avaiable.");
                else
                {
                    Route trolleyRouteLeader = trolleyRoutes[0];
                    writer.WriteLine("Trolleybuses: " + trolleyRouteLeader.route + " Times travelled: " + trolleyRouteLeader.amount + ".");
                }
            }
            else
                writer.WriteLine("No trolleybus routes avaiable.");

            writer.WriteLine(">>>Leaders in vehicles sector<<<");
            if (vehicles.Count > 0)
            {
                List<Vehicle> busVehicles = vehicles.Where(vehicle => vehicle.type == Types.Bus).ToList();
                if (busVehicles.Count == 0)
                    writer.WriteLine("No bus vehicles available.");
                else
                {
                    Vehicle busVehicleLeader = busVehicles[0];
                    writer.WriteLine("Buses: " + busVehicleLeader.vehicle + " Times travelled: " + busVehicleLeader.amount + ".");
                }
            }
            else
                writer.WriteLine("No bus vehicles available.");

            if (vehicles.Count > 0)
            {
                List<Vehicle> tramVehicles = vehicles.Where(vehicle => vehicle.type == Types.Tram).ToList();
                if (tramVehicles.Count == 0)
                    writer.WriteLine("No tram vehicles available");
                else
                {
                    Vehicle tramVehicleLeader = tramVehicles[0];
                    writer.WriteLine("Trams: " + tramVehicleLeader.vehicle + " Times travelled: " + tramVehicleLeader.amount + ".");
                }
            }
            else
                writer.WriteLine("No tram vehicles available");

            if (vehicles.Count > 0)
            {
                List<Vehicle> trolleyVehicles = vehicles.Where(vehicle => vehicle.type == Types.Trolleybus).ToList();
                if (trolleyVehicles.Count == 0)
                    writer.WriteLine("No trolleybus vehicles available.");
                else
                {
                    Vehicle trolleyVehicleLeader = trolleyVehicles[0];
                    writer.WriteLine("Trolleybuses: " + trolleyVehicleLeader.vehicle + " Times travelled: " + trolleyVehicleLeader.amount + ".");
                }
            }
            else
                writer.WriteLine("No trolleybus vehicles available.");

            if (vehicles.Count > 0)
            {
                List<Vehicle> electroVehicles = vehicles.Where(vehicle => vehicle.type == Types.Electrobus).ToList();
                if (electroVehicles.Count == 0)
                    writer.WriteLine("No electrobus vehicles available.");
                else
                {
                    Vehicle electroVehicleLeader = electroVehicles[0];
                    writer.WriteLine("Electrobuses: " + electroVehicleLeader.vehicle + " Times travelled: " + electroVehicleLeader.amount + ".");
                }
            }
            else
                writer.WriteLine("No electrobus vehicles available.");

            writer.WriteLine(">>>Data about passing through Magistrate crossroads<<<");
            if (passingData[0] + passingData[1] > 0)
            {
                double passingPercentage = (double)((passingData[0]) * 100) / (double)(passingData[0] + passingData[1]);
                writer.WriteLine("Percentual chance to pass through the crossroads: " + passingPercentage + "%.");
            }
            else
                writer.WriteLine("Percentual chance to pass through the crossroads: N/A");
            writer.Close();
            stream.Close();
            Console.WriteLine("Report successfully generated.");
        }
    }
}
