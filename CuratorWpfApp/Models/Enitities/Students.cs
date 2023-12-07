using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CuratorWpfApp.Models.Enitities
{
    public class Students
    {
        public int Id { get; set; } 
        public string Full_name { get; set; }
        private string birthday;
        public string Birthday {
            get { return birthday.Replace(" 00:00:00", ""); }
            set => birthday = value;
        }
        public string Group_name { get; set; }
        public byte[] Photo {  get; set; }
        public bool Has_Military_id { get; set; }
    }
}
