using Spectre.Console;
using net.am7999.Util;
using net.am7999.Package;

class App {
    static int Main(String[] args) {

        // exiting if you didn't provide an argument
        if (args.Length == 0) {
            Console.WriteLine("Installer, A simple package installer written in C#\n");
            Console.WriteLine("Usage: Installer.exe [option] <file/folder> ");
            Console.WriteLine("Options: ");
            Console.WriteLine("  -i, --install <file>  Install a package");
            Console.WriteLine("  -p, --pack <dir>  Pack a directory into a package");
            Console.WriteLine("  -h, --help  Display this help message\n");
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
                        // this caused so much pain for me in testing like you dont know how much time i spent trying to figure why the fuck my installer was bringing files back from the dead
                        bool checkForLeftoverCacheDir = Util.doesFolderExsist(Util.returnInstallPath(false));
                        if (checkForLeftoverCacheDir) { Util.DeleteDirectory(Util.returnInstallPath(false)); }
                        // Unpacking the package
                        AnsiConsole.Status()
                            .Spinner(Spinner.Known.Shark)
                            .Start("Unpacking to Temporary Directory...", ctx => {
                                Package.Unpack(args[1], Util.returnInstallPath(false));
                            });

                        // checking the pkg architecture
                        bool pkgArch = Package.returnPackageArch(Util.returnInstallPath(false) + "packageManifest.json");
                        if (!pkgArch) {
                            return 1;
                        }

                        // Grabbing the package information
                        string pkgName = Package.getPkgInformation(Util.returnInstallPath(false) + "packageManifest.json", true, false, false, false, false);
                        string pkgVersion = Package.getPkgInformation(Util.returnInstallPath(false) + "packageManifest.json", false, true, false, false, false);
                        string pkgDesc = Package.getPkgInformation(Util.returnInstallPath(false) + "packageManifest.json", false, false, true, false, false);
                        string pkgAuthor = Package.getPkgInformation(Util.returnInstallPath(false) + "packageManifest.json", false, false, false, true, false);
                        string pkgLic = Package.getPkgInformation(Util.returnInstallPath(false) + "packageManifest.json", false, false, false, false, true);

                        AnsiConsole.Markup("[bold green]Package Information[/]\n");
                        AnsiConsole.Markup($"[bold]Name:[/] {pkgName}\n");
                        AnsiConsole.Markup($"[bold]Version:[/] {pkgVersion}\n");
                        AnsiConsole.Markup($"[bold]Description:[/] {pkgDesc} \n");
                        AnsiConsole.Markup($"[bold]Author:[/] {pkgAuthor} \n");
                        AnsiConsole.Markup($"[bold]License:[/] {pkgLic} \n\n");

                        string installPath = Util.returnInstallPath(false);

                        var changeInstallPath = AnsiConsole.Confirm("Do you want to change the default install path? (currently: " + installPath + pkgName, false);
                        if (changeInstallPath) {
                            var newPath = AnsiConsole.Prompt(
                                new TextPrompt<string>("\nPlease enter a [bold]new[/] install path: "));
                            installPath = newPath;
                        }

                        var confirm = AnsiConsole.Confirm("Do you want to install this package?");
                        if (confirm) {
                            AnsiConsole.Status()
                            .Spinner(Spinner.Known.Shark)
                            .Start("Installing...", ctx => {
                                Util.CopyDirectory(Util.returnInstallPath(false) + "", installPath + pkgName);
                                //Package.GenerateUninstallFiles(path);
                            });

                            // Moving the files to the correct directory

                            AnsiConsole.Markup("[green]Package Installation Completed Sucessfuly[/]");
                            return 0;
                        }
                        else {
                            Console.WriteLine("Package installation cancelled.");
                            return 1;
                        }
                    }
                    else {
                        AnsiConsole.Markup("[red]The file does not exist.[/]");
                        return 1;
                    }
                }
                else {
                    Console.WriteLine("Please provide a file to install.");
                    return 1;
                }
            }

            if(args[0] == "-d") {
                string pkg = Util.FileReader(args[1]);
                Package.generateUninstallFiles(pkg);
                return 10;
            }

            if (args[0] == "-p" || args[0] == "--package") {
                if (args[1] != "") {
                    bool checkForLeftoverCacheDir = Util.doesFolderExsist(Util.returnInstallPath(false));
                    if (checkForLeftoverCacheDir) { Util.DeleteDirectory(Util.returnInstallPath(false) + "*"); }

                    string pkgName = Package.getPkgInformation(args[1] + "packageManifest.json", true, false, false, false, false);
                    string pkgAuthor = Package.getPkgInformation(args[1] + "packageManifest.json", false, false, false, true, false);
                    string pkgVersion = Package.getPkgInformation(args[1] + "packageManifest.json", false, true, false, false, false);

                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Material)
                        .Start("[green]Packaging: [/]" + pkgName + " By: " +  pkgAuthor + " v" + pkgVersion, ctx => { Package.Pack(args[1], "C:\\" + pkgName + ".cab" );});
                }
                else {
                    AnsiConsole.Markup("[red] Could not find " + args[1] + "[/]");
                }
            }

            // Checking if the first argument is -h or --help
            if (args[0] == "-h" || args[0] == "--help"){
                Console.WriteLine("Installer, A simple package installer written in C#\n");
                Console.WriteLine("Usage: Installer.exe [option] <file/folder> ");
                Console.WriteLine("Options: ");
                Console.WriteLine("  -i, --install <file>  Install a package");
                Console.WriteLine("  -p, --pack <dir> <config>  Pack a directory into a package");
                Console.WriteLine("  -h, --help  Display this help message\n");
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
  // Thank you github copilot for the lyrics (how the hell did it actually get it like actually decently correct)
