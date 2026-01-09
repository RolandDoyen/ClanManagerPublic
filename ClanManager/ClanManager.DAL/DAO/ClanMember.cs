using ClanManager.Core;
using System.ComponentModel.DataAnnotations;

namespace ClanManager.DAL.DAO
{
    /// <summary>
    /// Data Access Object representing the many-to-many relationship between a <see cref="User"/> and a <see cref="Clan"/>.
    /// Stores specific membership details such as the user's role and joining date.
    /// </summary>
    public class ClanMember
    {
        /// <summary>
        /// Gets or sets the unique primary key for the membership record.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key referencing the associated clan.
        /// </summary>
        [Required]
        public Guid ClanId { get; set; }

        /// <summary>
        /// Navigation property to the <see cref="Clan"/> entity.
        /// </summary>
        public Clan Clan { get; set; } = null!;

        /// <summary>
        /// Gets or sets the foreign key referencing the associated user.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Navigation property to the <see cref="User"/> entity.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Gets or sets the hierarchical role assigned to the user within this specific clan.
        /// </summary>
        [Required]
        public ClanRole ClanRole { get; set; }

        /// <summary>
        /// Gets or sets the timestamp indicating when the user officially joined the clan.
        /// </summary>
        [Required]
        public DateTime JoinedAt { get; set; }
    }
}