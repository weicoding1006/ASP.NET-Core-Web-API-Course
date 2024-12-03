using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Dtos.QueryParameters
{
    public class QueryParametersDto
    {
        private int _pageSize = 3;

        public int PageNumber { get; set; } = 1; // 預設為第 1 頁
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
    }
}
