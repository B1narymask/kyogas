using static System.Console;
using System;
using System.IO;
using Kiogas;
class Program {
    static void Main(string[] argv) {
        if (argv.Length < 2) {
            WriteLine("parser.huh: uh... what am i supposed to parse? please provide a .kyo file as an argument.");
            return;
        }
        
        Parser parser =  new Parser();

        // Kind of redundant, but the check above may not catch this
        try {
            string file = argv[1];
        } catch (IndexOutOfRangeException) {
            WriteLine("parser.huh: uh... what am i supposed to parse? please provide a .kyo file as an argument.");
            return;
        }
            
        if (File.exists(file)) {
            parser.parse(file);
        } else {
            throw new Exception("The file you passed does not exist in this context.");
        }
    }
}
