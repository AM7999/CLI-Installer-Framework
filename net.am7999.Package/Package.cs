using System.Dynamic;
using Microsoft.Deployment.Compression.Cab;
using Newtonsoft.Json;
using net.am7999.Util;
using Spectre.Console;

namespace net.am7999.Util {
    public class Util {
        public static string fileReader(string file) {
            StreamReader sr = new StreamReader(file);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }

        public static string GetHostArch() {
            string arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            return arch;
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
    }
}


namespace net.am7999.Package {
    public class Package {
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
            // Might go for a different approach for packing the cab file, MakeCab seems to be a better (easier) option since i can just query through each file in a dir then add it to the MakeCab ddf?
            cab.Pack(sourceDir, true, Microsoft.Deployment.Compression.CompressionLevel.Normal, null);
        }

        // This is probably going to get me publicly executed by Mr. bill gates for how just messy this function is
        public static string getPkgInformation(string json,
            bool getPkgName, bool getPkgVersion,
            bool getPkgDesc, bool getAuthorElements,
            bool checkLic) {
            string pkgInfo = net.am7999.Util.Util.fileReader(json);
            dynamic pkg = JsonConvert.DeserializeObject(pkgInfo);
            if (getPkgName) { return pkg["name"]; }
            if (getPkgVersion) { return pkg["version"]; }
            if (getPkgDesc) { return pkg["description"]; }
            if (getAuthorElements) { return pkg["author"]["name"] + " " + pkg["author"]["email"]; }
            if (checkLic) { return pkg["license"]; }
            return "No information found.";
        }

        // This function is used to check if the package is compatible with the host architecture
        public static bool returnPackageArch(string json) {
            string pkgInfo = net.am7999.Util.Util.fileReader(json);
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

        public struct jsonSchema {
            public string pkgName;
            public string pkgVersion;
            public string pkgDesc;

        }
    }
}
