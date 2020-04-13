using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(List<T> items, int count , int pageNumber, int PageSize)
        {
            this.CurrentPage = pageNumber;           
            this.PageSize = PageSize;
            this.TotalCount = count;
            this.TotalPage = (int)Math.Ceiling(count/(double)PageSize);
            this.AddRange(items);
        }
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pagesize)
        {
          var count = await source.CountAsync();
          var items = await source.Skip((pageNumber-1)*pagesize).Take(pagesize).ToListAsync();
          return new PagedList<T>(items, count, pageNumber,pagesize);
        }
    }
}