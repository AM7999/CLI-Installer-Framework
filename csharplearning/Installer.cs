using System.IO;
using System;
using Microsoft.Deployment.Compression.Cab;
using Newtonsoft.Json;
using Spectre.Console;
using net.am7999.Util;
using net.am7999.Package;

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
                        AnsiConsole.Markup($"[bold]License:[/] {pkgLic} \n\n");

                        string installPath = "C:\\Program Files\\";

                        var changeInstallPath = AnsiConsole.Confirm("Do you want to change the default install path? (currently: " + installPath + pkgName);
                        if (changeInstallPath) {
                            var newPath = AnsiConsole.Prompt(
                                new TextPrompt<string>("\nPlease enter a [bold]new[/] install path: "));
                            installPath = newPath;
                        }

                        var confirm = AnsiConsole.Confirm("Do you want to install this package?");
                        if (confirm) {
                            // Moving the files to the correct directory
                            net.am7999.Util.Util.CopyDirectory("C:\\Windows\\Temp\\InstallerCache\\", installPath + pkgName);
                            AnsiConsole.Markup("[green]Package Installation Completed Sucessfuly[/]");
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

            if (args[0] == "-p" || args[0] == "--package") {
                if (args[1] != "") {
                    if (args[2] != "") {
                        if (File.Exists(args[2])) {

                        }
                        else {
                            AnsiConsole.Markup("[red]Could Not find file!!![/]");
                            return 0;
                        }
                    }
                    else {

                    }
                }
                else {

                }
            }

            if (args[0] == "-pc" || args[0] == "--package-config")

            // Checking if the first argument is -h or --help
            if (args[0] == "-h" || args[0] == "--help"){
                Console.WriteLine("Installer, A simple package installer written in C#\n");
                Console.WriteLine("Usage: Installer.exe [Option] <File> ");
                Console.WriteLine("Options: ");
                Console.WriteLine("  -i, --install <file>  Install a package");
                Console.WriteLine("  -p, --pack <dir> <config>  Pack a directory into a package");
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