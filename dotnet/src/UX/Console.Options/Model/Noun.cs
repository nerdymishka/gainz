
using System;
using System.Reflection;
using System.Collections.Generic;

namespace NerdyMishka.Extensions.Console.Options
{

    public class Directory
    {
        public List<Noun> Nouns { get; set; } = new List<Noun>();

    }

    public class Noun 
    {
        public Type ClrType { get; set; }
        public NounAttribute Attribute { get; set; }

        public string Name { get => this.Attribute?.Name; }

        public IList<Verb> Verbs { get; set; } = new List<Verb>();
    }

    public class Flag 
    {
        public MemberInfo Property { get; set; }

        public string Name { get => this.Attribute?.Name; }

        public string Alias { get => this.Attribute?.Alias;}

        public FlagAttribute Attribute { get; set ; }
    }

    public class RemainingArgs 
    {
        public MemberInfo Property { get; set; }

        public RemainingArgAttribute Attribute { get; set ; }
    }

    public class Verb 
    {
        public string Name { get => this.Attribute?.Name; }

        public string Alias { get => this.Attribute?.Alias;}

        public MemberInfo Property { get; set; }

        public Type ClrType  { get; set; }

        public FlagAttribute Attribute { get; set ; }

        public IList<Option> Options { get; set ;} = new List<Option>();

        public IList<Flag> Flags { get; set; } = new List<Flag>();

        public RemainingArgs RemainingArgs  {get; set; }
    }

    public class Option
    {
        public string Name { get => this.Attribute?.Name; }

        public string Alias { get => this.Attribute?.Alias;}

        public MemberInfo Property { get; set; }

        public OptionAttribute Attribute { get; set; }
    }



  
}
