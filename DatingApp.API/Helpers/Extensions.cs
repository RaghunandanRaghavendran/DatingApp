using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void ApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error",message);
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static int CalculateAge(this DateTime dateofBirth) 
        {
            var age = DateTime.Today.Year - dateofBirth.Year;
            if(dateofBirth.AddYears(age)> DateTime.Today)
            {
                age --;
            }
            return age;
        }

        public static void AddPagination(this HttpResponse response, int currentPage, 
                     int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var camelcaseFormatter = new JsonSerializerSettings();
            camelcaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader,camelcaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
    }
}