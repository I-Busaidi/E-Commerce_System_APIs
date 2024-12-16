using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_System.Models
{
    [PrimaryKey(nameof(orderId), nameof(productId))]
    public class OrderProducts
    {
        [ForeignKey("Order")]
        public int orderId { get; set; }
        public virtual Order Order { get; set; }

        [ForeignKey("Product")]
        public int productId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int productQuantity { get; set; }
    }
}
