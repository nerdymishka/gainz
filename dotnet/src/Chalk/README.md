# 🖍 NerdyMishka.Chalk 🖍

Console/Terminal string styling for .NET console applications based on the
amazing [chalk][chalk] library and it's dependencies in node.

## Overview

I looked for a .NET library that similar to chalk while writing Kryptos, a command
line to for encrypting/decrypting files. I did not find one. What I did find
was exciting news around the Windows Console.

[Microsoft has a team that][blog] is working hard on transforming the Windows Console
after decades of gathering dust.

Windows 10 1511 and above enabled true color for the Windows console. This
library will enable virtual terminal for Stand Output streams on Window 10.
The virtual termainl is enabled in static constructor for `Chalk`. Chalk uses
ANSI codes to style terminal strings.

The chalk library for node relies on [libuv][libuv] to handle true color
support for windows.

Console/Terminal emulators often support 256 colors, but not true color, this
include ConEmu. This library will attempt to find the closet color that the
terminal supports.

## Samples

```csharp
ChalkConsole.WriteLine("test test", Color.Red);

Console.WriteLine(Chalk.Red().Draw("This is a test"));
Console.WriteLine(Chalk.BrightRed().Draw("This is a test"));
Console.WriteLine(Chalk.Color("Orange")
    .Draw("orange you glad Microsoft is updating the console?"));

Console.WriteLine(Chalk.Rgb(255, 0, 0).Draw("Rgb accepted"));
Console.WriteLine(Chalk.BgRed().White().Draw("This is a test"));
Console.WriteLine(Chalk.Magenta().Draw("This is a test w/o bold"));
Console.WriteLine(Chalk.Magenta().Bold().Draw("This is a test"));
Console.WriteLine(Chalk.BrightYellow().Draw("This is a test w bright"));
Console.WriteLine(Chalk.Yellow().Draw("This is a test w/o bright"));
Console.WriteLine(Chalk.Yellow().Dim().Draw("This is a test w  dim"));
Console.WriteLine(
    Chalk.Color("Purple", true)
    .Bold()
    .White()
    .Underline()
    .Draw(" > Woah! "));
Console.WriteLine("Back to being a normie");
Console.WriteLine(Chalk.Grey("chill"));
Console.WriteLine(Chalk.Black("chill"));

// emojis still do not work in the window's console
Console.WriteLine(Chalk.StrikeOut().Draw("3 strikes 😃"));
```

![](https://pbs.twimg.com/media/Dslbn2PU0AAA9SR.jpg)

## Release Notes

- **0.1.0**: Gather early feedback on the API and use across multiple
  platforms.  

## License

Copyright 2018 Nerdy Mishka

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.


[chalk]: https://github.com/chalk/chalk.git
[libuv]: https://github.com/libuv/libuv
[blog]: https://blogs.msdn.microsoft.com/commandline/2018/08/02/windows-command-line-introducing-the-windows-pseudo-console-conpty/