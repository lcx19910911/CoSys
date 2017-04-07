using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core.Model
{
    public class StatisticsTotal
    {
        public List<StatisticsModel> List { get; set; }
        public string Name { get; set; }

        public int AllCount { get; set; }
        public int PassCount { get; set; }

        public int PeopleCount { get; set; }
    }
}
