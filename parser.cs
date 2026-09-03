using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;
namespace Kiogas {
    public static class Helper {
        public static string glyphs = "-!°<#+%";
        public static string[] bools = {"f", "t", "true", "false", "True", "False", "TRUE", "FALSE", "T", "F"};
        public static string nums = "-1234567890";
        public static string fltNums = "-1234567890.";
        public static bool boolify(string str) {
            // string[] realbools = {"}
            str = str.ToLower();
            switch(str) {
                case "f": return false;
                case "t": return true;
                case "true": return true;
                case "false": return false;
            }
            return true;
        }
    }

    public static class IsIt {
        public static bool Int(string str, uint ln) {
            uint matches = 0;
            uint i = 0;
            while (i < str.Length) {
                if (Helper.nums.Contains(str[i])) matches++;
                i++;
            }
            if (str.Length == 1 && str[0] == '-') {
                WriteLine($"int.invalid [{ln}]: '{str}' is not a valid integer.");
                return false;
            }
            else if (str.Contains('-') && str[0] != '-') {
                WriteLine($"int.invalid [{ln}]: '{str}' is not a valid integer.");
                return false;
            }
            else if (matches != str.Length || str.Count(c => c == '-') > 1) {
                WriteLine($"int.invalid [{ln}]: '{str}' is not a valid integer.");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(str)) {
                WriteLine($"flt.empty [{ln}]: expected a float, got nothing.");
                return false;
            }
            return true;
        }
        public static bool flt(string str, uint ln) {
            uint matches = 0;
            uint i = 0;
            while (i < str.Length) {
                if (Helper.fltNums.Contains(str[i])) matches++;
                i++;
            }
            if (matches != str.Length || str.Count(c => c == '.') > 1 ) {
                WriteLine($"flt.invalid [{ln}]: '{str}' is not a valid float.");
                return false;
            }
            else if (str.Contains(".") && str.Count(c => c == '.') == 1) {
                int x = str.IndexOf(".")+1;
                if (x == str.Length) {
                    WriteLine($"flt.invalid [{ln}]: '{str}' is not a valid float.");
                    return false;
                }
            }
            else if (string.IsNullOrWhiteSpace(str)) {
                WriteLine($"flt.empty [{ln}]: expected a float, got nothing.");
                return false;
            }
            return true;
        }
        public static bool positive(string str, uint ln) {
            if (str.Contains('-')) { 
                WriteLine($"uint.underflow [{ln}]: {str} is negative. Unsigned integers cannot be negative.");
                return false;
            }
            try {uint _a =Convert.ToUInt32(str);}
            catch (Exception) {
                WriteLine($"uint.underflow [{ln}]: {str} is negative. Unsigned integers cannot be negative.");
                return false;
            }
            return true;
        }
        public static bool u8(string str, uint ln) {
            int x = Convert.ToInt32(str);
            if (x > 255) {
                WriteLine($"byte.overflow [{ln}]: Value {str} is greater than the 8 bit unsigned integer limit (255)\n(Basically, this shouldn't be over 255)");
                return false;
            }
            if (x < 0) {
                WriteLine($"byte.underflow: [{ln}]: Bytes cannot be negative."); 
                return false;
            }
            if (x > 0 && x < 256) return true;
            return true;
        }
    }

    public class Data {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public List<object> Array { get; set; }
        public bool IsArr { get; set; }
    }

