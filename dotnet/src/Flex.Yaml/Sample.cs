using System;

namespace NerdyMishka.Flex.Yaml
{
    public class Sample 
    {

        [Symbol("build")]
        public BuildConfig Build { get; set; }

        public class BuildConfig
        {
            [Switch]
            [Symbol("disabled")]
            public bool Disabled { get; set; } 


            [Symbol("buildName")]
            public string BuildName { get; set; }

            [Symbol("var")]
            [Encrypt]
            public string Var { get; set; }
        }
    }
}
