using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime LastActiveDate { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public List<PaymentType> PaymentTypes { get; set; } = new List<PaymentType>();
    }
}