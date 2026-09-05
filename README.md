# Kyogas 
*(it's actually spelled "kiógas")*

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

Types are mostly intuitive, but for clarification purposes only, here is a small table:

|  Kyogas  | C#  |
|----|----|
|  str  |  string  |
|  int  |  int  |
|  flt  |  float  |
|  uint |  uint  |
|  byte |  byte  |
|  bool |  bool  |
|  arr<type>  | type[]  |

## Arrays

Arrays must have the `<-` prefix, otherwise it may cause parsing issues.

Valid: `arr<str> <-things` invalid: `arr<str> things`.

Arrays are closed with `->`

***Please note that the closing line MUST be the array terminator ONLY. If there are random characters after or before it, the parser will crash.***

### Valid vs Invalid Arrays

Invalid: 
```
arr<str> <-showcase
    "super cool"
    "and awesome"
    "strings should"
    "go here"
    "!!!!"
-> hi mom!!
```

Valid:

```
arr<str> <-showcase
    "super cool"
    "and awesome"
    "strings should"
    "go here"
    "!!!!"
->
```

## Dictionaries

Yeah, dicts... we don't have those. 

Kyogas doesn't and probably never will support dictionaries (or any kind of depth/nesting for that matter). 

Why?

Because I can't code them in.. I swear I've tried!!

## Null
You can assign the equivalent to `null` to a key with the `empty` keyword:
`str name: empty` = `string name = null;`

# Example snippet
A player save file for an RPG

```
str name: player
byte lvl: 5
uint gold: 358
uint xp: 44
byte hp: 100
byte max-hp: 100
byte defense: 4
byte attack: 45
arr<str> <-inventory
    | as an alternative to dicts, you could just use different files 
    | and reference them in string arrays
    "sword.kyo" 
    "strength-potion.kyo" 
->
```