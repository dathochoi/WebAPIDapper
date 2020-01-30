using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Dtos
{
    public class PageResult<T>
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize {get;set;}
        public int TotalRow { get; set; }

        
    }
}
