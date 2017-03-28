using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core
{
    public class StatisticsModel
    {
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string CountyName { get; set; }
        public string Name { get; set; }

        public int AllCount { get; set; }
        public int PassCount { get; set; }

        public string Link { get; set; }

        public int PeopleCount { get; set; }

        public string Key { get; set; }

        public string AreaId { get; set; }

        public int AreaType { get; set; }

        public List<StatisticsModel> Childrens { get; set; }
    }
}
