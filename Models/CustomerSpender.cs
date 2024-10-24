using System.ComponentModel.DataAnnotations;

namespace ChinookApp.Models
{
    /// <summary>
    /// Represents a customer's spending information.
    /// </summary>
    public class CustomerSpender
    {
        /// <summary>
        /// Gets or sets the unique identifier for the customer spender.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the total amount spent by the customer.
        /// </summary>
        [Required]
        public decimal TotalSpent { get; set; }
    }
}
