using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace NerdyMishka
{
    /// <summary>
    /// The default implementation of <see cref="IConsole" /> that maps to <see cref="System.Console" />
    /// </summary>
    public class NerdyConsole : IConsole
    {
        public virtual int WindowWidth 
        { 
            get => Console.WindowWidth; 
            set => Console.WindowWidth = value; 
        }

        public virtual bool IsOutputRedirected => Console.IsOutputRedirected;

        public virtual bool IsErrorRedirected => Console.IsErrorRedirected;
        public virtual TextReader In => Console.In;
        
        public virtual TextWriter Out => Console.Out;

        public virtual TextWriter Error => Console.Error;

        public virtual Encoding InputEncoding 
        {
            get => Console.InputEncoding;
            set => Console.InputEncoding = value;
        }

        public virtual Encoding OutputEncoding 
        {
            get => Console.OutputEncoding;
            set => Console.OutputEncoding = value;
        }
        public virtual ConsoleColor BackgroundColor 
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        public virtual ConsoleColor ForegroundColor 
        { 
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value; 
        }
        public virtual int BufferHeight 
        {
            get => Console.BufferHeight;
            set => Console.BufferHeight = value;
        }

        public virtual int BufferWidth 
        {
            get => Console.BufferWidth;
            set => Console.BufferWidth = value;
        }

        public virtual int WindowHeight 
        {
            get => Console.WindowHeight;
            set => Console.WindowHeight = value;
        }
        public virtual bool TreatControlCAsInput 
        {
            get => Console.TreatControlCAsInput;
            set => Console.TreatControlCAsInput = value;
        }

        public virtual int LargestWindowWidth  
        {
            get => Console.LargestWindowWidth;
        }
        public virtual int LargestWindowHeight => Console.LargestWindowHeight;
        public virtual int WindowLeft 
        {
            get => Console.WindowLeft;
            set => Console.WindowLeft = value;
        }
        public virtual int WindowTop 
        {
            get => Console.WindowTop;
            set => Console.WindowTop = value;
        }
        public virtual int CursorLeft 
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public virtual int CursorTop 
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }
        public virtual  int CursorSize
        {
            get => Console.CursorSize;
            set => Console.CursorSize = value;
        }
        
        public virtual bool CursorVisible 
        {
            get => Console.CursorVisible;
            set => Console.CursorVisible = value;
        }
        public virtual string Title
        {
            get => Console.Title;
            set => Console.Title = value;
        }
        public virtual bool KeyAvailable 
        {
            get => Console.KeyAvailable;
        }
        public virtual bool NumberLock 
        {
            get => Console.NumberLock;
        }
        public virtual bool CapsLock 
        {
            get => Console.CapsLock;
        }
        public virtual bool IsInputRedirected { get => Console.IsInputRedirected; }

        public virtual event ConsoleCancelEventHandler CancelKeyPress 
        {
            add => Console.CancelKeyPress += value;
            remove => Console.CancelKeyPress -= value;
        }

        public virtual void Beep() => Console.Beep();

       
        public virtual void Beep(int frequency, int duration) => Console.Beep(frequency, duration);
        
        public virtual void Clear() => Console.Clear();
        public virtual void MoveBufferArea(
            int sourceLeft, 
            int sourceTop, 
            int sourceWidth, 
            int sourceHeight, 
            int targetLeft, 
            int targetTop)
            => Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
       
        public virtual void MoveBufferArea(
            int sourceLeft, 
            int sourceTop, 
            int sourceWidth, 
            int sourceHeight, 
            int targetLeft, 
            int targetTop, 
            char sourceChar, 
            ConsoleColor sourceForeColor, 
            ConsoleColor sourceBackColor)
            => Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, sourceChar, sourceForeColor, sourceBackColor);
        public virtual Stream OpenStandardError()
            => Console.OpenStandardError();
        public virtual Stream OpenStandardError(int bufferSize)
            => Console.OpenStandardError(bufferSize);
        public virtual Stream OpenStandardInput(int bufferSize)
            => Console.OpenStandardInput(bufferSize);
        public virtual Stream OpenStandardInput()
            => Console.OpenStandardInput();
        public virtual Stream OpenStandardOutput(int bufferSize)
            => Console.OpenStandardOutput(bufferSize);

        public virtual Stream OpenStandardOutput()
            => Console.OpenStandardOutput();
        public virtual int Read()
            => Console.Read();
       
        public virtual ConsoleKeyInfo ReadKey(bool intercept)
            => Console.ReadKey(intercept);
        public virtual ConsoleKeyInfo ReadKey()
            => Console.ReadKey();

        public virtual string ReadLine()
            => Console.ReadLine();
        
        public virtual void ResetColor()
            => Console.ResetColor();
      
        public virtual void SetBufferSize(int width, int height)
            => Console.SetBufferSize(width, height);
        public virtual void SetCursorPosition(int left, int top)
            => Console.SetCursorPosition(left, top);
        
        public virtual void SetError(TextWriter newError)
            => Console.SetError(newError);
      
        public virtual void SetIn(TextReader newIn)
            => Console.SetIn(newIn);
        
        public virtual void SetOut(TextWriter newOut)
            => Console.SetOut(newOut);

        public virtual void SetWindowPosition(int left, int top)
            => Console.SetWindowPosition(left, top);
     
        public virtual void SetWindowSize(int width, int height)
            => Console.SetWindowSize(width, height);
        public virtual void Write(string value)
            => Console.Write(value);

        public virtual void WriteAnsi(string value, Color color)
            => Console.Write(value);

        public virtual void WriteAnsi(string value, params int[] ansiCodes)
            => Console.Write(value);

        public virtual void Write(object value)
            => Console.Write(value);

        public virtual void Write(string format, object arg0, object arg1)
            => Console.Write(format, arg0, arg1);
      
        public virtual void Write(string format, object arg0, object arg1, object arg2, object arg3)
            => Console.Write(format, arg0, arg1, arg2, arg3);
        public virtual  void Write(string format, params object[] arg)
            => Console.Write(format, arg);
     
        public virtual void Write(char[] buffer, int index, int count)
            => Console.Write(buffer, index, count);
        public virtual void Write(string format, object arg0, object arg1, object arg2)
            => Console.Write(format, arg0, arg1, arg2);
     
        public virtual void WriteLine()
            => Console.WriteLine();
     
        public virtual void WriteLine(object value)
            => Console.WriteLine(value);

        public virtual void WriteLine(string value)
            => Console.WriteLine(value);

        public virtual void WriteLineAnsi(string value, Color color)
            => Console.WriteLine(value);

        public virtual void WriteLineAnsi(string value, params int[] ansiCodes)
            => Console.WriteLine(value);
        public virtual void WriteLine(string format, object arg0)
            => Console.WriteLine(format, arg0);

        public virtual void WriteLine(string format, object arg0, object arg1, object arg2)
            => Console.WriteLine(format, arg0, arg1, arg2);
   
        public virtual void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
            => Console.WriteLine(format, arg0, arg1, arg2, arg3);

        public virtual void WriteLine(string format, params object[] arg)
            => Console.WriteLine(format, arg);
        public virtual void WriteLine(char[] buffer, int index, int count)
            => Console.WriteLine(buffer, index, count);
        
        public virtual void WriteLine(string format, object arg0, object arg1)
            => Console.WriteLine(format, arg0, arg1);
    }
}