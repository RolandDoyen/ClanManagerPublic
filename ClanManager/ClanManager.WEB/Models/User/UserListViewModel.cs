namespace ClanManager.WEB.Models.User
{
    /// <summary>
    /// View model designed to aggregate multiple user profiles for display in a directory or administrative list.
    /// </summary>
    public class UserListViewModel
    {
        /// <summary>
        /// A collection of detailed user information records formatted for the list view.
        /// Defaults to an empty list to prevent null reference exceptions in the UI.
        /// </summary>
        public List<UserDetailViewModel> Users { get; set; } = new();
    }
}