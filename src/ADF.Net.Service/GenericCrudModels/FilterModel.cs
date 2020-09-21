using System;

namespace ADF.Net.Service.GenericCrudModels
{
    public class FilterModel
    {

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int Status { get; set; }

        public string Searched { get; set; }

    }
}