using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public double Total { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int PaymentTypeId { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
