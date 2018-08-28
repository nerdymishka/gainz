using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka
{
    public interface IApplicationEnvironment
    {
        string ResolvePath(string relativePath, params string[] segments);

        string ApplicationName { get; set; }

        string EnvironmentName { get; set; }

        object this[string key] { get;  set; }
    }
}
