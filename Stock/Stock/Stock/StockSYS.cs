using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Stock
{
    class StockSYS
    {
        static DateTime _sysDateTime;
        static DateTime sysDateTime
        {
            get
            {
                return _sysDateTime;
            }

        }
        public static void setSysDateTime(DateTime dateTime)
        {
            _sysDateTime = dateTime;
        }

    }
}
