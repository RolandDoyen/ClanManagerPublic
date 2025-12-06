namespace ClanManager.WEB.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; } = null;

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public ErrorViewModel(string? requestId)
        {
            RequestId = requestId;
        }
    }
}
