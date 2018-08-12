using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace NerdyMishka.Data
{
    public class DbParameterList : IEnumerable<DbParameter>
    {
        private List<DbParameter> parameters = new List<DbParameter>();


        public DbParameterList()
        {

        }



        public void Add(DbParameter parameter)
        {
            this.parameters.Add(parameter);
        }

        public IEnumerator<DbParameter> GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }
    }
}
