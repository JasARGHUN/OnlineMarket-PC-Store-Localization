using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineMarket.Models.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentCategory { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
