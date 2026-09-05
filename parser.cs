using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;
namespace Kiogas {
    public static class Helper {
        public static string[] types = {"int", "byte", "uint", "str", "bool", "arr", "flt"};
        public static string[] bools = {"f", "t", "true", "false"};
        public static string nums = "-1234567890";
        public static string fltNums = "-1234567890.";
        public static char[] strs = ['\'', '"', ];
        public static object boolify(string str) {
            switch(str) {
                case "f":     return false;
                case "t":     return true;
                case "true":  return true;
                case "false": return false;
                default: WriteLine($"bool.invalid: {str} is not a valid boolean"); return null;
            }
            // return true;
        }
        public static string escapeCheck(string str, uint ln) {
            foreach (char c in str) {
                int cIndex = str.IndexOf(c);
                if (c == '\\' ) {
                    switch(str[cIndex+1]){
                        case 'n': str = str.Replace("\\n", "\n"); break;
                        case 't': str = str.Replace("\\t", "\t"); break;
                        case '\\': 
                        default: WriteLine($"str.escape.unknown [{ln}]: '\\{c}' is not a recognized escape sequence."); return ".:ERR:.";
                    }
                }
            }
            return str;
        }
        public static string unquote(string str, uint ln) {
            char first = str[0];
            char last  = str[str.Length - 1];
            if (first == '"' && last == '"' || (first == '\'' && last == first)) {
                return str[1..^1];
            }
            else if (first != last) {
                Console.WriteLine($"str.misquoted [{ln}]: string {str} has a mismatched/missing quote.");
                return "";
            }

            return str[1..^1];

        }
        public static string getType(string str, uint ln) {
            if(!str.Contains(" ")) {
                WriteLine($"parser.missingSpace [{ln}]: Missing space. How did this even happen?");
                return "";
            }
            string[] _temp = str.Split(' ', 2);
            string _type = _temp[0];
            if (_type.StartsWith("arr<") && _type.EndsWith(">")) {
                string subtype = _type.Split('<')[1];
                subtype = subtype[0..^1]; // to get rid of the closing '>'
                return subtype switch {
                    "int" => "arr.int",
                    "flt" => "arr.flt",
                    "byte" => "arr.u8",
                    "uint" => "arr.u32",
                    "str" => "arr.str",
                    "bool" => "arr.bool",
                    _ => ""
                };
            }
            else if (_type.StartsWith("arr<") && !_type.EndsWith(">")) {
                WriteLine($"type.arr.unclosed [{ln}]: Array type delcaration is missing the closing angle bracket ('>')");
                return "";
            }
            else if (_type.StartsWith("arr") && !_type.Contains("<") && _type.EndsWith(">")) {
                WriteLine($"type.arr.unopened [{ln}]: Array type delcaration is missing the opening angle bracket ('<')");
                return "";
            }
            else if (Helper.types.Contains(_type)) return _type;
            else {
                if (Helper.types.Contains(_type.ToLower())) {
                    WriteLine($"type.similar [{ln}]: There is no such type '{_type}'. Did you mean {_type.ToLower()}?");
                    return "";
                }
            }
            return "";
        }
    }

