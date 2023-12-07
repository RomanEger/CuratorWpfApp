using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorWpfApp.Models.Enitities
{
    class CertificatesTable
    {
        public int Id { get; set; }
        public string Full_name { get; set; }
        public string Title { get; set; }
        public string Start_date { get; set; }
        public string? End_date { get; set; }
    }
}