    public class Parser {
        private bool inArr = false;
        // don't ask me why I store the name twice, it just works. 
        private Dictionary<string, Data> data = new();
        private List<string> names = new();
        private string parseGlyph(string line, uint num) {
            line = line.Trim();
            char first = line[0];
            if (!(Helper.glyphs.Contains(first))) {
                Console.WriteLine($"type.marker.unknown: Unrecognized type marker '{first}' in line {num}.");
                return "ERR";
            } 
            switch(first) {
                case '-': return "str";
                case '!': return "bool";
                case '°': return "flt";
                case '#': return "int";
                case '+': return "uint";
                case '<': return "arr";
                case '%': return "byte";
            }

            return "ERR";
            
        }
        private bool isDuplicate(string name) {
            if (names.Contains(name)) {
                Console.WriteLine($"name.duplicate: There are two or more keys named '{name}'.");
                return true;
            }
            return false;
        }
        private string parseArrType(string line, uint num) {
            char id = line[1];
            if (!(Helper.glyphs.Contains(id))) {
                Console.WriteLine($"arr.invalid: Invalid type marker '{id}' found after array marker (<) in line {num}.");
                return "arr.ERR";
            }
            else {
                switch(id) {
                    case '-': return "arr.str";
                    case '!': return "arr.bool";
                    case '°': return "arr.flt";
                    case '#': return "arr.int";
                    case '+': return "arr.uint";
                    case '%': return "arr.byte";
                }
            }
            return "arr.ERR";
        }
        public void parse(string fn) {
            string[] lines = File.ReadAllLines(fn);
            for (uint i = 0; i < lines.Length; i++) {
                int __i = Convert.ToInt32(i);
                uint lineNum = i+1;
                int intline = (int)i+1;
                string line = lines[__i];
                if (line.Trim() == ">") inArr = false;
                if (line[0] == '|' || string.IsNullOrWhiteSpace(line)) continue; // ignores comments and empty lines
                if (line.Trim()[0] == '>' && line.IndexOf(">") != line.Length-1) {
                    WriteLine($"arr.terminator.poluted [{lineNum}]: Polluted array terminator (the end of an array should be JUST '>', NOTHING else)");
                    return;
                }
                string[] _parts = line.Split(' ', 2);
                if (_parts.Length < 2) {
                    WriteLine($"key.value.missing [{lineNum}]: Key {_parts[0]} was not given a value.");
                    return;
                }
                string name = _parts[0];
                if (isDuplicate(name)) return;
                names.Add(name);
                string val = _parts[1]; 

                    
                
                string type = parseGlyph(line, lineNum);
                if (type == "ERR") return;
                data[name] = new Data {
                    Name = name,
                    Value = val,
                    Type = type,
                    Array = new List<object>(),
                    IsArr = (type == "arr")
                };
                if (type == "arr") {
                    inArr = true;
                    string _temptype = parseArrType(line, lineNum);
                    if (_temptype == "arr.ERR") return;
                    data[name].Type = _temptype;
                
                    i++;  
                    while (inArr && i < lines.Length) {
                        string arrLine = lines[i];
                        uint arrLineNum = (uint)i + 1;
                        i++;
                        
                        if (arrLine.Trim() == ">") {
                            inArr = false;
                            break;
                        }
                        if (arrLine[0] == '|' || string.IsNullOrWhiteSpace(arrLine)) continue;

                        if (arrLine[0] == '|' || string.IsNullOrWhiteSpace(arrLine)) continue;
                    
                        if (data[name].Type == "arr.int" && !IsIt.Int(arrLine, arrLineNum) || (data[name].Type == "arr.flt" && !IsIt.flt(arrLine, arrLineNum))) {
                            WriteLine($"arr.item.mismatch [{arrLineNum}]: {arrLine} is not does not match {data[name].Name}'s type, thus cannot be appended .");
                            return;
                        }

                        else if (data[name].Type == "arr.bool" && !(Helper.bools.Contains(arrLine))) {
                            WriteLine($"arr.item.mismatch [{arrLineNum}]: {arrLine} is not does not match {data[name].Name}'s type, thus cannot be appended .");
                            return;
                        }
                    
                        else if (data[name].Type == "arr.uint" &&  !IsIt.positive(arrLine, arrLineNum)) {
                            WriteLine($"arr.item.mismatch [{arrLineNum}]: {arrLine} is not does not match {data[name].Name}'s type, thus cannot be appended .");
                            return;
                        }
                        switch(data[name].Type) {
                            case "arr.uint": if (IsIt.positive(arrLine, arrLineNum)) data[name].Array.Add(Convert.ToUInt32(line)); break;
                            case "arr.int": if (IsIt.Int(arrLine, arrLineNum))       data[name].Array.Add(Convert.ToInt32(line)); break;
                            case "arr.str":                                          data[name].Array.Add(line); break;
                            case "arr.bool": if (Helper.bools.Contains(arrLine))     data[name].Array.Add(Helper.boolify(line)); break;
                            case "arr.flt": if (IsIt.flt(arrLine, arrLineNum))       data[name].Array.Add(line); break;
                            case "arr.byte": if (IsIt.u8(arrLine, arrLineNum))       data[name].Array.Add(line); break;
                        }
                    }
                }
            }
        }
    }
}