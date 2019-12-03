using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public static class CommonExtensions
    {
        
        

        internal static IEnumerable<KeyValuePair<string, object>> ToParameters(this object obj)
        {
            if(obj is IDictionary<string, object>)
            {
                return (IDictionary<string, object>) obj;
            }

            var dictionary = new Dictionary<string, object>();
            var properties = obj.GetType().GetRuntimeProperties();

            foreach(var property in properties)
            {
                dictionary.Add(property.Name, property.GetValue(obj));
            }

            return dictionary;
        }

        public static void RunAndClose(this ISqlExecutor executor, Action<ISqlExecutor> invoke)
        {
            
            try
            {
                executor.OnNext(null);
                invoke(executor);
            }
            catch
            {
                executor.OnError(null);                
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }


        public static int Execute(this ISqlExecutor executor, SqlBuilder builder)
        {
            return Execute(executor, builder.ToString(true));
        }


        public static int Execute(this ISqlExecutor executor, string query)
        {
            try
            {

                var statement = new SqlStatementContext(query);
                executor.OnNext(statement);

                var result = statement.Command.Execute();

                return result;
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static int Execute(this ISqlExecutor executor, SqlBuilder builder, IList parameters)
        {
            return Execute(executor, builder.ToString(true), parameters);
        }

        public static int Execute(this ISqlExecutor executor, string query, IList parameters)
        {
            try
            {
                var statement = new SqlStatementContext(query);
                executor.OnNext(statement);


                return statement.Command.Execute();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }

        }

        public static int Execute(this ISqlExecutor executor, SqlBuilder builder, IDictionary parameters)
        {
            return Execute(executor, builder.ToString(true), parameters);
        }

        public static int Execute(this ISqlExecutor executor, string query, IDictionary parameters)
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement.Command.Execute();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static Task<int> ExecuteAsync(
            this ISqlExecutor executor, 
            SqlBuilder builder,
            CancellationToken token = default(CancellationToken))
        {
            return ExecuteAsync(executor, builder.ToString(true));
        }

        public static Task<int> ExecuteAsync(
            this ISqlExecutor executor, 
            string query,
            CancellationToken token = default(CancellationToken))
        {
            try
            {
                var statement = new SqlStatementContext(query);
                executor.OnNext(statement);


                return statement.Command.ExecuteAsync(token);
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }



        public static Task<int> ExecuteAsync(this ISqlExecutor executor, SqlBuilder builder, IDictionary parameters, CancellationToken token = default(CancellationToken))
        {
            return ExecuteAsync(executor, builder.ToString(true), parameters, token);
        }

        public static Task<int> ExecuteAsync(this ISqlExecutor executor, string query, IDictionary parameters, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement.Command.ExecuteAsync(token);
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static Task<int> ExecuteAsync(this ISqlExecutor executor, SqlBuilder sqlBuilder, IList parameters, CancellationToken token = default(CancellationToken))
        {
            return ExecuteAsync(executor, sqlBuilder.ToString(true), parameters, token);
        }

        public static Task<int> ExecuteAsync(this ISqlExecutor executor, string query, IList parameters, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement.Command.ExecuteAsync(token);
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

 
        public static int Execute(this ISqlExecutor executor, string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);

                return statement.Command.Execute();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }


        public static IDataReader FetchReader(this ISqlExecutor executor, SqlBuilder builder)
        {
            return FetchReader(executor, builder.ToString(true));
        }


        public static IDataReader FetchReader(this ISqlExecutor executor, string query)
        {

            try
            {
               
                var statement = new SqlStatementContext(query);
                executor.OnNext(statement);             

                return statement.Command.FetchReader();
              
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
           

        }


        public static IDataReader FetchReader(this ISqlExecutor executor, string query, object parameters)
        {

            try
            {
                var statement = new SqlStatementContext(query, parameters.ToParameters());
                executor.OnNext(statement);

                return statement.Command.FetchReader();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }

        }

     

        public static object FetchScalar(this ISqlExecutor executor, string query)
        {
            try
            {
                var statement = new SqlStatementContext(query);
                executor.OnNext(statement);

                return statement.Command.Fetch();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static T FetchValue<T>(this ISqlExecutor executor, SqlBuilder builder) where T: class 
        {
            return FetchValue<T>(executor, builder);
        }

        public static T FetchValue<T>(this ISqlExecutor executor, string query) where T: class 
        {
            try
            {
                var statement = new SqlStatementContext(query);
                executor.OnNext(statement);

               
                return statement.Command.Fetch<T>();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static T FetchValue<T>(this ISqlExecutor executor, SqlBuilder builder, IList parameters)
        {
            return FetchValue<T>(executor, builder, parameters);
        }

        public static T FetchValue<T>(this ISqlExecutor executor, string query, IList parameters)
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);

                return (T)statement
                    .Command
                    .Fetch();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static T FetchValue<T>(this ISqlExecutor executor, SqlBuilder builder, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return FetchValue<T>(executor, builder, parameters);
        }

        public static T FetchValue<T>(this ISqlExecutor executor, string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);

                return (T)statement
                    .Command
                    .Fetch();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

       

        

        public static IDataReader FetchReader(this ISqlExecutor executor, string query, params DbParameter[] parameters)
        {
            try
            {
                if (parameters == null)
                    parameters = Array.Empty<DbParameter>();
                
                var statement = new SqlStatementContext(query, (IEnumerable<DbParameter>)parameters);
                executor.OnNext(statement);

               
                return statement
                    .Command
                    .FetchReader();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static IDataReader FetchReader(this ISqlExecutor executor, string query, IList parameters)
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchReader();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static IDataReader FetchReader(this ISqlExecutor executor, string query, IDictionary parameters)
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchReader();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static IDataReader FetchReader(this ISqlExecutor executor, string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            try 
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchReader();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }



        public static Task<IDataReader> FetchReaderAsync(this ISqlExecutor executor, string query, CancellationToken token = default(CancellationToken), params DbParameter[] parameters)
        {
            try
            {
                IEnumerable<DbParameter> set = parameters;
                if (parameters == null)
                    parameters = Array.Empty<DbParameter>();
                

                var statement = new SqlStatementContext(query,  set);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchReaderAsync();
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            } 
        }

        public static Task<IDataReader> FetchReaderAsync(
            this ISqlExecutor sqlExecutor, 
            SqlBuilder sqlBuilder,
            CancellationToken token = default(CancellationToken))
        {
            return FetchReaderAsync(sqlExecutor, sqlBuilder.ToString(true));
        }

        public static Task<IDataReader> FetchReaderAsync(
            this ISqlExecutor executor, 
            string query,
             CancellationToken token = default(CancellationToken))
        {
            try
            {
                var statement = new SqlStatementContext(query);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchReaderAsync(token);
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static Task<IDataReader> FetchReaderAsync(this ISqlExecutor sqlExecutor, SqlBuilder sqlBuilder, IList parameters, CancellationToken token = default(CancellationToken))
        {
            return FetchReaderAsync(sqlExecutor, sqlBuilder.ToString(true), parameters, token);
        }

        public static Task<IDataReader> FetchReaderAsync(this ISqlExecutor executor, string query, IList parameters, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchReaderAsync(token);
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static Task<IDataReader> FetchReaderAsync(this ISqlExecutor sqlExecutor, SqlBuilder sqlBuilder, IDictionary parameters, CancellationToken token = default(CancellationToken))
        {
            return FetchReaderAsync(sqlExecutor, sqlBuilder.ToString(true), parameters, token);
        }

        public static Task<IDataReader> FetchReaderAsync(this ISqlExecutor executor, string query, IDictionary parameters, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchReaderAsync(token);
            }
            catch
            {
                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        

    
        public static Task<ICollection<T>> FetchCollectionAsync<T>(this ISqlExecutor executor, string query, params DbParameter[] parameters) where T : class
        {
            try
            {
                IEnumerable<DbParameter> set = parameters;
                #if (NET451 || NET45 || NET452 )
                    if (parameters == null)
                        parameters = new DbParameter[0];
                #else 
                    if (parameters == null)
                        parameters = Array.Empty<DbParameter>();
                #endif 

                var statement = new SqlStatementContext(query, set);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchCollectionAsync<T>();
            }
            catch
            {

                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
            
        }

        public static Task<ICollection<T>> FetchCollectionAsync<T>(this ISqlExecutor executor, string query, IEnumerable<KeyValuePair<string, object>> parameters) where T : class
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchCollectionAsync<T>();
            }
            catch
            {

                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static Task<ICollection<T>> FetchCollectionAsync<T>(this ISqlExecutor executor, string query, IDictionary parameters) where T : class
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchCollectionAsync<T>();
            }
            catch
            {

                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static Task<ICollection<T>> FetchCollectionAsync<T>(this ISqlExecutor executor, string query, IList parameters) where T : class
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchCollectionAsync<T>();
            }
            catch
            {

                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

    
        public static ICollection<T> FetchCollection<T>(this ISqlExecutor executor, string query, IEnumerable<KeyValuePair<string, object>> parameters) where T : class
        {

            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchCollection<T>();
            }
            catch
            {

                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }

          
        }

        public static ICollection<T> FetchCollection<T>(this ISqlExecutor executor, string query, params DbParameter[] parameters) where T : class
        {
            try
            {
                IEnumerable<DbParameter> set = parameters;
                #if (NET451 || NET45 || NET452 )
                    if (parameters == null)
                        parameters = new DbParameter[0];
                #else 
                    if (parameters == null)
                        parameters = Array.Empty<DbParameter>();
                #endif 

                var statement = new SqlStatementContext(query, set);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchCollection<T>();
            }
            catch
            {

                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        public static ICollection<T> FetchCollection<T>(this ISqlExecutor executor, string query, IDictionary parameters) where T : class
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchCollection<T>();
            }
            catch
            {

                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }


        }

        public static ICollection<T> FetchCollection<T>(this ISqlExecutor executor, string query, IList parameters) where T : class
        {
            try
            {
                var statement = new SqlStatementContext(query, parameters);
                executor.OnNext(statement);


                return statement
                    .Command
                    .FetchCollection<T>();
            }
            catch
            {

                executor.OnError(null);
                throw;
            }
            finally
            {
                executor.OnCompleted();
            }
        }

        
    }
}
