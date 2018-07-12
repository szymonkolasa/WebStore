using System;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Data.Entities
{
    public class ShippingInformation
    {
        [Key]
        public Guid Id { get; set; }

        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        public string Street { get; set; }

        [DataType(DataType.Text)]
        public string HouseNumber { get; set; }

        [DataType(DataType.Text)]
        public string FlatNumber { get; set; }

        [DataType(DataType.Text)]
        public string City { get; set; }

        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }

        [DataType(DataType.Text)]
        public string Country { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }
}
