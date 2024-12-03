using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class QueryParameters
    {
        private int _pageSize = 3;

        public int PageNumber { get; set; } = 1; // 預設為第 1 頁
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
        public int StartIndex
        {
            get
            {
                return (PageNumber - 1) * PageSize;
            }
        }
    }
}
