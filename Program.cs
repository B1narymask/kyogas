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
        parser.parse(argv[1]);
    }
}
