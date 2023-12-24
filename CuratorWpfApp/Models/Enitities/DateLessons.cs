using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorWpfApp.Models.Enitities
{
    internal class DateLessons
    {
        public int Id { get; set; }
        private string dateL = string.Empty;
        public string DateL
        {
            get => dateL.Replace(" 00:00:00", ""); 
            set => dateL = value;
        }
    }
}
