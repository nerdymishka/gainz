using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public interface ICreateCommand
    {
        IDataCommand CreateCommand();
    }
}
