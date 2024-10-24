using System.ComponentModel.DataAnnotations;

namespace ChinookApp.Models
{
    /// <summary>
    /// Represents a customer's country information.
    /// </summary>
    public class CustomerCountry
    {
        /// <summary>
        /// Gets or sets the unique identifier for the customer country.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the count of customers from this country.
        /// </summary>
        public int CustomerCount { get; set; }
    }
}

