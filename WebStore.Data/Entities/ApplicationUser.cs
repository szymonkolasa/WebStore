using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WebStore.Data.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public List<ShippingInformation> ShippingInformations { get; set; }
    }
}
