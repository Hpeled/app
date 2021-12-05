using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication_GameStoreIL.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double TotalPrice { get; set; } = 0;

        //many <-> many : Cart <-> Product 
        public List<Product> Products { get; set; }

        internal static object Index()
        {
            throw new NotImplementedException();
        }
    }
}
