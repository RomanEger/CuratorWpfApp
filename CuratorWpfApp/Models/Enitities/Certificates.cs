using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorWpfApp.Models.Enitities
{
    public class Certificates
    {
        public int Id {  get; set; }
        public int Student_id { get; set; }
        public string Title { get; set; }

        private string startDate = string.Empty;
        public string Start_date
        {
            get { return startDate.Replace(" 00:00:00", ""); }
            set => startDate = value;
        }

        private string endDate;
        public string End_date
        {
            get { return endDate?.Replace(" 00:00:00", "") ?? null; }
            set => endDate = value;
        }
    }
}