    public static class IsIt {
        public static bool Int(string str, uint ln) {
            uint matches = 0;
            uint i = 0;
            while (i < str.Length) {
                if (Helper.nums.Contains(str[(int)i])) matches++;
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
                if (Helper.fltNums.Contains(str[(int)i])) matches++;
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
        public static bool str(string str, uint ln) {
            str = str.Trim();
            char first = str[0];
            char last  = str[str.Length - 1];
            str = Helper.unquote(str, ln);
            if (str == "") return false;
            else return true;
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
        private Dictionary<string, Data> data = new();
        private List<string> names = new();
        private bool isDuplicate(string name) {
            if (names.Contains(name)) {
                Console.WriteLine($"name.duplicate: There are two or more keys named '{name}'.");
                return true;
            }
            return false;
        }
        public Dictionary<string, Data> parse(string fn) {
            // you might want to wrap this in a try catch
            string[] lines = File.ReadAllLines(fn);
            for (uint i = 0; i < lines.Length; i++) {
                int __i = Convert.ToInt32(i);
                uint lineNum = i+1;
                int intline = (int)i+1;
                string line = lines[__i];
                line = line.Trim();
                if (line.Trim() == "->") inArr = false;
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line[0] == '|') continue; // ignores comments and empty lines
                
                if (line.Trim()[0] == '-') {
                    if (line.Trim()[1] == '>' && line.IndexOf(">") != line.Length-1) {
                        WriteLine($"arr.terminator.polluted [{lineNum}]: Polluted array terminator (the end of an array should be JUST '->', NOTHING else)");
                        break;
                    }
                }
                string[] _parts = line.Split(':', 2);
                _parts[0] = _parts[0].Trim();
                if (_parts.Length < 2 && !Helper.getType(line, lineNum).StartsWith("arr")) {
                    WriteLine($"key.value.missing [{lineNum}]: Key {_parts[0]} was not given a value.");
                    break;
                }
                else if (_parts.Length == 1 && Helper.getType(line, lineNum).StartsWith("arr")) {
                    inArr = true;
                }
                string name = _parts[0];
                if (isDuplicate(name)) break;
                if (_parts.Length >1) _parts[1] = _parts[1].Trim();
                names.Add(name);
                string val = "";
                if (!inArr) {
                    val = _parts[1]; 
                }
                else { 
                    // I have no idea why this is here but I'm scared to touch it - Wer
                    val = "arr"; 
                }
                if (val == "empty") {
                    val = null;
                }
                string type = Helper.getType(line, lineNum);
                if (type == "") break;
                data[name] = new Data {
                    Name = name,
                    Value = val,
                    Type = type,
                    Array = new List<object>(),
                    IsArr = (type == "arr")
                };
                // ¿¿¿ debug
                //WriteLine(val);
                WriteLine(type);
                if (inArr) WriteLine("got to inArr loop");
                // ??? debug
                
                if (type.StartsWith("arr")) {
                    inArr = true;
                    string _temptype = Helper.getType(line, lineNum);
                    if (_temptype == "") break;
                    data[name].Type = _temptype;
                
                    i++;  
                    while (inArr && i < lines.Length) {
                        string arrLine = lines[i].Trim();
                        // WriteLine(arrLine);
                        uint arrLineNum = (uint)i + 1;
                        
                        if (arrLine.Trim() == ">") {
                            inArr = false;
                            i++;
                            break;
                        }
                        i++;
                        if (arrLine[0] == '|' || string.IsNullOrWhiteSpace(arrLine)) continue;

                        if (arrLine[0] == '|' || string.IsNullOrWhiteSpace(arrLine)) continue;
                    
                        if (data[name].Type == "arr.int" && !IsIt.Int(arrLine, arrLineNum) || (data[name].Type == "arr.flt" && !IsIt.flt(arrLine, arrLineNum))) {
                            WriteLine($"arr.item.mismatch [{arrLineNum}]: {arrLine} is not does not match {data[name].Name}'s type, thus cannot be appended .");
                            break;
                        }

                        else if (data[name].Type == "arr.bool" && !(Helper.bools.Contains(arrLine))) {
                            WriteLine($"arr.item.mismatch [{arrLineNum}]: {arrLine} is not does not match {data[name].Name}'s type, thus cannot be appended .");
                            break;
                        }
                    
                        else if (data[name].Type == "arr.u32" &&  !IsIt.positive(arrLine, arrLineNum)) {
                            WriteLine($"arr.item.mismatch [{arrLineNum}]: {arrLine} is not does not match {data[name].Name}'s type, thus cannot be appended .");
                            break;
                        }

                        // if (data[name].Type == "arr.str" && arrLine[0] == '<') data[name].Array.Add(arrLine);
                        switch(data[name].Type) {
                            case "arr.u32": 
                                if (IsIt.positive(arrLine, arrLineNum)) {
                                    data[name].Array.Add(Convert.ToUInt32(arrLine)); 
                                }
                                else {
                                    break;
                                }
                                break;
                            case "arr.int": 
                                if (IsIt.Int(arrLine, arrLineNum)) {
                                    data[name].Array.Add(Convert.ToInt32(arrLine));
                                }
                                else {
                                    break;
                                }
                                break; 
                            case "arr.str": 
                                if (IsIt.str(arrLine, arrLineNum)) data[name].Array.Add(arrLine);
                                else break;
                                break;
                            case "arr.bool": 
                                if (Helper.bools.Contains(arrLine)) {
                                    data[name].Array.Add(Helper.boolify(arrLine)); 
                                }
                                else {
                                    break;
                                }
                                break;
                            case "arr.flt": 
                                if (IsIt.flt(arrLine, arrLineNum)) {
                                    data[name].Array.Add(arrLine); 
                                }
                                else {
                                    break;
                                }
                                break;
                            case "arr.u8": 
                                if (IsIt.u8(arrLine, arrLineNum)) {
                                    data[name].Array.Add(arrLine); 
                                }
                                else {
                                    break;
                                }
                                break;
                            default: break;
                        }
                    }
                }
            }
            // !!!!!!! DEBUG
            foreach (var kvp in data) {
                if (kvp.Value.IsArr) {
                    WriteLine($"{kvp.Key}: [{kvp.Value.Type}] = {string.Join(", ", kvp.Value.Array)}");
                } else {
                    WriteLine($"{kvp.Key}: [{kvp.Value.Type}] = {kvp.Value.Value}");
                }
                
            }
            return data;
        }
    }
}
