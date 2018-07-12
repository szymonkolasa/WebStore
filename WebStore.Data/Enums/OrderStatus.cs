using System.ComponentModel.DataAnnotations;

namespace WebStore.Data.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "New")]
        New,
        [Display(Name = "In preparation")]
        Preparation,
        [Display(Name = "Sent")]
        Sent,
        [Display(Name = "Cancelled")]
        Cancelled
    }
}
