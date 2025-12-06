namespace ClanManager.WEB.Models.Clan
{
    public class ClanListViewModel
    {
        public List<ClanViewModel> Clans { get; set; } = new ();
        public bool ActiveOnly { get; set; } = true;
        public bool MyClansOnly { get; set; } = false;
        public bool IsLogedIn { get; set; } = false;
    }
}
