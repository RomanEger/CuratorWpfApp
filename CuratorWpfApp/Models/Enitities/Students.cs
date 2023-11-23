using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorWpfApp.Models.Enitities
{
    class Students
    {
        public int Id { get; set; } 
        public string FullName { get; set; }
        public string Birthday { get; set; }
        public string GroupName { get; set; }
        public string Photo {  get; set; }
        public bool HasMilitaryId { get; set; }
        public string? CertificateName { get; set; }
        public string? CertificateStartDate { get; set; }
        public string? CertificateEndDate { get; set; }
    }
}
