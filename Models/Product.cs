using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication_GameStoreIL.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 5)]
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, 500)]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Image path")]
        public string imagePath { get; set; }
        public bool IsOnSale { get; set; }

        //one -> many : Category -> Product
        [Display(Name = "Category Id")]
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        //many <-> many : Cart <-> Product
        public List<Cart> Carts { get; set; }
    }
}
