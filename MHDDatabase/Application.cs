using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MHDDatabase
{
    class Application
    {
        string version = "v1.3.3";
        public void run()
        {
            bool applicationQuitTrigger = false;
            while (!(applicationQuitTrigger))
            {
                Console.WriteLine("Welcome to MHD Database " + version);
                Console.WriteLine("To proceed using the application, please choose one of the available options:");
                Console.WriteLine("1 - Data entering mode.");
                Console.WriteLine("2 - Listing mode.");
                Console.WriteLine("3 - Developer mode.");
                Console.WriteLine("4 - Close application.");
                Console.Write("Your choice: ");
                string entry = Console.ReadLine();
                char choice = entry[0];
                switch (choice)
                {
                    case '1':
                        runEnteringMode();
                        break;
                    case '2':
                        runListingMode();
                        break;
                    case '3':
                        runDeveloperMode();
                        break;
                    default:
                        applicationQuitTrigger = true;
                        break;     
                }
            }
        }

        private void runEnteringMode()
        {
            while (true)
            {
                Console.WriteLine("You have chosen the entering mode. Please choose one of the available options:");
                Console.WriteLine("1 - Manual data entering mode.");
                Console.WriteLine("2 - Loading data from file.");
                Console.WriteLine("3 - Quit entering mode.");
                Console.Write("Your choice: ");
                string entry = Console.ReadLine();
                char choice = entry[0];
                switch (choice)
                {
                    case '1':
                        runManualEntering();
                        break;
                    case '2':
                        runFileLoading();
                        break;
                    case '3':
                        return;
                }
            }

        }

        private void runManualEntering()
        {
            Console.WriteLine("You have chosen the manual entering mode. Please type data you want to add to the database.");
            Console.WriteLine("Data needs to be given in the following format: ");
            Console.WriteLine("Vehicle [Type] Route [Type] [Flag]");
            Console.WriteLine("Arguments in square brackets are optional.");
            Console.WriteLine("To quit entering mode, type terminate on a new line.");
            DataUploader uploader = new DataUploader();
            Queue queue = uploader.enteringMode();
            List<Route> routes;
            List<Vehicle> vehicles;
            int[] passingData;
            try
            {
                queue.processQueue(DateTime.Now.Year, out routes, out vehicles, out passingData);
                DataSaver saver = new DataSaver(DateTime.Now.Year);
                saver.saveRoutes(routes);
                saver.saveVehicles(vehicles);
                saver.savePercentage(passingData);
                saver.saveProcessedEntries(queue.processedEntries);
                Console.WriteLine(queue.processedEntries.Count + " entries successfully processed.");
                if (queue.failedEntries.Count > 0)
                {
                    Console.WriteLine("Some of the data you entered didn't match format or there was some data missing in the reference database.");
                    foreach (Entry entry in queue.failedEntries)
                        Console.WriteLine(entry);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("One or more files associated with saving data was not found.");
            }
        }

        private void runFileLoading()
        {
            throw new NotImplementedException();
        }

        private void runListingMode()
        {
            bool listingModeQuitTrigger = false;
            DataLoader loader = new DataLoader();
            Listing listing;
            Console.WriteLine("You have chosen the listing mode. Please write the year from which you want data to be listed:");
            string year = Console.ReadLine();
            try
            {
                listing = new Listing(loader.loadRoutes("routes" + year + ".txt"), loader.loadVehicles("vehicles" + year +".txt"),
                    loader.loadPercentage("data" + year + ".txt"));

                while (!(listingModeQuitTrigger))
                {
                    Console.WriteLine("You have chosen the listing mode. Please choose one of the listing modes.");
                    Console.WriteLine("1 - Route listing mode.");
                    Console.WriteLine("2 - Vehicle listing mode.");
                    Console.WriteLine("3 - Passing data listing.");
                    Console.WriteLine("4 - Full listing mode.");
                    Console.WriteLine("5 - Quit listing mode.");
                    Console.Write("Your choice: ");
                    string entry = Console.ReadLine();
                    char choice = entry[0];
                    switch (choice)
                    {
                        case '1':
                            listing.listRoutes();
                            break;
                        case '2':
                            listing.listVehicles();
                            break;
                        case '3':
                            listing.listData();
                            break;
                        case '4':
                            listing.listRoutes();
                            listing.listVehicles();
                            listing.listData();
                            break;
                        default:
                            listingModeQuitTrigger = true;
                            break;
                    }
                }
                Console.WriteLine("Would you like to have a report generated?");
                string answer = Console.ReadLine();
                if (answer.ToLower().Equals("yes"))
                    listing.generateReport();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("One or more files associated with listing could not be found.");
                return;
            }
        }

        private void runDeveloperMode()
        {
            string password = "";
            int counter = 3;
            do
            {
                Console.WriteLine("Please enter password:");
                password = Console.ReadLine();
                if (password.ToLower().Equals("leave"))
                    return;
                if (password.Equals("magrum erupto"))
                    break;
                Console.WriteLine("Invalid password entered. You have " + --counter + " attempts remaining.");
                if (counter == 0)
                    return;
            } while (password.Equals("magrum erupto") == false);

            while (true)
            {
                Console.WriteLine("Welcome to MHDDatabase " + version + " developer tools.");
                Console.WriteLine("Please choose one of the available options.");
                Console.WriteLine("1 - Check internal database.");
                Console.WriteLine("2 - Delete entries from internal database(WARNING: Changes are irreversible!)");
                Console.WriteLine("3 - Wipe all database data.");
                Console.WriteLine("4 - Close developer tools.");
                Console.Write("Your choice: ");
                string entry = Console.ReadLine();
                char choice = entry[0];
                switch (choice)
                {
                    case '1':
                        checkInternalDatabase();
                        break;
                    case '2':
                        deleteFromInternalDatabase();
                        break;
                    case '3':
                        wipeInternalDatabase();
                        break;
                    default:
                        return;
                }
            }
        }

        private void checkInternalDatabase()
        {
            Console.WriteLine("1 - Lists the routes part of internal database.");
            Console.WriteLine("2 - Lists the vehicles part of internal database.");
            Console.WriteLine("3 - Checks if given route or vehicle is defined in database.");
            Console.WriteLine("4 - Close this mode.");
            Console.Write("Your choice: ");
            string entry = Console.ReadLine();
            char choice = entry[0];
            try
            {
                switch (choice)
                {
                    case '1':
                        TypeDatabase routesDatabase = new TypeDatabase("routesDatabase.txt");
                        routesDatabase.loadDatabase();
                        routesDatabase.listDatabase();
                        break;
                    case '2':
                        TypeDatabase vehiclesDatabase = new TypeDatabase("vehiclesDatabase.txt");
                        vehiclesDatabase.loadDatabase();
                        vehiclesDatabase.listDatabase();
                        break;
                    case '3':
                        Console.WriteLine("Enter route or vehicle number: ");
                        string evidence = Console.ReadLine();
                        identifyAndCheckEntry(evidence);
                        break;
                    default:
                        return;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("One or more files associated with listing could not be found.");
                return;
            }
        }

        private void identifyAndCheckEntry(string entry)
        {
            string type = identifyEntry(entry);
            TypeDatabase database;
            if (type.Equals("route"))
                database = new TypeDatabase("routesDatabase.txt");
            else
                database = new TypeDatabase("vehiclesDatabase.txt");
            try
            {
                database.loadDatabase();
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("One or more files associated with listing could not be found.");
                return;
            }
            try
            {
                Types entryType = database.getType(entry);
                Console.WriteLine("Given entry is listed in database as: " + entryType + ".");
            }
            catch (TypeDatabase.DatabaseError)
            {
                Console.WriteLine("Given entry is not listed in the database.");
            }
        }

        private string identifyEntry(string entry)
        {
            string entryPart;
            if (entry.Contains("+"))
                entryPart = entry.Split('+')[0];
            else
                entryPart = entry;
            int parseInt;
            if (int.TryParse(entry, out parseInt))
                if (parseInt > 100)
                    return "vehicle";

            return "route";
        }

        private void deleteFromInternalDatabase()
        {
            Console.Write("Write the route or vehicle you want to delete from the database: ");
            string entry = Console.ReadLine();
            string type = identifyEntry(entry);
            TypeDatabase database;
            if (type.Equals("route"))
                database = new TypeDatabase("routesDatabase.txt");
            else
                database = new TypeDatabase("vehiclesDatabase.txt");
            try
            {
                database.loadDatabase();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("One or more files associated with listing could not be found.");
                return;
            }
            string[] args = new string[2];
            args[0] = entry;
            args[1] = database.getType(entry).ToString();
            database.deleteFromDatabase(args);
        }

        private void wipeInternalDatabase()
        {
            Console.WriteLine("You have chosen to delete all current data from database and listing files.");
            Console.WriteLine("Do you want to proceed? yes/no");
            string line = Console.ReadLine();
            if (line.ToLower() == "yes")
            {
                wipeFile("routesDatabase.txt");
                wipeFile("vehiclesDatabase.txt");
                wipeFile("raw" + DateTime.Today.Year + ".txt");
                wipeFile("vehicles" + DateTime.Today.Year + ".txt");
                wipeFile("routes" + DateTime.Today.Year + ".txt");
                wipeFile("data" + DateTime.Today.Year + ".txt");
                Console.WriteLine("All database and listing files are now clean.");
            }
        }
        private void wipeFile(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Truncate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.Close();
            stream.Close();
        }
    }
}
