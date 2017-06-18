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
        public List<Entry> queuedEntries { get; set; }
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
                string[] rest = entry.arguments.Skip(routeTypeIndex).ToArray();
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
                    continue;
                }
                saveProcessedEntry(vehicleParts, routeParts, rest);
            }

            updatedRoutes = routes;
            updatedVehicles = vehicles;
            updatedPassingData = passingData;
        }

        public void processQueue(out List<Vehicle> vehicles, out List<Route> routes, out int[] passingData)
        {
            vehicles = new List<Vehicle>();
            routes = new List<Route>();
            passingData = new int[2];
            foreach (Entry entry in queuedEntries)
            {
                List<Route> routesBackup = cloneRouteList(routes);
                List<Vehicle> vehiclesBackup = cloneVehicleList(vehicles);
                int[] passingDataBackup = new int[] { passingData[0], passingData[1] };

                int vehicleTypeIndex = detectVehicleTypeIndex(entry);
                int routeTypeIndex = detectRouteTypeIndex(entry, vehicleTypeIndex);
                string[] vehicleParts = entry.arguments.Take(vehicleTypeIndex).ToArray();
                string[] routeParts = entry.arguments.Skip(vehicleTypeIndex).Take(routeTypeIndex - vehicleTypeIndex).ToArray();
                string[] rest = entry.arguments.Skip(routeTypeIndex).ToArray();
                try
                {
                    processVehiclePart(vehicleParts, vehicles);
                    processRoutePart(routeParts, routes);
                    processRest(rest, passingData);
                }
                catch (EntryProccessingFailed)
                {
                    routes = routesBackup;
                    vehicles = vehiclesBackup;
                    passingData = passingDataBackup;
                    failedEntries.Add(entry);
                    continue;
                }
                saveProcessedEntry(vehicleParts, routeParts, rest);
            }
        }

        private int detectVehicleTypeIndex(Entry entry)
        {
            if (entry.arguments.Length == 0 || entry.arguments.Length == 1)
                return 0;
            string candidate = entry.arguments[1].ToLower();
            string[] types = new string[] { "bus", "tram", "trolleybus", "electrobus" };
            if (types.Contains(candidate.ToLower()))
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
                if (types.Contains(candidate.ToLower()))
                    return vehicleTypeIndex + 2;
                else
                    return vehicleTypeIndex + 1;
            }
            else
            {
                if (entry.arguments.Length == vehicleTypeIndex)
                    return vehicleTypeIndex;
                else
                    return vehicleTypeIndex + 1;
            }               
        }

        private void processVehiclePart(string[] vehicleParts, List<Vehicle> vehicles)
        {
            TypeDatabase vehicleDatabase = new TypeDatabase("vehiclesDatabase.txt");
            vehicleDatabase.loadDatabase();
            if (vehicleParts.Length == 0)
                throw new EntryProccessingFailed();
            if (vehicleParts.Length == 1)
            {
                if (vehicleDatabase.checkIfItemExistsInDatabase(vehicleParts[0]))
                {
                    if (vehicles.Exists(v => v.vehicle.Equals(vehicleParts[0])))
                    {
                        vehicles.Find(v => v.vehicle.Equals(vehicleParts[0])).amount++;
                    }
                    else
                    {
                        Vehicle vehicle = new Vehicle(vehicleParts[0], vehicleDatabase.getType(vehicleParts[0]), 1);
                        vehicles.Add(vehicle);
                    }
                }
                else
                    throw new EntryProccessingFailed();
            }
            else
            {
                if (vehicleDatabase.checkIfItemExistsInDatabase(vehicleParts[0]))
                {
                    if (vehicles.Exists(v => v.vehicle.Equals(vehicleParts[0])))
                    {
                        vehicles.Find(v => v.vehicle.Equals(vehicleParts[0])).amount++;
                    }
                    else
                    {
                        Vehicle vehicle = new Vehicle(vehicleParts[0], vehicleDatabase.getType(vehicleParts[0]), 1);
                        vehicles.Add(vehicle);
                    }
                }
                else
                {
                    vehicleDatabase.updateDatabase(new string[] { vehicleParts[0], vehicleParts[1] });
                    vehicleDatabase.loadDatabase();
                    Vehicle vehicle = new Vehicle(vehicleParts[0], vehicleDatabase.getType(vehicleParts[0]), 1);
                    vehicles.Add(vehicle);
                }
            }
        }

        private void processRoutePart(string[] routeParts, List<Route> routes)
        {
            TypeDatabase routeDatabase = new TypeDatabase("routesDatabase.txt");
            routeDatabase.loadDatabase();
            if (routeParts.Length == 0)
                throw new EntryProccessingFailed();
            if (routeParts.Length == 1)
            {
                if (routeDatabase.checkIfItemExistsInDatabase(routeParts[0]))
                {
                    if (routes.Exists(r => r.route.Equals(routeParts[0])))
                    {
                        routes.Find(r => r.route.Equals(routeParts[0])).amount++;
                    }
                    else
                    {
                        Route newRoute = new Route(routeParts[0], routeDatabase.getType(routeParts[0]), 1);
                        routes.Add(newRoute);
                    }
                }
                else
                    throw new EntryProccessingFailed();
            }
            else
            {
                if (routeDatabase.checkIfItemExistsInDatabase(routeParts[0]))
                {
                    if (routes.Exists(r => r.route.Equals(routeParts[0])))
                    {
                        routes.Find(r => r.route.Equals(routeParts[0])).amount++;
                    }
                    else
                    {
                        Route newRoute = new Route(routeParts[0], routeDatabase.getType(routeParts[0]), 1);
                        routes.Add(newRoute);
                    }
                }
                else
                {
                    routeDatabase.updateDatabase(new string[] { routeParts[0], routeParts[1] });
                    routeDatabase.loadDatabase();
                    Route newRoute = new Route(routeParts[0], routeDatabase.getType(routeParts[0]), 1);
                    routes.Add(newRoute);
                }
            }
        }

        private void processRest(string[] rest, int[] passingData)
        {
            if (rest.Length > 0)
            {
                if (rest[0].Equals("P"))
                    passingData[0]++;
                else if (rest[0].Equals("N"))
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
            if (rest.Length != 0)
                processedEntries.Add(vehicleParts[0] + " " + routeParts[0] + " " + rest[0]);
            else
                processedEntries.Add(vehicleParts[0] + " " + routeParts[0]);
        }
    }
}
