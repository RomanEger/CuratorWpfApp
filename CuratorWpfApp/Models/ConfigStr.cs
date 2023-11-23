using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorWpfApp.Models
{
    class ConfigStr
    {
        public static string ConStr { get { 
                return "Data Source=DESKTOP-LEU7Q9N;Initial Catalog=CuratorDB;Integrated Security=True;" +
                    "Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;" +
                    "Multi Subnet Failover=False"; } }
    }
}
