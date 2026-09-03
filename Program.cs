using static System.Console;
using System;

class Program {
    static void Main(string[] argv) {
        if (argv.Length < 1) {
            Writeline("parser.huh: uh... what am i supposed to parse? please provide a .kyo file as an argument.");
            return;
        }
        parse(argv[0]);
    }
}