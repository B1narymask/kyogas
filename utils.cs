using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace Utils;
public static class Helper {
    public static string[] types = {
    		"int", 
    		"byte", 
    		"uint", 
    		"str", 
    		"bool", 
    		"arr", 
    		"flt", 
    		"obj", 
    		"short", 
    		"ushot",
    		"sbyte",
    		"long",
    		"ulong"
		  };

    public static string[] truthy = {
    		"t", 
    		"true", 
    		"yes", 
    		"1", 
    		"on"
    };
    public static string[] falsy = {
    		"f", 
    		"false", 
    		"no", 
    		"0", 
    		"off"
    };
    
    public static string nums = "-1234567890";
    public static string fltNums = "-1234567890.";
    public static char[] strs = ['\'', '"', ];
   
    public static bool boolify(string str, uint ln) {
        if (truthy.Contains(str)) return true;
        else if (falsy.Contains(str)) return false;
        else  {
        		WriteLine($"bool.Invalid [{ln}]: {str} is not a valid boolean.");
        		WriteLine($"Tip:\nValid booleans\ntruthy: \n\t{foreach (var x in truthy) Write($"{x} ");}");
        		WriteLine($"falsy: {foreach (var x in falsy) write($"{x} ");}");
        }
    }n
    public static string escapeCheck(string str, uint ln) {
        foreach (char character in str) {
            int cIndex = str.IndexOf(c);
            if (character == '\\' ) 
            {
                switch(str[cIndex+1])
                {
                    case 'n': 
                        str = str.Replace("\\n", "\n"); 
                        break;
                    case 't': 
                        str = str.Replace("\\t", "\t"); 
                        break;
                    case '\\':
                    default: 
                        WriteLine($"str.escape.unknown [{ln}]: '\\{character}' is not a recognized escape sequence."); 
                        return ".:ERR:.";
                }
            }
        }
        return str;
    }
    public static string unquote(string str, uint ln) {
        char first = str[0];
        char last  = str[str.Length - 1];
        
        if (first == '"' && last == '"' || (first == '\'' && last == first)) 
        {
            return str[1..^1];
        }
        else if (first != last) 
        {
            Console.WriteLine($"str.misquoted [{ln}]: string {str} has a mismatched/missing quote.");
            return "";
        }

        return str[1..^1];

    }
    public static string getType(string str, uint ln) {

        string[] _temp = str.Split(' ', 2);
        string _type = _temp[0];
        
        if (_type.StartsWith("arr<") && _type.EndsWith(">")) 
        {
            string subtype = _type.Split('<')[1];
            // to get rid of the closing '>'
            subtype = subtype[0..^1]; 
            return subtype switch 
            {
                "int"    => "arr.int",
                "flt"    => "arr.flt",
                "byte"   => "arr.u8",
                "uint"   => "arr.u32",
                "str"    => "arr.str",
                "bool"   => "arr.bool",
                "short"  => "arr.i16",
                "ushort" => "arr.u16",
                "long"   => "arr.i64"
                "ulong"  => "arr.u64",
                "sbyte"  => "arr.i8"
                _        => ""
            };
        }
        else if (_type.StartsWith("arr<") && _type.EndsWith(">") == false) 
        {
            WriteLine($"type.arr.unclosed [{ln}]: Array type delcaration is missing the closing angle bracket ('>')");
            return "";
        }
        else if (_type.StartsWith("arr.") && _type.Contains("<") == false && _type.EndsWith(">")) 
        {
            WriteLine($"type.arr.unopened [{ln}]: Array type delcaration is missing the opening angle bracket ('<')");
            return "";
        }
        if (str.Contains("<==") && str.LastIndexOf("=") == str.Length - 1 && str.StartsWith("obj ")) 
        {
            return "obj";
        }
        else if (Helper.types.Contains(_type)) 
        {
            return _type;
        }
        else 
        {
            if (Helper.types.Contains(_type.ToLower())) 
            {
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
        
        while (i < str.Length) 
        {
            if (Helper.nums.Contains(str[(int)i])) 
            {
                matches++;
            }
            
            i++;
        }
        if (str.Length == 1 && str[0] == '-') 
        {
            WriteLine($"int.invalid [{ln}]: '{str}' is not a valid integer.");
            return false;
        }
        else if (str.Contains('-') && str[0] != '-') 
        {
            WriteLine($"int.invalid [{ln}]: '{str}' is not a valid integer.");
            return false;
        }
        else if (matches != str.Length || str.Count(c => c == '-') > 1) 
        {
            WriteLine($"int.invalid [{ln}]: '{str}' is not a valid integer.");
            return false;
        }
        else if (string.IsNullOrWhiteSpace(str)) 
        {
            WriteLine($"flt.empty [{ln}]: expected a float, got nothing.");
            return false;
        }
        return true;
    }
    public static bool flt(string str, uint ln) {
        uint matches = 0;
        uint i = 0;
        
        while (i < str.Length) {
            if (Helper.fltNums.Contains(str[(int)i]))
            {
                matches++;
            }
            i++;
        }
        if (matches != str.Length || str.Count(c => c == '.') > 1 ) 
        {
            WriteLine($"flt.invalid [{ln}]: '{str}' is not a valid float.");
            return false;
        }
        else if (str.Contains(".") && str.Count(c => c == '.') == 1) 
        {
            int x = str.IndexOf(".") + 1;
            
            if (x == str.Length) 
            {
                WriteLine($"flt.invalid [{ln}]: '{str}' is not a valid float.");
                return false;
            }
        }
        else if (string.IsNullOrWhiteSpace(str)) 
        {
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
        try 
        {
            uint _a = Convert.ToUInt32(str);
        }
        catch (Exception) 
        {
            WriteLine($"uint.underflow [{ln}]: {str} is negative. Unsigned integers cannot be negative.");
            return false;
        }
        return true;
    }
    public static bool u8(string str, uint ln) {
        if (string.IsNullOrWhiteSpace(str)) {
            WriteLine($"byte.empty [{ln}]: expected a byte, got nothing.");
            return false;
        }
        // check if theres a '-' symbol -- no real reason to convert to an int first.
        if (str[0] == '-') {
            WriteLine($"byte.underflow [{ln}]: {str} is negative. Bytes cannot be negative.");
            return false;
        }
        if (int.TryParse(str, out int asInt) && asInt > 255) {
            WriteLine($"byte.overflow [{ln}]: Value {str} is greater than the 8 bit unsigned integer limit (255)\n(Basically, this shouldn't be over 255)");
            return false;
        }
        if (byte.TryParse(str, out _) == false) {
            WriteLine($"byte.invalid [{ln}]: '{str}' is not a valid byte (0-255).");
            return false;
        }
        return true;
    }
    public static bool i64(string str, uint ln) {
							long x = null;
							try {
									x = Convert.ToInt64(str);
							}
							catch {
									WriteLine($"long.invalid [{ln}]: {str} is not a valid long.");
									return false;
							}
							
							return true;
				}
				public static bool u64(string str, uint ln) {
							ulong x = null;
							try {
									x = Convert.ToUInt64(str);
							}
							catch {
									WriteLine($"ulong.invalid [{ln}]: {str} is not a valid ulong.");
									return false;
							}
							
							return true;
				}
				public static bool i16(string str, uint ln) {
							short x = null;
							try {
									x = Convert.ToInt16(str);
							}
							catch {
									WriteLine($"short.invalid [{ln}]: {str} is not a valid short.");
									return false;
							}
							
							return true;
				}
				public static bool u16(string str, uint ln) {
							ushort x = null;
							try {
									x = Convert.ToUInt16(str);
							}
							catch {
									WriteLine($"ushort.invalid [{ln}]: {str} is not a valid ushort.");
									return false;
							}
							
							return true;
				}

    public static bool str(string str, uint ln) {
        str        = str.Trim();
        char first = str[0];
        char last  = str[str.Length - 1];
        str        = Helper.unquote(str, ln);
        
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