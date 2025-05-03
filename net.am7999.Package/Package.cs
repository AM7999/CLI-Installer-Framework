using System.Dynamic;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Deployment.Compression.Cab;
using Newtonsoft.Json;
using Spectre.Console;

namespace net.am7999.Util {
    public class Util {

        private string WinTempDir = "C:\\Windows\\Temp\\InstallerCache";
        private string UnixTempDir = "/tmp/Installe";


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
        public static string returnInstallPath() {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) 
                return "/tmp/InstallerCache/";
            if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "/tmp/InstallerCache/";
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                return "C:\\Windows\\Temp\\InstallerCache\\";
            return "unknown";
        }
    }
}

namespace net.am7999.Package {
    public class Package {
        public static void Unpack(string cabFile, string destDir) {
            // Creating the CabInfo object
            CabInfo cab = new CabInfo(cabFile);
            // Unpacking the cab file to the destination directory
            cab.Unpack(destDir);
        }
        public static void Pack(string sourceDir, string destDir) {
            CabInfo cab = new CabInfo(destDir);
            // Packing files in the source directory with normal compression into the cab file
            cab.Pack(sourceDir, true, Microsoft.Deployment.Compression.CompressionLevel.Max, null);
        }

        // This is probably going to get me publicly executed by Mr. bill gates for how just messy this function is
        // whoever developed dotnet I deeply am sorry for this function
        public static string getPkgInformation(string json,
            bool getPkgName, bool getPkgVersion,
            bool getPkgDesc, bool getAuthorElements,
            bool checkLic) {
            string pkgInfo = net.am7999.Util.Util.FileReader(json);
            dynamic pkg = JsonConvert.DeserializeObject(pkgInfo);
            if (getPkgName) { return pkg["name"]; }
            if (getPkgVersion) { return pkg["version"]; }
            if (getPkgDesc) { return pkg["description"]; }
            if (getAuthorElements) { return pkg["author"]["name"] + " " + pkg["author"]["email"]; }
            if (checkLic) { return pkg["license"]; }
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
            if (pkgArch != expectedArch.ToLower() && pkgArch != "Any")
            {
                AnsiConsole.MarkupLine("[bold red]The package is not compatible with the host architecture.[/]");
                return false;
            }
            if (pkgArch != expectedArch.ToLower() && pkgArch == "Any")
            {
                return true;
            }
            return true;
        }

        public static void createInstallationInformation() {

        }
    }
}
