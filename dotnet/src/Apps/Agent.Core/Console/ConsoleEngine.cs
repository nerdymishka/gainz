using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using CommandLine;
using NerdyMishka.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Console
{
    public class ConsoleEngine
    {

        private IReadOnlyCollection<Assembly> assemblies;
        private IReadOnlyCollection<string>  directories;
        private CompositionContainer container;

        public class ConsoleEngineOptions
        {
            public IReadOnlyCollection<Assembly> Assemblies { get; set; }

            public IReadOnlyCollection<string> Directories { get; set; }


        }

        public ConsoleEngine(ConsoleEngineOptions options = null)
        {
            options = options ?? new ConsoleEngineOptions() {
                Assemblies = new List<Assembly>(){ typeof(ConsoleEngine).Assembly }
            };  

            this.assemblies = options.Assemblies;
            this.directories = options.Directories;
        }


        public async Task<int> RunAsync(string[] args)
        {
            if(this.container == null)
            {
                
                var catalog = new AggregateCatalog();
                foreach(var assembly in this.assemblies)
                    catalog.Catalogs.Add(new AssemblyCatalog(assembly));
                
                if(this.directories != null && this.directories.Count > 0)
                {
                    foreach(var dir in directories)
                        catalog.Catalogs.Add(new DirectoryCatalog(dir));
                }

                this.container = new CompositionContainer(catalog);
            }

            string noun = null;
                if(args.Length > 0)
                    noun = args[0];
           
            var actions = this.container.GetExports<ICommand>().Select(o => o.Value);
            if(!string.IsNullOrWhiteSpace(noun))
                actions = actions.Where(o => o.Noun.Match(noun));

            var argumentTypes = actions.SelectMany(o => o.ArgumentsTypes).ToArray();
            var result = (Parsed<object>)CommandLine.Parser.Default.ParseArguments(args, argumentTypes);
            var argumentValues = result.Value;
            var type = argumentValues.GetType();
            var action = actions.FirstOrDefault(o => o.CanExecute(type));
            if(action != null)
                    return await action.ExecuteAsync(argumentValues);

            return 1;
        }
    }   
}