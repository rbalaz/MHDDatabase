using System;
using System.IO;

namespace MHDDatabase
{
    class DeveloperMode
    {
        private string version;

        public DeveloperMode(string version)
        {
            this.version = version;
        }

        public void runDeveloperMode()
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
            while (true)
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
            catch (FileNotFoundException)
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
            Console.WriteLine(entry + " successfully deleted from database.");
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
