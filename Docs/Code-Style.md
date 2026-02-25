# Code Style Guide
## 'var' Usage
When possible, use explicit types instead of 'var'.

No:
```csharp
var a = 1;
var n = new Name("Bones");
```

Yes:
```csharp
int a = 1;
Name n = new("Bones");
var foo = new {
    something: 1
};
```

## Bracing Style
Braces should almost always be used, even for single line blocks.
The exception is within lambda expressions when they are not needed.

No:
```csharp
if (condition) DoSomething();
for (int i = 0; i < 10; i++) DoSomething();
list.Select(item =>
{
  return item.Id;
});
```

Yes:
```csharp
if (condition)
{
    DoSomething();
}

for (int i = 0; i < 10; i++)
{
    DoSomething();
}

list.Select(item => item.Id);

list.Select(item =>
{
  if (item.IsActive)
  {
      return item.Id;
  }
  else
  {
      return null;
  }
});
```

## Comparisons
When comparing to null, always use 'is' or 'is not'.
No:
```csharp
if (obj == null) { }
if (obj != null) { }
```

Yes:
```csharp
if (obj is null) { }
if (obj is not null) { }
```

When comparing values, always use '==' or '!='.
No:
```csharp
int i = 1;
if (i is 0) { }
```

Yes:
```csharp
int i = 1;
if (i == 0) { }
```

## Strings
When concatenating strings, prefer string interpolation instead of '+' operator.
When building strings in loops or large strings, prefer StringBuilder.

## XML Documentation
All public types and members should have XML documentation comments. This includes their parameters and return values. Missing these will fail the build.

Internal types and members may have XML documentation comments if it would improve code readability, but it is not required.

Private types and members should not have XML documentation comments, if you need additional explaination for code thats all in the same file, the code probably needs to be refactored.