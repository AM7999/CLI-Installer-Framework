class App {
    static void Main(string[] args) {
        if(args.Length == 0) {
            Console.WriteLine("Please run this with the included batch file");
        }
        if(args.Length > 0) {
            // might honestly get rid of this csproj since i'm not sure if it's 100% necessary
            // especially since i could bake uninstalling into the installer
        }
    }
}