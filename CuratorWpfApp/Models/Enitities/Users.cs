using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorWpfApp.Models.Enitities
{
    class Users
    {
        public int Id { get; 
            set; }
        public string Login { get; 
            set; }
        public string Password { get; 
            set; }
        public string Full_name { get; 
            set; }
        public string Group_name { get; 
            set; }
        public int Role_id {  get; 
            set; }
    }
}
