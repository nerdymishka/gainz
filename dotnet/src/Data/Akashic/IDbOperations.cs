using System;

namespace NerdyMishka.Data 
{
    public interface IDbOperationResult
    {
        bool Ok { get;  }

        bool IsSupported { get;  }

        Exception Exception { get;  }
    }

    public interface IDbOperations
    {
         IDbOperationResult CreateDatabase(string name, object options = null);

         IDbOperationResult CreateUser(string name, object options = null);

         IDbOperationResult DropUser(string name, object options = null);

         IDbOperationResult CreateRole(string name, object options = null);

         IDbOperationResult DropRole(string name, object options = null);

         IDbOperationResult DropDatabase(string name, object options = null);
    }
    
}