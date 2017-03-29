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
        string version = "v1.3.4";
        public void run()
        {
            bool applicationQuitTrigger = false;
            if (checkIfVitalFilesExist() == false)
                applicationQuitTrigger = true;
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
                        (new DeveloperMode(version)).runDeveloperMode();
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



        private bool checkIfVitalFilesExist()
        {
            string[] files = new string[6];
            files[0] = "routesDatabase.txt";
            files[1] = "vehiclesDatabase.txt";
            files[2] = "data" + DateTime.Today.Year + ".txt";
            files[3] = "raw" + DateTime.Today.Year + ".txt";
            files[4] = "routes" + DateTime.Today.Year + ".txt";
            files[5] = "vehicles" + DateTime.Today.Year + ".txt";
            bool isFileMissing = false;
            List<int> missingIndices = new List<int>();
            for(int i = 0; i < files.Length; i++)
            {
                if (File.Exists(files[i]) == false)
                {
                    isFileMissing = true;
                    missingIndices.Add(i);
                }
            }
            if (isFileMissing)
            {
                Console.WriteLine("Some of the depending files for the application are missing. Would you like to create them?");
                Console.Write("Your choice(yes/no)? ");
                string choice = Console.ReadLine();
                if (choice.ToLower().Equals("yes"))
                {
                    foreach (int index in missingIndices)
                    {
                        File.Create(files[index]);
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }
    }
}
