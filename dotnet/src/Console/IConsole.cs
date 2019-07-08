using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace NerdyMishka
{


    public interface IConsole
    {

        int WindowWidth { get; set; }
        bool IsOutputRedirected { get; }
        bool IsErrorRedirected { get; }
        TextReader In { get; }
        TextWriter Out { get; }
        TextWriter Error { get; }
        Encoding InputEncoding { get; set; }
        Encoding OutputEncoding { get; set; }
        ConsoleColor BackgroundColor { get; set; }
        ConsoleColor ForegroundColor { get; set; }
        int BufferHeight { get; set; }
        int BufferWidth { get; set; }
        int WindowHeight { get; set; }
        bool TreatControlCAsInput { get; set; }
        int LargestWindowWidth { get; }
        int LargestWindowHeight { get; }
        int WindowLeft { get; set; }
        int WindowTop { get; set; }
        int CursorLeft { get; set; }
        int CursorTop { get; set; }
        int CursorSize { get; set; }
        bool CursorVisible { get; set; }
        string Title { get; set; }
        bool KeyAvailable { get; }
        bool NumberLock { get; }
        bool CapsLock { get; }
        bool IsInputRedirected { get; }

        event ConsoleCancelEventHandler CancelKeyPress;

        void Beep();

       
        void Beep(int frequency, int duration);
        
        void Clear();
        void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);
       
        void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor);
        Stream OpenStandardError();
        Stream OpenStandardError(int bufferSize);
        Stream OpenStandardInput(int bufferSize);
        Stream OpenStandardInput();
        Stream OpenStandardOutput(int bufferSize);
        Stream OpenStandardOutput();
        int Read();
       
        ConsoleKeyInfo ReadKey(bool intercept);
        ConsoleKeyInfo ReadKey();
        string ReadLine();
        
        void ResetColor();
      
        void SetBufferSize(int width, int height);
        void SetCursorPosition(int left, int top);
        
        void SetError(TextWriter newError);
      
        void SetIn(TextReader newIn);
        
        void SetOut(TextWriter newOut);
        void SetWindowPosition(int left, int top);
     
        void SetWindowSize(int width, int height);
        void Write(string value);

        void WriteAnsi(string value, Color color);

        void WriteAnsi(string value, params int[] ansiCodes);

        void Write(object value);

        void Write(string format, object arg0, object arg1);
      
        void Write(string format, object arg0, object arg1, object arg2, object arg3);
        void Write(string format, params object[] arg);
     
        void Write(char[] buffer, int index, int count);
        void Write(string format, object arg0, object arg1, object arg2);
     
        void WriteLine();
     
        void WriteLine(object value);
        void WriteLine(string value);

        void WriteLineAnsi(string value, Color color);

        void WriteLineAnsi(string value, params int[] ansiCodes);
        void WriteLine(string format, object arg0);
        void WriteLine(string format, object arg0, object arg1, object arg2);
   
        void WriteLine(string format, object arg0, object arg1, object arg2, object arg3);
        void WriteLine(string format, params object[] arg);
        void WriteLine(char[] buffer, int index, int count);
        
        void WriteLine(string format, object arg0, object arg1);
    }
}