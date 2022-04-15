## MemberFilterAttribute Documentation

### Options Parameters Legend
Add these prefixes while declaring your string array options to add
more filtering to the filter attribute
 - "r:"+"System.Type.Name" → filters return type to match that type
 - "nc:"+"string" → filters members with name.Contains(string) 
 - "ne:"+"string" → filters members with name == string
 - "mpc:"+"Integer" → filters methods with that amount of parameters. -1 by default means any, 0 means 0 and so on.
 - "mpnc:"+"string" → filters methods with any parameters with name.Contains(string)
 - "mpne:"+"string" → filters methods with any parameters with name == string
 - "mpt:"+"System.Type.Name" → methods with parameters of type

### Supported Special Types Names
- Single as float
- Single[] as float[]
- Int32 as int
- Int32[] as int[]
- Boolean as bool
- Boolean[] as bool[]
- Void, String and String[] supports their lowercase void, string and string[]

### Notes
 - Yes, they're case sensitive!
 - Yes most of them support flat strings arrays separates by commas. "nc: stringA, stringB" with or without spaces.
 - If more "mpc" are found, only the last one will count!
 

 
 
 

