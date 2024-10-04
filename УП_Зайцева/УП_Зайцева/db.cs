using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace УП_Зайцева
{
    internal class db
    {
        private readonly string connectionString;

        public db()
        {
            connectionString = @"Data Source=ADCLG1;Initial Catalog=!!!Зайцева_УП;Integrated Security=true";
        }

        public string GetConnectionString()
        {
            return connectionString;
        }
    }
}
