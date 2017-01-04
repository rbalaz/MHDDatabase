using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHDDatabase
{
    class Application
    {
        public void run()
        {
            bool applicationQuitTrigger = false;
            while (!(applicationQuitTrigger))
            {
                Console.WriteLine("Welcome to MHD Database v1.0.");
                Console.WriteLine("To proceed using the application, please choose one of the available options:");
                Console.WriteLine("1 - Data entering mode.");
                Console.WriteLine("2 - Listing mode.");
                Console.WriteLine("3 - Close application.");
                Console.Write("Your choice: ");
                char choice = (char)Console.Read();
                switch (choice)
                {
                    case '1':
                        runEnteringMode();
                        break;
                    case '2':
                        runListingMode();
                        break;
                    default:
                        applicationQuitTrigger = true;
                        break;     
                }
            }
        }

        private void runEnteringMode()
        {
            Console.WriteLine("You have chosen the entering mode. Please type data you want to add to the database.");
            Console.WriteLine("Data needs to be given in the following format: ");
            Console.WriteLine("Vehicle [Type] Route [Type] [Flag]");
            Console.WriteLine("Arguments in square brackets are optional.");
            Console.WriteLine("To quit entering mode, type terminate on a new line.");
            DataUploader uploader = new DataUploader();
            Queue queue = uploader.enteringMode();
            List<Route> routes;
            List<Vehicle> vehicles;
            int[] passingData;
            queue.processQueue(out routes, out vehicles, out passingData);
            DataSaver saver = new DataSaver();
            saver.saveRoutes(routes);
            saver.saveVehicles(vehicles);
            saver.savePercentage(passingData);
            if (queue.failedEntries.Count > 0)
            {
                Console.WriteLine("Some of the data you entered didn't match format or there was some data missing in the reference database");
                foreach (Entry entry in queue.failedEntries)
                    Console.WriteLine(entry);
            }
        }

        private void runListingMode()
        {
            bool listingModeQuitTrigger = false;
            DataLoader loader = new DataLoader();
            Listing listing = new Listing(loader.loadRoutes("routes2016.txt"), loader.loadVehicles("vehicles2016.txt"), 
                loader.loadPercentage("data2016.txt"));
            while (!(listingModeQuitTrigger))
            {
                Console.WriteLine("You have chosen the listing mode. Please choose one of the listing modes.");
                Console.WriteLine("1 - Route listing mode.");
                Console.WriteLine("2 - Vehicle listing mode.");
                Console.WriteLine("3 - Passing data listing.");
                Console.WriteLine("4 - Full listing mode.");
                Console.WriteLine("5 - Quit listing mode.");
                Console.Write("Your choice: ");
                char choice = (char)Console.Read();
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
    }
}
