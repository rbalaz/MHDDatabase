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
            this.routes = routes;
            this.vehicles = vehicles;
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
            Console.WriteLine("Chance to pass: " + passingData[0] * 100 / (passingData[0] + passingData[1]) + "%");
        }

        public void generateReport()
        {
            string currentDate = DateTime.Today.Day + "." + DateTime.Today.Month + "." + DateTime.Today.Year;
            string filename = DateTime.Today.Day + "_" + DateTime.Today.Month + "_" + DateTime.Today.Year + ".txt";
            FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("Report from MHDDatabase on " + currentDate + ".");
            writer.WriteLine(">>>Leaders in routes sector<<<");
            Route busRouteLeader = routes.Where(route => route.type == Types.Bus).ToList()[0];
            writer.WriteLine("Buses: " + busRouteLeader.route + " Times travelled: " + busRouteLeader.amount + ".");
            Route tramRouteLeader = routes.Where(route => route.type == Types.Tram).ToList()[0];
            writer.WriteLine("Trams: " + tramRouteLeader.route + " Times travelled: " + tramRouteLeader.amount + ".");
            Route trolleyRouteLeader = routes.Where(route => route.type == Types.Trolleybus).ToList()[0];
            writer.WriteLine("Trolleybuses: " + trolleyRouteLeader.route + " Times travelled: " + trolleyRouteLeader.amount + ".");
            writer.WriteLine(">>>Leaders in vehicles sector<<<");
            Vehicle busVehicleLeader = vehicles.Where(vehicle => vehicle.type == Types.Bus).ToList()[0];
            writer.WriteLine("Buses: " + busVehicleLeader.vehicle + " Times travelled: " + busVehicleLeader.amount + ".");
            Vehicle tramVehicleLeader = vehicles.Where(vehicle => vehicle.type == Types.Tram).ToList()[0];
            writer.WriteLine("Trams: " + tramVehicleLeader.vehicle + " Times travelled: " + tramVehicleLeader.amount + ".");
            Vehicle trolleyVehicleLeader = vehicles.Where(vehicle => vehicle.type == Types.Trolleybus).ToList()[0];
            writer.WriteLine("Trolleybuses: " + trolleyVehicleLeader.vehicle + " Times travelled: " + trolleyVehicleLeader.amount + ".");
            writer.WriteLine(">>>Data about passing through Magistrate crossroads<<<");
            double passingPercentage = (double)((passingData[0]) * 100) / (double)(passingData[0] + passingData[1]);
            writer.WriteLine("Percentual chance to pass through the crossroads: " + passingPercentage + "%.");
            writer.Close();
            stream.Close();
            Console.WriteLine("Report successfully generated.");
        }
    }
}
