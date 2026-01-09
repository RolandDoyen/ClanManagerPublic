using System.ComponentModel.DataAnnotations;

namespace ClanManager.DAL.DAO
{
    /// <summary>
    /// Data Access Object representing a Clan entity in the database.
    /// This entity serves as the primary organizational unit for grouping users.
    /// </summary>
    public class Clan
    {
        /// <summary>
        /// Gets or sets the unique primary key for the clan.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the official name of the clan. Maximum length is 100 characters.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the unique identifier tag (shorthand) for the clan. Maximum length is 20 characters.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Tag { get; set; } = null!;

        /// <summary>
        /// Gets or sets an optional descriptive text about the clan's goals or requirements.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the clan is currently active and visible in search results.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the collection of membership records linking users to this clan.
        /// Initializes as an empty list to avoid null reference exceptions.
        /// </summary>
        public List<ClanMember> Members { get; set; } = new();
    }
}