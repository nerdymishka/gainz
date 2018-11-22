using System;
using NerdyMishka;
using System.Drawing;

namespace Ansi
{
    class Program
    {
        static void Main(string[] args)
        {
            ChalkConsole.WriteLine("test test", Color.Red);
          
            Console.WriteLine(Chalk.Red().Draw("This is a test"));
            Console.WriteLine(Chalk.BrightRed().Draw("This is a test"));
            Console.WriteLine(Chalk.Rgb(135,95,0).Draw("orange you glad Microsoft is updating the console?"));
            Console.WriteLine(Chalk.BgRed().White().Draw("This is a test"));
            Console.WriteLine(Chalk.Magenta().Draw("This is a test w/o bold"));
            Console.WriteLine(Chalk.Magenta().Bold().Draw("This is a test"));
            Console.WriteLine(Chalk.BrightYellow().Draw("This is a test w bright"));
     
            Console.WriteLine(Chalk.Yellow().Draw("This is a test w/o bright"));
                 Console.WriteLine(Chalk.Yellow().Dim().Draw("This is a test w  dim"));
            Console.WriteLine(Chalk.Color("Purple", true).Bold().White().Underline().Draw(" > Woah! "));
            Console.WriteLine("Back to being a normie");
            Console.WriteLine(Chalk.Grey("chill"));
            Console.WriteLine(Chalk.Black("chill"));
            Console.WriteLine(Chalk.StrikeOut().Draw("3 strikes 😃"));
           
        }
    }
}
