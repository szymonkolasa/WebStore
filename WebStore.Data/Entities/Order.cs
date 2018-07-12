using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebStore.Data.Enums;

namespace WebStore.Data.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public ApplicationUser Purchaser { get; set; }

        public ApplicationUser Employee { get; set; }

        [Display(Name = "Creation date")]
        public DateTime CreationDate { get; set; }

        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        public string Street { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "House number")]
        public string HouseNumber { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Flat number")]
        public string FlatNumber { get; set; }

        [DataType(DataType.Text)]
        public string City { get; set; }

        [DataType(DataType.PostalCode)]
        [Display(Name = "Zip code")]
        public string ZipCode { get; set; }

        [DataType(DataType.Text)]
        public string Country { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Gift")]
        public bool IsGift { get; set; }

        public OrderStatus Status { get; set; }

        public List<OrderItem> Items { get; set; }
    }
}
