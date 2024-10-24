using System.ComponentModel.DataAnnotations;

namespace ChinookApp.Models
{
    /// <summary>
    /// Represents a customer's genre preference.
    /// </summary>
    public class CustomerGenre
    {
        /// <summary>
        /// Gets or sets the unique identifier for the customer genre.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the genre.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the count of customers who prefer this genre.
        /// </summary>
        public int CustomerCount { get; set; }
        /// <summary>
        /// Gets or sets the count of purchases for this genre.
        /// </summary>
        public int PurchaseCount { get; set; }

        /// <summary>
        /// Gets or sets the count of tracks for this genre.
        /// </summary>
        public int TrackCount { get; set; }
    }
}
