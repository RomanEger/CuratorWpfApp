using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorWpfApp.Models.Enitities
{
    class Grades
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int DisciplineId {  get; set; }
        public int Grade {  get; set; }
    }
}
