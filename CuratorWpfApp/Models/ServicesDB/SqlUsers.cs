using CuratorWpfApp.Models.Enitities;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CuratorWpfApp.Models.ServicesDB
{
    class SqlUsers
    {
        private string conStr = ConfigStr.ConStr;

        public async Task<Users> GetUserByIdAsync(int id)
        {
            using(IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryFirstOrDefaultAsync<Users>($"SELECT * FROM UsersT WHERE Id={id}") 
                    ?? throw new Exception("Такого пользователя нет");
            }
        }

        public async Task<Users> GetUserByLoginAsync(string login)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryFirstOrDefaultAsync<Users>($"SELECT * FROM UsersT WHERE Login='{login}'")
                    ?? throw new Exception("Такого пользователя нет");
            }
        }

        public async Task<int?> LoginAsync(string login, string password)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryFirstOrDefaultAsync<int?>($"SELECT Role_id FROM UsersT WHERE Login='{login}' AND Password='{password}'")
                    ?? throw new Exception("Некорректный логин или пароль");
            }
        }
    }
}
