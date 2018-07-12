using System;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Data.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }
    }
}
