using System.Linq;
using System.Reflection;

namespace NerdyMishka.Extensions.Console.Options
{
    public class Reflect
    {
        

        public static Noun MapOptions<T>()
        {
            var type = typeof(T);
            var attrs = type.GetCustomAttributes(true);
            var props = type.GetProperties();

            var noun = GetNoun(type, attrs, props);
            if(noun != null)
                return noun;


            // options type doesn't really require a noun or verb.  
            // as the cmdline program could only have options.  

            // For the purposes of maintaining a graph, we create
            // a default noun and default verb if one does not exist.
            noun = new Noun() {
                Attribute = new NounAttribute("_Default") { IsHidden = true }
            };

            var verb = GetVerb(type, attrs, props);
            if(verb == null)
                verb = GetDefaultVerb(type, attrs, props);

            noun.Verbs.Add(verb);

            return noun;
        }

        private static Noun GetNoun(Type clrType, object[] attrs = null, PropertyInfo[] props = null)
        {
            attrs = attrs ?? type.GetCustomAttributes(true);
            var nounAttr = attrs.Where(o => o is NounAttribute)
                                .Cast<NounAttribute>()
                                .SingleOrDefault();

            if(nounAttr == null)
                return null;

            var noun = new Noun() {
                ClrType = clrType,
                Attribute = nounAttr
            };

            var props = props ?? clrType.GetProperties();
            foreach(var prop in props)
            {
                var verb = GetVerb(prop.PropertyType);
                if(verb == null)
                    continue;

                verb.Property = prop;
                noun.Verbs.Add(verb);
            }
        }


        private static Verb GetDefaultVerb(Type clrType, object[] attrs = null, PropertyInfo[] props = null)
        {
            attrs = attrs ?? clrType.GetCustomAttributes(true);
           

            var verbAttr = new VerbAttribute("_Default") { IsDefault = true, IsHidden = true };
            var verb = new Verb(){
                ClrType = clrType,
                Attribute = verbAttr,
                
            };
            
            props = props ?? clrType.GetProperties();

            foreach(var prop in props) {
                var opts = GetOption(prop);
                if(opts != null)
                {
                    verb.Options.Add(opts);
                    continue;
                }
                var flag = GetFlag(prop);
                if(flag != null)
                {
                    verb.Options.Add(flag);
                    continue;
                }
                var r  = GetRemainingArgs(prop);
                verb.RemainingArgs = r;
               
                
            }

            return verb;
        }

        private static Verb GetVerb(Type clrType, object[] attrs = null, PropertyInfo[] props = null)
        {
            attrs = attrs ?? clrType.GetCustomAttributes(true);
            var verbAttr = attrs.Where(o => o is VerbAttribute)
                                .Cast<VerbAttribute>()
                                .SingleOrDefault();

            if(verbAttr == null)
                return null;

            var verb = new Verb(){
                ClrType = clrType,
                Attribute = verbAttr
            };
            
            props = props ?? clrType.GetProperties();

            foreach(var prop in props) {
                var opts = GetOption(prop);
                if(opts != null)
                {
                    verb.Options.Add(opts);
                    continue;
                }
                var flag = GetFlag(prop);
                if(flag != null)
                {
                    verb.Options.Add(flag);
                    continue;
                }
                var r  = GetRemainingArgs(prop);
                verb.RemainingArgs = r;
               
                
            }

            return verb;
        }


       


        private static RemainingArgs  GetRemainingArgs(MemberInfo member)
        {
            var attr = member.GetCustomAttributes(typeof(RemainingArgsAttribute), true)
                        .Cast<RemainingArgsAttribute>()
                        .SingleOrDefault();

            if(attr == null)
                return null;

            return new RemainingArgs() {
                Attribute = attr,
                Property = member
            };
        }

        private static Option GetOption(MemberInfo member)
        {
            var attr = member.GetCustomAttributes(typeof(OptionAttribute), true)
                        .Cast<OptionAttribute>()
                        .SingleOrDefault();

            if(attr == null)
                return null;

            return new Option() {
                Attribute = attr,
                Property = member
            };
        }

        private static Flag GetFlag(MemberInfo member)
        {
            var attr = member.GetCustomAttributes(typeof(OptionAttribute), true)
                        .Cast<FlagAttribute>()
                        .SingleOrDefault();
            
            if(attr == null)
                return null;

            return new Flag() {
                Attribute = attr,
                Property = member
            };
        }
    }
}
