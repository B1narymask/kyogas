using static System.Console;
using System;
using Kiogas;
class Program {
    static void Main(string[] argv) {
        if (argv.Length < 2) {
            WriteLine("parser.huh: uh... what am i supposed to parse? please provide a .kyo file as an argument.");
            return;
        }
        
        Parser parser =  new Parser();
        string file = argv[1];
        
        if (File.exists(file)) {
            parser.parse(file);
        } else {
            throw new Exception("The file you passed does not exist in this context.");
        }
    }
}
