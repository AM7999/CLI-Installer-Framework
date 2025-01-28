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
    static void CreateFolder(string folderPath, string folderName) {
        if (!Directory.Exists(folderPath + folderName)) {
            Directory.CreateDirectory(folderPath + folderName);
        }
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
            Console.WriteLine("Please run this with the included batch file.");
            return 1;
        }

        // Checking if you have more than 0 arguments
        if (args.Length > 0) {
            // checking the first arg
            if (args[0] == "--package" || args[0] == "-p") {
                Console.Write(util.GetHostArch());
            }


            // checking if the first arg is -pc or --package-config to see 
            if (args[0] == "--package-config" || args[0] == "-pc") {
                if(args.Length < 2) {
                    Console.WriteLine("Please provide a package configuration file.");
                    return 1;
                } else if (args.Length == 2) {
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Triangle)
                        .Start("Compressing...", ctx => {
                            Package.Pack("A:\\TestApplication\\", "A:\\disk0.cab");
                        });
                    Console.WriteLine("Finished Packing App" /*add pkg name eventually*/ );
                }
            }

            
            if (args[0] == "-i" || args[0] == "--install") {
                if (args.Length < 2) {
                    Console.WriteLine("Please provide a package file.");
                    return 1;
                }
                else if (args.Length == 2) {
                    bool compatible = Package.returnPackageArch(args[1]);
                    if(!compatible){
                        return 1;
                    } else if(compatible) {
                        Package.Unpack(args[1], "A:\\TestInstallPath\\");
                    }
                }
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