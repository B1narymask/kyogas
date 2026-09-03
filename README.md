# Kyogas 
*it's actually kiógas*

Kyogas (.kyo) is a minimalistic markup language with static typing that I made because I didn't have anything better to do. And because JSON is too verbose and I despise YAML using indentation for everything 


## Why should I use it?
Kyogas was made mainly as a personal project,  but if you're interested in it, here are some advantages:
- No bloaty syntax 
- Easy and fast to type
- Intuitive error codes that actually tell you what went wrong
- Static typing
- and a lot more to come!

# Syntax
Comments are only inline and marked with `|`
As mentioned before, Kyogas has static typing, which means that every key has a set type. 
Types are marked by a "prefix" or "type marker":

|  type  | marker  |
|----|----|
|  string  |  -  |
|  int  |  #  |
|  float  |  °  |
|  unsigned int |  +  |
|  byte  |  %  |
|  bool |  !  |
|  array  | (see below)  |

## Arrays

Arrays have *2*  type markers instead of just 1 like every other data type: `<` to indicate it's an array, then the type marker for the items, so an array of strings would be `<-`, one of integers would use `<#`, a list of booleans would be marked with `<!`, etc. 

## Dictionaries

Yeah, dicts... we don't have those. 
Kyogas doesn't and probably never will support dictionaries (or any kind of depth/nesting for that matter). If you want dictionaries, I don't know, use JSON or find a workaround

# Example snippet
A player save file for an RPG

```
-name player
%lvl 5
+gold 358
+xp 44
%hp 100
%max-hp 100
%defense 4
%attack 45
<-inventory
    | as an alternative to dicts, you could just use different files 
    | and reference them in string arrays
    "sword.kyo" 
    "strength-potion.kyo" 
>
```