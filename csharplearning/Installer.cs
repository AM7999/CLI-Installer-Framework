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
        string arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
        return arch;
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
        // Might go for a different approach for packing the cab file, MakeCab seems to be a better (easier) option since i can just query through each file in a dir then add it to the MakeCab ddf
        cab.Pack(sourceDir, true, Microsoft.Deployment.Compression.CompressionLevel.Normal, null);
    }

    // This is probably going to get me publicly executed by Mr. bill gates for how just messy this function is
    public static string getPkgInformation(string json, 
        bool getPkgName, bool getPkgVersion,
        bool getPkgDesc, bool getAuthorElements,
        bool checkLic)
    {
        Util util = new Util();
        string pkgInfo = util.fileReader(json);
        dynamic pkg = JsonConvert.DeserializeObject(pkgInfo);
        if (getPkgName) { return pkg["name"];}
        if (getPkgVersion) { return pkg["version"]; }
        if (getPkgDesc) { return pkg["description"]; }
        if (getAuthorElements) { return pkg["author"]["name"] + " " + pkg["author"]["email"]; }
        if (checkLic) { return pkg["license"]; }
        return "No information found.";
    }

    // This function is used to check if the package is compatible with the host architecture
    public static bool returnPackageArch(string json) {
        Util util = new Util();
        string pkgInfo = util.fileReader(json);
        dynamic pkg = JsonConvert.DeserializeObject(pkgInfo);
        string pkgArch = pkg["arch"];
        string expectedArch = util.GetHostArch();
        // How does my code get any worse than this
        if(pkgArch != expectedArch.ToLower() && pkgArch != "Any") {
            AnsiConsole.MarkupLine("[bold red]The package is not compatible with the host architecture.[/]");
            return false;
        }
        if(pkgArch != expectedArch.ToLower() && pkgArch == "Any") {
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
            Console.WriteLine("  -p, --pack <dir>  Pack a directory into a package");
            Console.WriteLine("  -pc --package-config <config_name> ");
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
                        // Unpacking the package
                        AnsiConsole.Status()
                            .Spinner(Spinner.Known.Material)
                            .Start("Unpacking to Temporary Directory...", ctx => {
                                Package.Unpack(args[1], "C:\\Windows\\Temp\\InstallerCache");
                            });

                        // checking the pkg architecture
                        bool pkgArch = Package.returnPackageArch("C:\\Windows\\Temp\\InstallerCache\\packageManifest.json");
                        if (!pkgArch) {
                            return 1;
                        }

                        // Grabbing the package information
                        string pkgName = Package.getPkgInformation("C:\\Windows\\Temp\\InstallerCache\\packageManifest.json", true, false, false, false, false);
                        string pkgVersion = Package.getPkgInformation("C:\\Windows\\Temp\\InstallerCache\\packageManifest.json", false, true, false, false, false);
                        string pkgDesc = Package.getPkgInformation("C:\\Windows\\Temp\\InstallerCache\\packageManifest.json", false, false, true, false, false);
                        string pkgAuthor = Package.getPkgInformation("C:\\Windows\\Temp\\InstallerCache\\packageManifest.json", false, false, false, true, false);
                        string pkgLic = Package.getPkgInformation("C:\\Windows\\Temp\\InstallerCache\\packageManifest.json", false, false, false, false, true);

                        AnsiConsole.Markup("[bold green]Package Information[/]\n");
                        AnsiConsole.Markup($"[bold]Name:[/] {pkgName}\n");
                        AnsiConsole.Markup($"[bold]Version:[/] {pkgVersion}\n");
                        AnsiConsole.Markup($"[bold]Description:[/] {pkgDesc} \n");
                        AnsiConsole.Markup($"[bold]Author:[/] {pkgAuthor} \n");
                        AnsiConsole.Markup($"[bold]License:[/] {pkgLic} \n");

                        var confirm = AnsiConsole.Confirm("Do you want to install this package?");
                        if (confirm) {
                            // Moving the files to the correct directory
                            Directory.Move("C:\\Windows\\Temp\\InstallerCache\\", "C:\\Program Files\\" + pkgName);
                            Console.WriteLine("Package installed successfully.");
                            return 0;
                        }
                        else {
                            Console.WriteLine("Package installation cancelled.");
                            return 1;
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
                Console.WriteLine("  -p, --pack <dir>  Pack a directory into a package");
                Console.WriteLine("  -pc --package-config <config_name> ");
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