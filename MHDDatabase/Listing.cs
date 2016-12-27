using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
