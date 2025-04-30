using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.ApplicationData
{
    class AppConnect
    {
        public static TaskManagerDBEntities modelOdb { get; set; }
        public static Users CurrentUser { get; set; }

        public static void Initialize()
        {
            modelOdb = new TaskManagerDBEntities();
        }
    }
}
