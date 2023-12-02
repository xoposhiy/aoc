# Autoparser for adventofcode style of input

Input is parsed automatically according to Solve method parameters.

For example this input:
```
1 2
1 3
2 3

1
3
```
Can be automatically parsed to arguments of this method:
```csharp
void Solve((int src, int dest)[] graph, (int source, target) task)
```

## Blocks
Input is parsed to array of blocks. Blocks in input are separated by empty line.

Every parameter of Solve method is one block.
Params parameter is array of blocks.

```csharp
void Solve(BlockItem[] block1, string block2, params string[] otherBlocks)
```

Simple inputs are consisted of only one block. 

```csharp
// Input:
// 42
// 43
// 44
void Solve(int[] values)
```

Each block is usually an array, where each item is parsed from one input line, 
but there are special cases for block:

* Multiline string
* Multiline JsonNode
* Multiline object
* char[][] map - for rectangular maps or mazes
* int[][] map - for rectangular map of some values

## Block Lines

Each line can be parsed as:

* just a string
* object which arguments are parsed from line by regex template  (specified by TemplateAttribute on the object type definition)
* JsonNode
* splitted to fields and parsed:
    * to object via its constructor
    * to array of values
    * to array of objects

## More details and examples


Parameter named map is parsed as `char[][]` or `int[][]`.

```csharp
// Input:
// #.##
// #..# 
// ##.# 

void Solve(char[][] map)
```

`TemplateAttribute` can be assigned to types to specify regex template for parsing.
Use `@ParameterName` to specify parameter position in template.

```csharp
// Input:
// x=1, y=2
// x=3,   y=42

[Template("x=@X,\s+y=@Y")]
record V(int X, int Y);

void Solve(V[] points)
```

`SeparatorAttribute` can be assigned to block parameters or to method `Solve` itself.
```csharp
// Input:
// a → 42
// b → 43

[Separators(" →")]
void Solve((int a, int b)[] mapping)

void Solve([Separators(" →")](int a, int b)[] mapping)

```

Objects can be nested
```csharp
// Input:
// (1 2) add (3 4) 
// (5 6) substract (1 0) 

record V(int X, int Y);

[Separators("() ")]
void Solve((V left, string operation, V right)[] commands)

```
Be careful with arrays in objects. They are parsed greedily!
```csharp
// Input:
// ball 1,2 3,4 
// table 5,6 7,8 9,10
// chair 1,0

void Solve((int name, (int x, int y)[])[] objects)
```

## More details in code

See more examples in the Unit Tests in [ParsingTests project](../ParsingTests/Tests.cs).

See more details in the implementation: [ParsingExtensions.cs](ParsingExtensions.cs)