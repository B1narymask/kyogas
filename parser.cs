using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utils;

using static System.Console;

namespace Kiogas;
public class Parser
{
    private bool inArr                     = false; // currently inside of an array
    private bool inObj                     = false; // currently inside of an object

    private Dictionary<string, Data> data  = new();
    private List<string> names             = new();

    private bool isDuplicate(string name)
    {
        if (names.Contains(name))
        {
            Console.WriteLine($"name.duplicate: There are two or more keys named {name}");
            return true;
        }
        return false;
    }

    public Dictionary<string, Data> parse(string fn)
    {
        string[] lines = File.ReadAllLines(fn);
        for (uint i = 0; i < lines.Length; i++)
        {
            int __i = Convert.ToInt32(i);
            uint lineNum = i + 1;
            string line = lines[__i].Trim();

            if (string.IsNullOrWhiteSpace(line) || line[0] == '|')
            {
                continue;
            }

            if (line.StartsWith("->"))
            {
                inArr = false;
                if (line.Contains("->") && line != "->")
                {
                    WriteLine($"arr.terminator.polluted [{lineNum}]: Polluted array terminator (the end of an array should be JUST '->', NOTHING else)");
                    break;
                }
                continue;
            }
            if (line.StartsWith("=>>"))
            {
                inObj = false;
                if (line.Contains("=>>") && line != "=>>")
                {
                    WriteLine($"obj.terminator.polluted [{lineNum}]: Polluted object terminator (the end of an object should be JUST '=>>', NOTHING else)");
                    break;
                }
            }
            string type = Helper.getType(line, lineNum);
            if (string.IsNullOrEmpty(type))
            {
                break;
            }

            string[] _parts = line.Split(':', 2);
            _parts[0]       = _parts[0].Trim();

            // debug
            WriteLine($"debug | type: {type}");

            if (_parts.Length < 2 && (!type.StartsWith("arr.") && type != "obj"))
            {
                WriteLine($"key.value.missing [{lineNum}]: Key {_parts[0]} was not given a value.");
                break;
            }

            string name = _parts[0].Split(' ')[1];

            if (name.StartsWith("<-") && type.StartsWith("arr."))
            {
                name = name[2..];
            }

            if (isDuplicate(name))
            {
                break;
            }

            names.Add(name);

            string val = "";

            if (_parts.Length > 1)
            {
                val = _parts[1].Trim();
                if (val == "empty")
                {
                    val = null;
                }
            }
            if (type == "str" && val.Contains("\\"))
            {
                val = Helper.unquote(val, lineNum);
            }

            bool isArrayType  = type.StartsWith("arr.");
            bool isObjectType = (type == "obj");
            data[name]        = new Data
            {
                Name    = name,
                Value   = val,
                Type    = type,
                Array   = new List<object>(),
                IsArr   = isArrayType,
                Object  = new Dictionary<string, object>(),
                IsObj   = isObjectType,
                objType = null
            };

            if (isArrayType)
            {
                inArr = true;
                i++;

                while (inArr && i < lines.Length)
                {
                    string arrLine  = lines[(int)i].Trim();
                    uint arrLineNum = i + 1;

                    if (arrLine == "->")
                    {
                        inArr = false;
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(arrLine) || arrLine[0] == '|')
                    {
                        i++;
                        continue;
                    }

                    switch (data[name].Type)
                    {
                        case "arr.u32":
                            if (IsIt.positive(arrLine, arrLineNum))
                            {
                                data[name].Array.Add(Convert.ToUInt32(arrLine));
                            }
                            break;
                        case "arr.int":
                            if (IsIt.Int(arrLine, arrLineNum))
                            {
                                data[name].Array.Add(Convert.ToInt32(arrLine));
                            }
                            break;
                        case "arr.str":
                            if (IsIt.str(arrLine, arrLineNum))
                            {
                                data[name].Array.Add(Helper.unquote(arrLine, arrLineNum));
                            }
                            break;
                        case "arr.bool":
                            if (Helper.bools.Contains(arrLine))
                            {
                                data[name].Array.Add(Helper.boolify(arrLine));
                            }
                            break;
                        case "arr.flt":
                            if (IsIt.flt(arrLine, arrLineNum))
                            {
                                data[name].Array.Add(Convert.ToDouble(arrLine));
                            }
                            break;
                        case "arr.u8":
                            if (IsIt.u8(arrLine, arrLineNum))
                            {
                                data[name].Array.Add(Convert.ToByte(arrLine));
                            }
                            break;
                        default:
                            break;
                    }
                    i++;
                }
            }

            if (isObjectType) {
                inObj      = true;
                int keyNum = 0;
                i++;

                if (data[name].Object == null)
                {
                    data[name].Object = new Dictionary<string, object>();
                }
                if (data[name].objType == null)
                {
                    data[name].objType = new List<string>();
                }

                while (inObj && i < lines.Length)
                {
                    string objLine = lines[(int)i].Trim();
                    uint objln     = i + 1;

                    if (objLine == "==>")
                    {
                        inObj = false;
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(objLine) || objLine[0] == '|')
                    {
                        i++;
                        continue;
                    }

                    if (!objLine.Contains(":")) {
                        WriteLine($"obj.missingColon [{objLine}]: Missing colon.");
                        i++;
                        break;
                    }

                    string[] parts         = objLine.Split(':', 2);
                    string key             = parts[0].Trim();
                    string keyVal          = parts[1].Trim();

                    data[name].Object[key] = keyVal;

                    // infer key type
                    data[name].objType.Add(Helper.getType(objLine, objln));
                    string t = data[name].objType[keyNum];

                    if (t == "str" && keyVal.Contains("\\"))
                    {
                        keyVal                 = Helper.unquote(keyVal, objln);
                        data[name].Object[key] = keyVal;
                    }

                    if (t == "obj")
                    {
                        WriteLine($"obj.nested [{objln}]: Nested objects are not supported.");
                        i++;
                        break;
                    }

                    keyNum++;
                    i++;
                }

                // debug
                int j = 0; // what does this even do??? - Centurion
                foreach (var x in data[name].Object)
                {
                    WriteLine("DEBUG:");
                    WriteLine($"{x.Key}: [{data[name].objType[j]}] = {x.Value}");
                    j++;
                }
            }
        }

        foreach (var kvp in data)
        {
            if (kvp.Value.IsArr)
            {
                WriteLine($"{kvp.Key}: [{kvp.Value.Type}] = {string.Join(", ", kvp.Value.Array)}");
            }
            else
            {
                WriteLine($"{kvp.Key}: [{kvp.Value.Type}] = {kvp.Value.Value}");
            }
        }
        return data;
    }
}
