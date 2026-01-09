namespace ClanManager.Core
{
    /// <summary>
    /// Defines the hierarchical ranks within a specific clan, 
    /// determining member authorities and management capabilities.
    /// </summary>
    public enum ClanRole
    {
        /// <summary> The absolute owner of the clan with full management rights. </summary>
        ClanLeader,

        /// <summary> High-ranking officer capable of assisting the leader in clan operations. </summary>
        CoLeader,

        /// <summary> Trusted member with basic moderation. </summary>
        Elder,

        /// <summary> Standard confirmed member of the clan. </summary>
        Member,

        /// <summary> Newcomer or trial member. </summary>
        Recruit
    }

    /// <summary>
    /// Defines the global security roles for the application, 
    /// used for Role-Based Access Control (RBAC) and route protection.
    /// </summary>
    public enum Role
    {
        /// <summary> Highest authority with access to system settings and all user data. </summary>
        SuperAdmin,

        /// <summary> Administrative user capable of managing clans and moderating users. </summary>
        Admin,

        /// <summary> Standard registered user with access to public and clan features. </summary>
        User
    }
}
