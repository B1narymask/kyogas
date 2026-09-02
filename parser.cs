using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

private static class Helper {
    public static string glyphs = "-!°<#+%";
    public static string[] bools = {"f", "t", "true", "false", "True", "False", "TRUE", "FALSE", "T", "F"};
    public static string nums = "-1234567890";
    public static string fltNums = "-1234567890.";
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
        if (str.Contains('-') && str[0] != '-') {
            WriteLine($"int.invalid [{ln}]: '{str}' is not a valid integer.");
            return false;
        }
        if (matches != str.Length || str.Count(c => c == '-') > 1) {
            WriteLine($"int.invalid [{ln}]: '{str}' is not a valid integer.");
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
        return true;
    }
    public static bool positive(string str, uint ln) {
        if (str.contains('-')) { 
            WriteLine($"uint.underflow [{ln}]: {str} is negative. Unsigned integers cannot be negative.");
            return false;
        }
        try uint _a =Convert.ToUInt32(str);
        catch (Exception) {
            WriteLine($"uint.underflow [{ln}]: {str} is negative. Unsigned integers cannot be negative.");
            return false;
        }
        return true;
    }
}


public class Parser {
    private bool inArr = false;
    // don't ask me why I store the name twice, it just works. 
    private Dictionary<string, (string Name, string Type, string Value)> _data = new();
    private List<string> names = new();
    private string parseGlyph(string line, uint num) {
        line = line.Trim();
        char first = line[0];
        if (!(Helper.glyphs.Contains(first))) {
            Console.WriteLine($"type.marker.unknown: Unrecognized type marker '{first}' in line {num}.");
            return "ERR";
        }
        else {
            switch(first) {
                case '-': return "str";
                case '!': return "bool";
                case '°': return "flt";
                case '#': return "int";
                case '+': return "uint";
                case '<': return "arr";
                case '%': return "byte";
            }
        }
    }
    private bool isDuplicate(string name) {
        if (names.Contains(name)) {
            Console.WriteLine($"name.duplicate: There are two or more keys named '{name}'.");
            return true;
        }
        return false;
    }
    private void parseArrType(string line, uint num) {
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
    }
    public void parse(string fn) {
        string[] lines = File.ReadAllLines(fn);
        uint lineNum = 0;
        for (string line in lines) {
            lineNum++;
            if (line[0] == '|' || string.IsNullOrWhiteSpace(line)) continue; // ignores comments and empty lines
            string[] _parts = line.Split(' ', 2);
            string name = _parts[0];
            // _data[name]["name"] = name;
            if (isDuplicate(name)) return;
            names.Add(name);
            string val = _parts[1];
            string type = parseGlyph(line, lineNum);
            if (type == "ERR") return;
            _data[name] = (name, type, val);
            if (type == "arr") {
                inArr = true;
                string _temptype = parseArrType(line, lineNum);
                if (_temptype == "arr.ERR") return; 
                _data[name].Type = _temptype;
            }
            while (inArr && line.Trim() != ">" {
                if (line[0] == '|' || string.IsNullOrWhiteSpace(line)) continue;
                
                if (_data[name].Type == "arr.int" && !IsIt.Int(line, lineNum) || (_data[name].Type == "arr.flt" && !IsIt.flt(line, lineNum))) {
                    WriteLine($"arr.item.mismatch [{lineNum}]: {line} is not does not match {_data[name].Name}'s type, thus cannot be appended .");
                    return;
                }

                else if (_data[name].Type == "arr.bool" && !(Helper.bools.Contains(line))) {
                    WriteLine($"arr.item.mismatch [{lineNum}]: {line} is not does not match {_data[name].Name}'s type, thus cannot be appended .");
                    return;
                }
              
                else if (_data[name].Type == "arr.uint" &&  !IsIt.positive(line, lineNum)) {
                    WriteLine($"arr.item.mismatch [{lineNum}]: {line} is not does not match {_data[name].Name}'s type, thus cannot be appended .");
                    return;
                }
             }
        }
    }
}