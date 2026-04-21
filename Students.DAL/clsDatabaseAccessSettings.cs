using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDataAccessLayer
{
    public class clsDatabaseAccessSettings
    {
        public static string _connectionString = Environment.GetEnvironmentVariable("STUDENT_DB_CONNECTION")
    ?? throw new InvalidOperationException("Database Connection String not found in Environment Variables!");

    }
}
