using System;
using System.Collections.Generic;
using System.IO;

namespace MHDDatabase
{
    class Application
    {
        private string version = "v3.5.2";
        private List<Vehicle> historyVehicles;
        private List<Route> historyRoutes;
        private int[] historyPassingData;
        private string year = "";

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
            while (true)
            {
                Console.WriteLine("Welcome to data history mode. Please choose one of the following options: ");
                Console.WriteLine("1 - Load data from specific year.");
                Console.WriteLine("2 - List loaded data.");
                Console.WriteLine("3 - Save loaded data.");
                Console.WriteLine("4 - Quit data history mode.");
                Console.Write("Your choice: ");
                string entry = Console.ReadLine();
                char choice = entry[0];
                switch (choice)
                {
                    case '1':
                        loadSpecificFile(out historyVehicles, out historyRoutes, out historyPassingData);
                        break;
                    case '2':
                        listLoadedData(historyVehicles, historyRoutes, historyPassingData);
                        break;
                    case '3':
                        int intYear;
                        if (int.TryParse(year, out intYear))
                        {
                            DataSaver saver = new DataSaver(int.Parse(year));
                            saver.saveRoutes(historyRoutes);
                            saver.saveVehicles(historyVehicles);
                            saver.savePercentage(historyPassingData);
                            Console.WriteLine("Data from year " + this.year + " successfully loaded.");
                        }
                        else
                            Console.WriteLine("No history data to save.");
                        break;
                    case '4':
                        return;
                }
            }
        }

        private void loadSpecificFile(out List<Vehicle> vehicles, out List<Route> routes, out int[] passingData)
        {
            Console.WriteLine("Enter year from which you want to load data.");
            Console.Write("Year: ");
            string year = Console.ReadLine();
            this.year = year;
            string fileName = year + @"/raw" + year + ".txt";
            DataUploader uploader = new DataUploader();
            Queue queue = uploader.fileLoadingMode(fileName);
            queue.processQueue(out vehicles, out routes, out passingData);
            if (queue.failedEntries.Count > 0)
                fixFailedEntries(queue);
            Console.WriteLine("Data from year " + year + " successfully loaded.");
        }

        private void fixFailedEntries(Queue queue)
        {
            Console.WriteLine("Some of the entries were not processed.");
            Console.WriteLine("Most common problems are typos or missing types in the database.");
            Console.WriteLine("For each of the failed entries, please write a correct equivalent.");
            List<Entry> correctEntries = new List<Entry>();
            foreach (Entry failed in queue.failedEntries)
            {
                Console.WriteLine(failed.ToString());
                Console.Write("Correction: ");
                Entry correct = new Entry(Console.ReadLine().Split(' '));
                if (correct.arguments[0].ToLower().Equals("bugged") || correct.arguments[0].ToLower().Equals("bug"))
                    continue;
                else
                    correctEntries.Add(correct);
            }
            Queue newQueue = new Queue();
            newQueue.queuedEntries = correctEntries;
            List<Vehicle> historyFixedVehicles = new List<Vehicle>();
            List<Route> historyFixedRoutes = new List<Route>();
            int[] historyFixedPassingData = new int[2];
            newQueue.processQueue(out historyFixedVehicles, out historyFixedRoutes, out historyFixedPassingData);
            mergeVehicleLists(historyFixedVehicles);
            mergeRouteLists(historyFixedRoutes);
            mergeData(historyFixedPassingData);
        }

        private void mergeVehicleLists(List<Vehicle> fixedList)
        {
            foreach (Vehicle vehicle in fixedList)
            {
                if (historyVehicles.Exists(v => v.vehicle == vehicle.vehicle))
                    historyVehicles.Find(v => v.vehicle == vehicle.vehicle).amount += vehicle.amount;
                else
                    historyVehicles.Add(vehicle);
            }
        }

        private void mergeRouteLists(List<Route> fixedList)
        {
            foreach (Route route in fixedList)
            {
                if (historyRoutes.Exists(r => r.route == route.route))
                    historyRoutes.Find(r => r.route == route.route).amount += route.amount;
                else
                    historyRoutes.Add(route);
            }
        }

        private void mergeData(int[] fixedData)
        {
            historyPassingData[0] += fixedData[0];
            historyPassingData[1] += fixedData[1];
        }

