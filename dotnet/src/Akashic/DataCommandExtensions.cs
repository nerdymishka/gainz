using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public static class DataCommandExtensions
    {


        
        public static IDataCommand Configure(this IDataCommand cmd, string query, IList parameters)
        {
            return Configure(cmd, AkashicSettings.AdoParameterPrefix, query, parameters);
        }


        public static IDataCommand Configure(this IDataCommand cmd, char prefix, string query, IList parameters)
        {
            var sql = query;

            if (parameters != null && parameters.Count > 0)
            {
                int index = 0;
                sql = Regex.Replace(sql, "?", (m) =>
                {
                    var name = prefix + index.ToString();
                    cmd.AddParameter(name, parameters[index]);
                    index++;

                    return name;
                });
            }

            cmd.Text = sql;
            cmd.Type = System.Data.CommandType.Text;

            return cmd;
        }


        public static IDataCommand Configure(this IDataCommand cmd, string query, object parameters)
        {
            return Configure(cmd, AkashicSettings.AdoParameterPrefix, query, parameters);
        }

        public static IDataCommand Configure(IDataCommand cmd, char prefix, string query, object parameters)
        {
            cmd.Type = System.Data.CommandType.Text;
            var sql = query;
            var properties = parameters.GetType().GetRuntimeProperties();

            bool replace = AkashicSettings.ParameterPrefix != prefix;

            if (parameters != null)
            {
                foreach (var prop in properties)
                {
                    var key = prop.Name;
                    var name = prefix + key;
                    cmd.AddParameter(name, prop.GetValue(parameters));
                    if(replace)
                        sql = sql.Replace(AkashicSettings.ParameterPrefix + key, name);
                }
            }

            cmd.Text = sql;
            return cmd;
        }


        public static IDataCommand Configure(this IDataCommand cmd, string query, params DbParameter[] parameters)
        {
            return Configure(cmd, AkashicSettings.AdoParameterPrefix, query, parameters);
        }

        public static IDataCommand Configure(IDataCommand cmd, char prefix, string query, params DbParameter[] parameters)
        {
            cmd.Type = System.Data.CommandType.Text;
            var sql = query;

            bool replace = AkashicSettings.ParameterPrefix != prefix;            

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    if (replace)
                    {
                        var key = parameter.ParameterName;
                        if(key[0] == AkashicSettings.ParameterPrefix)
                        {
                            parameter.ParameterName = prefix + key.Substring(1);
                            sql = sql.Replace(key, parameter.ParameterName);
                        }
                    }
                      
                    cmd.Parameters.Add(parameter);
                }
            }

            cmd.Text = sql;
            return cmd;
        }

        public static IDataCommand Configure(this IDataCommand cmd, string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return Configure(cmd, AkashicSettings.AdoParameterPrefix, query, parameters);
        }

       

        public static IDataCommand Configure(IDataCommand cmd, char prefix, string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            cmd.Type = System.Data.CommandType.Text;
            var sql = query;

            bool replace = AkashicSettings.ParameterPrefix != prefix;

            if (parameters != null)
            {
                foreach (var pair in parameters)
                {
                    var key = pair.Key;
                    var name = prefix + key;
                    cmd.AddParameter(name, pair.Value);
                    if(replace)
                        sql = sql.Replace(AkashicSettings.ParameterPrefix + key, name);
                }
            }

            cmd.Text = sql;
            return cmd;
        }
        public static IDataCommand Configure(this IDataCommand cmd, string query, IDictionary parameters)
        {
            return Configure(cmd, AkashicSettings.AdoParameterPrefix, query, parameters);
        }

        public static IDataCommand Configure(IDataCommand cmd, char prefix, string query, IDictionary parameters)
        {
            cmd.Type = System.Data.CommandType.Text;
            var sql = query;

            bool replace = AkashicSettings.ParameterPrefix != prefix;

            if (parameters != null && parameters.Count > 0)
            {
                foreach (string key in parameters.Keys)
                {
                    var name = prefix + key;
                    var value = parameters[key];
                    cmd.AddParameter(name, value);
                    if(replace)
                        sql = sql.Replace(AkashicSettings.ParameterPrefix + key, name);
                }
            }

            cmd.Text = sql;
            return cmd;
        }
    }
}
