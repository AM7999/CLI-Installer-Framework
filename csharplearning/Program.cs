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
    public string getHostArch() {
        return Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
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
        cab.Pack(sourceDir, true, Microsoft.Deployment.Compression.CompressionLevel.Max, null);
    }

    public static void parsePackageInfo(string json) {
        Util util = new Util();
        string pkgInfo = util.fileReader(json);
        dynamic pkg = JsonConvert.DeserializeObject(pkgInfo);
        string pkgName = pkg["name"];
        string pkgVersion = pkg["version"];
        string pkgDescription = pkg["description"];
        string pkgArch = pkg["arch"];
        if(pkgArch != util.getHostArch()) {
            Console.WriteLine("The package is not compatible with the host architecture.");
            return;
        }

    }
}

class App {
    static void Main(String[] args) {

        Util util = new Util();

        if (args.Length == 0) {
            Console.WriteLine("Please run this with the included batch file.");
        }
        if (args.Length > 0) {
            if (args[0] == "--package" || args[0] == "-p") {
                Console.Write(util.getHostArch());

            } else if(args[0] == "--package-config" || args[0] == "-pc") {
                if(args.Length < 2) {
                    Console.WriteLine("Please provide a package configuration file.");
                } else if (args.Length == 2) {
                    //Console.WriteLine(args[0] + " " + args[1]);
                }
            }
        }

        //Panel panel = new Panel("");
        //panel.Header = new PanelHeader("HI");
        //panel.Border = BoxBorder.Rounded;
        //panel.Padding = new Padding(5, 5, 5, 5);
        //panel.Expand = true;
        //AnsiConsole.Write(panel);



    }
}