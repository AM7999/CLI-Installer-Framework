using System.Runtime.InteropServices;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace net.am7999.Util {
    public class Util {

        private string WinTempDir = "C:\\Windows\\Temp\\InstallerCache";
        private string UnixTempDir = "/tmp/InstallerCache/";

        public static void DeleteDirectory(string target_dir) {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        public static string FileReader(string file) {
            StreamReader sr = new StreamReader(file);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }
        
        // this originally wasn't cross platform, going to have to run a few checks now to determine
        // how to get the host architecture
        public static string GetHostArch() {
            return "amd64";
            //return arch;
        }
        public static void CreateFolder(string folderPath, string folderName) {
            if (!Directory.Exists(folderPath + folderName))
            {
                Directory.CreateDirectory(folderPath + folderName);
            }
        }

        // This is so that we can actually copy through to different volumes on the host computer
        public static void CopyDirectory(string sourceDir, string destDir) {
            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var dest = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, dest);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                var dest = Path.Combine(destDir, Path.GetFileName(directory));
                CopyDirectory(directory, dest);
            }
        }
        public static bool doesFolderExsist(string folder) {
            if(Directory.Exists(folder)) {
                return true;
            } else {
                return false;
            }
        }

        // i forgot to make my code cross platform early on so
        // i mean, here we are now on May 2, 2025
        public static string returnTempPath(bool getOSName) {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))  {
                if(getOSName) {
                    return "Linux";
                }
                else {
                    return "/tmp/InstallerCache/";
                }
            }
            if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                if(getOSName) {
                    return "macOS";
                } else {
                    return "/tmp/InstallerCache/";
                }
            }
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                if(getOSName) {
                    return "Windows";
                } else {
                    return "C:\\Windows\\Temp\\InstallerCache\\";
                }
            }
            throw new Exception("Unknown Operating system");
        }
    }
}

namespace net.am7999.Package {
    public class Package {
        public static void Unpack(string zipFile, string destDir) {
            // its zipfile time!
            ZipFile.ExtractToDirectory(zipFile, destDir);
        }
        public static void Pack(string sourceDir, string destDir) {
            ZipFile.CreateFromDirectory(sourceDir, destDir + ".zip");
        }

        // This is probably going to get me publicly executed by Mr. bill gates for how just messy this function is
        // whoever developed dotnet I deeply am sorry for this function
        public static string getPkgInformation(string json,
            bool getPkgName, bool getPkgVersion,
            bool getPkgDesc, bool getAuthorElements,
            bool checkLic) {
            string pkgInfo = net.am7999.Util.Util.FileReader(json);
            dynamic pkg = JsonConvert.DeserializeObject(pkgInfo);
            if(pkg != null) {
                if (getPkgName) { return pkg["name"]; }
                if (getPkgVersion) { return pkg["version"]; }
                if (getPkgDesc) { return pkg["description"]; }
                if (getAuthorElements) { return pkg["author"]["name"] + " " + pkg["author"]["email"]; }
                if (checkLic) { return pkg["license"]; }
            }
            else if(pkg == null) {
                throw new Exception("No data in json file");
            }

            return "No information Specified.";
        }

        // This function is used to check if the package is compatible with the host architecture
        // need to make this cross platform because right now i really just
        // this is just bad code how about that
        public static bool returnPackageArch(string json) {
            string pkgInfo = net.am7999.Util.Util.FileReader(json);
            dynamic pkg = JsonConvert.DeserializeObject(pkgInfo);
            string pkgArch = pkg["arch"];
            string expectedArch = net.am7999.Util.Util.GetHostArch();
            // How does my code get any worse than this
            if (pkgArch != expectedArch.ToLower() && pkgArch != "Any") {
                AnsiConsole.MarkupLine("[bold red]The package is not compatible with the host architecture.[/]");
                return false;
            }
            if (pkgArch != expectedArch.ToLower() && pkgArch == "Any") {
                return true;
            }
            return true;
        }

        public static void generateUninstallFiles(string json, string installPath) {
            dynamic pkg = JsonConvert.DeserializeObject(json);
            for(int i = 0; i <= 2; i++ ) {
                if (pkg["OS"][i]["name"] == Util.Util.returnTempPath(true)) {
                    var fileArray = pkg["OS"][i]["files"].ToObject<string[]>();
                    int x = fileArray.Length - 1;
                    for (int y = 0; y <= x; y++) {
                        Console.WriteLine(pkg["OS"][i]["files"][y]);

                    }

                    return;
                }
                else if (pkg["OS"][i]["name"] != Util.Util.returnTempPath(true))
                    throw new Exception("The package is not compatible with the host operating system");
            }

        }
    }
}
