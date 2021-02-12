using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace OnlineMarket.Models
{
    public class CallBack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string Contact { get; set; }
        public DateTime callTime = DateTime.Now;
        public DateTime CallTime
        {
            get { return callTime; }
            set { callTime = value; }

        }

        [BindNever]
        public bool Marked { get; set; }
    }
}
