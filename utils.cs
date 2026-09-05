using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace Utils;
public static class Helper {
    public static string[] types = {"int", "byte", "uint", "str", "bool", "arr", "flt", "obj"};
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
        else if (_type.StartsWith("arr.") && !_type.Contains("<") && _type.EndsWith(">")) {
            WriteLine($"type.arr.unopened [{ln}]: Array type delcaration is missing the opening angle bracket ('<')");
            return "";
        }
        if (str.Contains("<==") && str.LastIndexOf("=") == str.Length - 1 && str.StartsWith("obj ")) return "obj";
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
    public Dictionary<string, object> Object { get; set; }
    public bool IsObj { get; set; }
    public List<string> objType { get; set; }
}