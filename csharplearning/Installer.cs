using System.IO;
using System;
using Microsoft.Deployment.Compression.Cab;
using Newtonsoft.Json;
using Spectre.Console;

class Util {
    public string fileReader(string file) {
        StreamReader sr = new StreamReader(file);
        sr.BaseStream.Seek(0, SeekOrigin.Begin);
        string str = sr.ReadToEnd();
        sr.Close();
        return str;
    }

    public string GetHostArch() {
        return Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
    }
    public static void CreateFolder(string folderPath, string folderName) {
        if (!Directory.Exists(folderPath + folderName)) {
            Directory.CreateDirectory(folderPath + folderName);
        }
    }
    static void elevator() {

    }
}

class Package {
    // Public access function, can be used in another class/function
    public static void Unpack(string cabFile, string destDir) {
        // Creating the CabInfo object
        CabInfo cab = new CabInfo(cabFile);
        // Unpacking the cab file to the destination directory
        cab.Unpack(destDir);
    }
    public static void Pack(string sourceDir, string destDir) {
        CabInfo cab = new CabInfo(destDir);
        // Packing files in the source directory with normal compression into the cab file
        cab.Pack(sourceDir, true, Microsoft.Deployment.Compression.CompressionLevel.Normal, null);
    }

    // This is probably going to get me publicly executed by Mr. bill gates for how just messy this function is
    public static string getPkgInformation(string json, 
        bool getPkgName, bool getPkgVersion,
        bool getPkgDesc, bool getAuthorElements) 
    {
        Util util = new Util();
        string pkgInfo = util.fileReader(json);
        dynamic pkg = JsonConvert.DeserializeObject(pkgInfo);
        if (getPkgName) { return pkg["name"];}
        if (getPkgVersion) { return pkg["version"]; }
        if (getPkgDesc) { return pkg["description"]; }
        if (getAuthorElements) { return pkg["author"]["name"] + " " + pkg["author"]["email"]; }
        return "No information found.";
    }

    // This function is used to check if the package is compatible with the host architecture
    public static bool returnPackageArch(string json) {
        Util util = new Util();
        string pkgInfo = util.fileReader(json);
        dynamic pkg = JsonConvert.DeserializeObject(pkgInfo);
        string pkgArch = pkg["arch"];
        if(pkgArch != util.GetHostArch() && pkgArch != "Any") {
            Console.WriteLine("The package is not compatible with the host architecture.");
            return false;
        }
        if(pkgArch != util.GetHostArch() && pkgArch == "Any") {
            return true;
        }
        return true;
    }
}

class App {
    static int Main(String[] args) {

        // Creating a util object so we can use both static and non static members of the class
        Util util = new Util();

        // exiting if you didn't provide an argument
        if (args.Length == 0) {
            Console.WriteLine("Installer, A simple package installer written in C#\n");
            Console.WriteLine("Usage: Installer.exe [Option] <File> ");
            Console.WriteLine("Options: ");
            Console.WriteLine("  -i, --install <file>  Install a package");
            Console.WriteLine("  -h, --help  Display this help message");
            return 1;
        }

        // Checking if you have more than 0 arguments
        if (args.Length > 0) {
            // Checking if the first argument is -i or --install
            if (args[0] == "-i" || args[0] == "--install") {
                // Checking if the second argument is not empty
                if (args[1] != "") {
                    // Checking if the file exists
                    if (File.Exists(args[1])) {
                        // Checking if the package is compatible with the host architecture
                        if (Package.returnPackageArch(args[1])) {
                            // Unpacking the package
                            Package.Unpack(args[1], "C:\\Windows\\Temp\\InstallerCache");
                            // Displaying the package information
                            Console.WriteLine("Package Information: ");
                            Console.WriteLine("Name: " + Package.getPkgInformation("C:\\Program Files\\package.json", true, false, false, false));
                            Console.WriteLine("Version: " + Package.getPkgInformation("C:\\Program Files\\package.json", false, true, false, false));
                            Console.WriteLine("Description: " + Package.getPkgInformation("C:\\Program Files\\package.json", false, false, true, false));
                            Console.WriteLine("Author: " + Package.getPkgInformation("C:\\Program Files\\package.json", false, false, false, true));
                            return 0;
                        }
                    }
                    else {
                        Console.WriteLine("The file does not exist.");
                        return 1;
                    }
                }
                else {
                    Console.WriteLine("Please provide a file to install.");
                    return 1;
                }
            }
            // Checking if the first argument is -h or --help
            if (args[0] == "-h" || args[0] == "--help"){
                Console.WriteLine("Installer, A simple package installer written in C#\n");
                Console.WriteLine("Usage: Installer.exe [Option] <File> ");
                Console.WriteLine("Options: ");
                Console.WriteLine("  -i, --install <file>  Install a package");
                Console.WriteLine("  -h, --help  Display this help message");
                return 0;
            }
        }
        return 0;
    }
} // miku miku you can call me miku 
  // blue hair, blue tie, hiding in your wifi
  // open secrets, anyone can find me
  // hear your music running through my mind
  // I'm thinking miku miku oo ee oo
  // I'm thinking miku miku oo ee oo
  // I'm thinking miku miku oo ee oo
  // I'm thinking miku miku oo ee oo
  // I'm on top of the world because of you
  // all i wanted to do is follow you
  // I'll keep singing along to all of you
  // I'll keep singing along
  // I'm thinking miku miku oo ee oo
  // Thank you github copilot for the lyrics (how the fuck did it actually get it like actually decently correct)