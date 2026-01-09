namespace ClanManager.WEB.Models.Clan
{
    /// <summary>
    /// View model representing the clan directory page, including search filters and user-specific view states.
    /// </summary>
    public class ClanListViewModel
    {
        /// <summary>
        /// Gets or sets the collection of clan summaries to be displayed in the list.
        /// </summary>
        public List<ClanViewModel> Clans { get; set; } = new();

        /// <summary>
        /// Filter flag indicating whether the list should only show clans currently marked as active.
        /// </summary>
        public bool ActiveOnly { get; set; } = true;

        /// <summary>
        /// Filter flag indicating whether the results should be restricted to clans the current user is a member of.
        /// </summary>
        public bool MyClansOnly { get; set; } = false;

        /// <summary>
        /// Contextual flag used by the view to determine which UI elements (like "Join" buttons) are accessible based on the session state.
        /// </summary>
        public bool IsLogedIn { get; set; } = false;
    }
}