using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace OnlineMarket.Models.ViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(List<Product> products, int? product, string name)
        {
            // устанавливаем начальный элемент, который позволит выбрать всех.
            products.Insert(0, new Product { Name = "All", Id = 0 });
            Products = new SelectList(products, "Id", "Name", product);
            SelectedProduct = product;
            SelectedName = name;
        }
        public SelectList Products { get; private set; } //список продуктов
        public int? SelectedProduct { get; private set; } //выбранный продукт
        public string SelectedName { get; private set; } //введенное имя 
        public string SelectedCategory { get; private set; } //введенное имя 
    }
}
