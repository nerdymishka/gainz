using System;
using System.Management.Automation.Host;
using Microsoft.Extensions.Logging;

namespace NerdyMishka.Management.Automation
{
    public class AppHostRawUI : PSHostRawUserInterface
    {
        private ILogger logger;
        private Lazy<IConsole> console;


        

        public AppHostRawUI(Lazy<IConsole> console, ILogger logger) 
        {
            if(this.logger == null)
                throw new ArgumentNullException(nameof(logger));

            this.logger = logger;
            this.console = console ?? new Lazy<IConsole>(() => new NerdyConsole());
        }

        public override ConsoleColor BackgroundColor 
        {
            get => this.console.Value.BackgroundColor;
            set => this.console.Value.BackgroundColor = value;
        }
        public override Size BufferSize
        {
            get => new Size(this.console.Value.BufferWidth, this.console.Value.BufferHeight);
            set => this.console.Value.SetBufferSize(value.Width, value.Height);
        }
        public override Coordinates CursorPosition
        {
            get => new Coordinates(this.console.Value.CursorLeft, this.console.Value.CursorTop);
            set => this.console.Value.SetCursorPosition(value.X, value.Y);
        }
        public override int CursorSize 
        {
            get => this.console.Value.CursorSize;
            set => this.console.Value.CursorSize = value;
        }
        public override ConsoleColor ForegroundColor
         {
            get => this.console.Value.ForegroundColor;
            set => this.console.Value.ForegroundColor = value;
        }
        public override bool KeyAvailable => this.console.Value.KeyAvailable;

        public override Size MaxPhysicalWindowSize => new Size(this.console.Value.LargestWindowWidth, this.console.Value.LargestWindowHeight);

        public override Size MaxWindowSize => this.MaxPhysicalWindowSize;

        public override Coordinates WindowPosition 
        {
            get => new Coordinates(this.console.Value.WindowLeft, this.console.Value.WindowTop);
            set => this.console.Value.SetWindowPosition(value.X, value.Y);
        }
        public override Size WindowSize 
        {
            get => new Size(this.console.Value.WindowWidth, this.console.Value.WindowHeight);
            set => this.console.Value.SetWindowSize(value.Width, value.Height);
        }
        public override string WindowTitle 
        {
            get => this.console.Value.Title;
            set => this.console.Value.Title = value;
        }

        public override void FlushInputBuffer()
        {
            
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
           
            
            throw new NotImplementedException();
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException();
        }
    }
}
