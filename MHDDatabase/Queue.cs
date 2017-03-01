using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHDDatabase
{
    class Queue
    {
        class EntryProccessingFailed : Exception{ }
        public List<Entry> queuedEntries { get; private set; }
        public List<Entry> failedEntries { get; private set; }
        public List<string> processedEntries { get; private set; }

        public Queue()
        {
            queuedEntries = new List<Entry>();
            failedEntries = new List<Entry>();
            processedEntries = new List<string>();
        }

        public void processQueue(int year, out List<Route> updatedRoutes, out List<Vehicle> updatedVehicles, out int[] updatedPassingData)
        {
            // Possible types of entries
            // 3320 19 - vehicle + route - type needs to be already in the DB
            // 530 9 P - tram + route + flag - type needs to be already in the DB
            // 4716 36 Bus - vehicle + route + type
            // 538 6 P Tram - tram + route + flag + type
            DataLoader loader = new DataLoader();
            List<Route> routes = loader.loadRoutes("routes" + year + ".txt");
            List<Vehicle> vehicles = loader.loadVehicles("vehicles" + year + ".txt");
            int[] passingData = loader.loadPercentage("data" + year + ".txt");
            foreach (Entry entry in queuedEntries)
            {
                List<Route> routesBackup = cloneRouteList(routes);
                List<Vehicle> vehiclesBackup = cloneVehicleList(vehicles);
                int[] passingDataBackup = new int[] { passingData[0], passingData[1] };

                int vehicleTypeIndex = detectVehicleTypeIndex(entry);
                int routeTypeIndex = detectRouteTypeIndex(entry, vehicleTypeIndex);
                string[] vehicleParts = entry.arguments.Take(vehicleTypeIndex).ToArray();
                string[] routeParts = entry.arguments.Skip(vehicleTypeIndex).Take(routeTypeIndex - vehicleTypeIndex).ToArray();
                string[] rest = entry.arguments.Skip(vehicleTypeIndex + routeTypeIndex).ToArray();
                try
                {
                    processVehiclePart(vehicleParts, vehicles);
                    processRoutePart(routeParts, routes);
                    processRest(rest, passingData);
                }
                catch(EntryProccessingFailed)
                {
                    routes = routesBackup;
                    vehicles = vehiclesBackup;
                    passingData = passingDataBackup;
                    failedEntries.Add(entry);
                }
                saveProcessedEntry(vehicleParts, routeParts, rest);
            }

            updatedRoutes = routes;
            updatedVehicles = vehicles;
            updatedPassingData = passingData;
        }

        private int detectVehicleTypeIndex(Entry entry)
        {
            string candidate = entry.arguments[1].ToLower();
            string[] types = new string[] { "bus", "tram", "trolleybus", "electrobus" };
            if (types.Contains(candidate))
                return 2;
            else
                return 1;
        }

        private int detectRouteTypeIndex(Entry entry, int vehicleTypeIndex)
        {
            if (entry.arguments.Length > vehicleTypeIndex + 1)
            {
                string candidate = entry.arguments[vehicleTypeIndex + 1];
                string[] types = new string[] { "bus", "tram", "trolleybus", "electrobus" };
                if (types.Contains(candidate))
                    return vehicleTypeIndex + 1;
                else
                    return vehicleTypeIndex;
            }
            else
                return vehicleTypeIndex;
        }

        private void processVehiclePart(string[] vehicleParts, List<Vehicle> vehicles)
        {
            if (vehicleParts.Length == 1)
            {
                if (vehicles.Exists(v => v.vehicle.Equals(vehicleParts[0])))
                    vehicles.Find(v => v.vehicle.Equals(vehicleParts[0])).amount++;
                else
                    throw new EntryProccessingFailed();
            }
            else
            {
                TypeDatabase vehicleDatabase = new TypeDatabase("vehiclesDatabase.txt");
                vehicleDatabase.updateDatabase(new string[] { vehicleParts[0], vehicleParts[1] });
                vehicleDatabase.loadDatabase();
                Vehicle vehicle = new Vehicle(vehicleParts[0], vehicleDatabase.getType(vehicleParts[0]));
                vehicles.Add(vehicle);
            }
        }

        private void processRoutePart(string[] routeParts, List<Route> routes)
        {
            if (routeParts.Length == 1)
            {
                if (routes.Exists(r => r.route.Equals(routeParts[0])))
                    routes.Find(r => r.route.Equals(routeParts[0])).amount++;
                else
                    throw new EntryProccessingFailed();
            }
            else
            {
                TypeDatabase routeDatabase = new TypeDatabase("routesDatabase.txt");
                routeDatabase.updateDatabase(new string[] { routeParts[0], routeParts[1] });
                Route newRoute = new Route(routeParts[0], routeDatabase.getType(routeParts[1]));
                routes.Add(newRoute);
            }
        }

        private void processRest(string[] rest, int[] passingData)
        {
            if (rest.Length > 0)
            {
                if (rest[0].Equals("P"))
                    passingData[0]++;
                else
                    passingData[1]++;
            }
        }

        private List<Vehicle> cloneVehicleList(List<Vehicle> pattern)
        {
            List<Vehicle> clone = new List<Vehicle>();
            foreach (Vehicle vehicle in pattern)
                clone.Add(new Vehicle(vehicle.vehicle, vehicle.type, vehicle.amount));

            return clone;
        }

        private List<Route> cloneRouteList(List<Route> pattern)
        {
            List<Route> clone = new List<Route>();
            foreach (Route route in pattern)
                clone.Add(new Route(route.route, route.type, route.amount));

            return clone;
        }

        private void saveProcessedEntry(string[] vehicleParts, string[] routeParts, string[] rest)
        {
            if (rest[0].Equals("") == false)
                processedEntries.Add(vehicleParts[0] + " " + routeParts[0] + rest);
            else
                processedEntries.Add(vehicleParts[0] + " " + routeParts[0]);
        }
    }
}
