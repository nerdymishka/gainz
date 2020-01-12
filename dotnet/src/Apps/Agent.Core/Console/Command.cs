

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace NerdyMishka.Console
{

    public class Command<T> : Command
    {
        private Func<T, Task<int>> command;

        private Type[] argumentTypes = new Type[] { typeof(T)};

        public Command(
            Func<T, Task<int>> command, 
            string description = null, 
            string noun = null)
          :base(description, noun)
        {
            this.command = command;
        }

        public override IReadOnlyCollection<Type> ArgumentsTypes => this.argumentTypes;

        public override async Task<int> ExecuteAsync(object parameters, CancellationToken token = default(CancellationToken))
        {
            return await this.command((T)parameters);
        }
    }

    public class Command : ICommand
    {
        private Func<object, Task<int>> command;

        public virtual string Noun { get; }

        public virtual string Description { get; }

        public virtual IReadOnlyCollection<Type> ArgumentsTypes { get; }

   
        public Command(
            string description = null, 
            string noun = null) 
        {
            this.Description = description;
            this.Noun =noun;
        }

        public Command(
            Func<object, Task<int>> command, 
            IReadOnlyCollection<Type> argumentsTypes,
            string description = null,
            string noun = null) : 
        this(description, noun)
        {
            this.command = command;
            this.ArgumentsTypes = argumentsTypes;
        }
        
        public bool CanExecute(Type argumentType)
        {
            if(this.ArgumentsTypes == null || this.ArgumentsTypes.Count == 0)
                return false;

            return this.ArgumentsTypes.Contains(argumentType);
        }

        public virtual async Task<int> ExecuteAsync(object parameters, CancellationToken token = default)
        {
            return await this.command(parameters);
        }
    }
}