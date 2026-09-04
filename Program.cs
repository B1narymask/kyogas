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
            throw new Exception("external.fileSys: The file you passed does not exist in this context.");
        }

        if (!file.Contains(".")) {
            WriteLine("external.fileSys.extension.missing: No file extension found");
            return;
        }
        else if (!file.EndsWith(".kyo")) {
            string[] temp = file.Split(".");
            WriteLine($"external.fileSys.extension.incorrect: Expected .kyo extension, got .{temp[1]}");
            return;
        }
    }
}
