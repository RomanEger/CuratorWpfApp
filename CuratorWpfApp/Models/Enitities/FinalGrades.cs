using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorWpfApp.Models.Enitities
{
    class FinalGrades
    {
        public int Id { get; set; }
        public int Student_id { get; set; }
        public int Discipline_id { get; set; }
        public int Grade { get; set; }
        public int Semester { get; set; }
        public int Cource { get; set; }
    }
}