        private void listLoadedData(List<Vehicle> vehicles, List<Route> routes, int[] passingData)
        {
            Listing listing = new Listing(routes, vehicles, passingData);
            listing.listRoutes();
            listing.listVehicles();
            listing.listData();
        }

        private void runListingMode()
        {
            bool listingModeQuitTrigger = false;
            DataLoader loader = new DataLoader();
            Listing listing;
            Console.WriteLine("You have chosen the listing mode. Please write the years from which you want data to be listed.");
            Console.WriteLine("For continual years span, use - symbol for the interval: 2012-2014");
            Console.WriteLine("Seperate more years using blank spaces or commas: 2013,2012 2015");
            string yearString = Console.ReadLine();
            string[] years = splitYearString(yearString);
            try
            {
                listing = new Listing(loader.loadRoutes(years),
                    loader.loadVehicles(years), 
                    loader.loadPercentage(years));

                while (!(listingModeQuitTrigger))
                {
                    Console.WriteLine("You have chosen the listing mode. Please choose one of the listing modes.");
                    Console.WriteLine("1 - Route listing mode.");
                    Console.WriteLine("2 - Vehicle listing mode.");
                    Console.WriteLine("3 - Passing data listing.");
                    Console.WriteLine("4 - Full listing mode.");
                    Console.WriteLine("5 - Filtered listing mode.");
                    Console.WriteLine("6 - Quit listing mode.");
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
                        case '5':
                            listing.filteredListing();
                            break;
                        default:
                            listingModeQuitTrigger = true;
                            break;
                    }
                }
                Console.WriteLine("Would you like to have a report generated?");
                string answer = Console.ReadLine();
                if (answer.ToLower().Equals("yes"))
                {
                    string currentYear = DateTime.Now.Year + "";
                    if (years.Length > 1)
                        listing.generateReport(chooseFinalYear(years));
                    else if (currentYear.Equals(years[0]))
                        listing.generateReport();
                    else
                        listing.generateReport(years[0]);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("One or more files associated with listing could not be found.");
                return;
            }
        }

        private string chooseFinalYear(string[] years)
        {
            string finalYear = years[0];
            for (int i = 1; i < years.Length; i++)
            {
                if (int.Parse(finalYear) < int.Parse(years[i]))
                    finalYear = years[i];
            }

            return finalYear;
        }

        private bool checkIfVitalFilesExist()
        {
            string[] files = new string[6];
            files[0] = "routesDatabase.txt";
            files[1] = "vehiclesDatabase.txt";
            files[2] = DateTime.Today.Year + @"\data" + DateTime.Today.Year + ".txt";
            files[3] = DateTime.Today.Year + @"\raw" + DateTime.Today.Year + ".txt";
            files[4] = DateTime.Today.Year + @"\routes" + DateTime.Today.Year + ".txt";
            files[5] = DateTime.Today.Year + @"\vehicles" + DateTime.Today.Year + ".txt";
            bool isFileMissing = false;
            List<int> missingIndices = new List<int>();
            for (int i = 0; i < files.Length; i++)
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
                    if (Directory.Exists(DateTime.Today.Year + "") == false)
                        Directory.CreateDirectory(DateTime.Today.Year + "");

                    foreach (int index in missingIndices)
                    {
                        FileStream stream = File.Create(files[index]);
                        stream.Close();
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }

        private string[] splitYearString(string year)
        {
            // Check if string is interval
            bool isInterval = (year.IndexOf('-') != -1);
            bool isSet = (year.IndexOf(' ') != -1) || (year.IndexOf(',') != -1);
            if (isInterval)
            {
                string[] fragments = year.Split('-');
                int lowerBoundary = int.Parse(fragments[0]);
                int upperBoundary = int.Parse(fragments[1]);
                string[] years = new string[upperBoundary - lowerBoundary + 1];
                for (int i = lowerBoundary; i <= upperBoundary; i++)
                    years[i - lowerBoundary] = lowerBoundary + "";

                return years;
            }
            // Check if string contains multiple years
            if (isSet)
                return year.Split(new char[] { '-', ' ' });

            // Otherwise its expected that just one year is the output
            return new string[] { year };
        }
    }
}
