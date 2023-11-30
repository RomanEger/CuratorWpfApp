using CuratorWpfApp.Models.Enitities;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorWpfApp.Models.ServicesDB
{
    internal class SqlStudents
    {
        private string conStr = ConfigStr.ConStr;

        public async Task<IEnumerable<Students>> GetStudentsByGroupAsync(string groupName)
        {
            using(IDbConnection db = new SqlConnection(conStr))
            {
                var list = await db.QueryAsync<Students>($"SELECT * FROM StudentsT WHERE Group_Name='{groupName}'");
                return list;
            }
        }
    }
}
