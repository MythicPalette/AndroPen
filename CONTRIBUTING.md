# Bug Reporting
If you have a bug report, please look for an existing bug report. If none exists, please write up a new one.

# Code Submissions
Everyone is welcome to submit pull requests. Please try to adhere to the code standards.

# Code Standards
## Indentation & Spacing
* Use ***4*** spaces for indentation.
* Add extra spaces between control statements, parentheses, and curly braces.
* Do not add spaces in parenthesis for casting.
```cs
for( int i = 0; i <= segments; i++ )
{
    float t = i / (float)segments;
    curvePoints[i] = QuadraticBezierInterpolation( p1, p2, p3, t );
}
```

## Method Naming
Method names should use `PascalCase`, with the first letter of every word capitalized .
```cs
public void SimulateTouch( RemotePointerInfo[] rpis )
```
## Variable Naming
Use `camelCase` for variables and parameters.
```cs
PointerFlags pointerFlags = PointerFlags.NONE;
Vector2 point = rpis[i].Translate( ScreenUtils.GetNamedBounds( Settings.ScreenDevice ));
```

## Member Variable & Property
Member variables should use `camelCase`. If the variable is `protected` or `private` it should be prefixed with an underscore (`_`)
```cs
private int _pointerId;
private string _screenDevice;
```

Member properties should use `PascalCase`. Do not use `protected` or `private` properties.

Members should be accessed with `this.` for clarity
```cs
private void DrawPoints( Graphics g )
{
    using var brush = new SolidBrush( this.NodeColor );
    DrawPoint( g, PointToScreenCoords( this._data.Threshold ), brush );
    DrawPoint( g, PointToScreenCoords( this._data.Softness ), brush );
    DrawPoint( g, PointToScreenCoords( this._data.Cap ), brush );
}
```

## Constants
Use `UPPER_SNAKE_CASE` for constants.

## Control Structures
Use curly braces `{}` for `if`, `else`, `for`, and `foreach` blocks only if the block contains more than one line of code.
```cs
if ( condition )
    // One line statement;

if ( condition )
{
    // Multi-line statement here
}
```

## Comments
Use single-line comments `//` for inline comments
Use multi-line comments `/* */` for longer explanations or sections of code.
Always describe ***why*** the commented code does what it does, not just what it does.

## Object Initialization
Use ***object initializer syntax*** when creating new objects with multiple properties.
```cs
outData[i] = new PointerTypeInfoTouch
{
    type = PointerInputType.PT_TOUCH,
    touchInfo = new PointerTouchInfo
    {
        pointerInfo = new PointerInfo
        {
            pointerType = PointerInputType.PT_TOUCH,
            pointerId = (uint)rpis[i].PointerId
        }
    }
};
```
## Line Length
Keep lines of code to a maximum of 80 characters. Break longer lines into smaller parts to improve readability.

## Error Handling
* Use descriptive language when handling errors to make debugging easier.
* Use the `Logging` class, not the `Console` class.
```cs
if ( !InjectSyntheticPointerInput( this._touch, outData, (uint)outData.Length ))
{
    Logging.Error( "Failed to inject touch input: " + Marshal.GetLastWin32Error() );
}
```

## Method Comments
Add doc comments `///` for public methods and classes to describe their purpose and parameters.
```cs
/// <summary>
/// Simulates touch input based on the provided remote pointer info.
/// </summary>
/// <param name="rpis">Array of remote pointer information</param>
public void SimulateTouch(RemotePointerInfo[] rpis)
```
