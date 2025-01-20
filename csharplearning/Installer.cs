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
        // Packing files in the source directory with max compression into the cab file
        cab.Pack(sourceDir, true, Microsoft.Deployment.Compression.CompressionLevel.Normal, null);
    }

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
            if(args[0] == "--package-config" || args[0] == "-pc") {
                if(args.Length < 2) {
                    Console.WriteLine("Please provide a package configuration file.");
                    return 1;
                } else if (args.Length == 2) {
                    testMarker:
                    Package.Pack("A:\\TestApplication\\", "A:\\disk0.cab");
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
}