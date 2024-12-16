using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_System.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int orderId { get; set; }

        public DateTime orderDate { get; set; } = DateTime.Now;

        public decimal orderTotalAmount { get; set; }

        [ForeignKey("Buyer")]
        public int userId { get; set; }
        public virtual User Buyer { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<OrderProducts> OrderProducts { get; set; } = new List<OrderProducts>();
    }
}
