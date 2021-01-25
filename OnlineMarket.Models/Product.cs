using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarket.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field can't be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field can't be empty")]
        public string Description { get; set; }

        [Required(ErrorMessage = "This field can't be empty")]
        public string Manufacturer { get; set; }

        [Required]
        [Range(1, 10000)]
        public double ListPrice { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }

        public string ImageUrl { get; set; }


        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        // Product Description
        [Required]
        public string Type { get; set; }
        [Required]
        public string Processor { get; set; }
        [Required]
        public string RAM { get; set; }
        [Required]
        public string PowerSupply { get; set; }
        [Required]
        public string StorageDevice { get; set; }
        [Required]
        public string VideoCard { get; set; }
        [Required]
        public string OperatingSystem { get; set; }
    }
}
