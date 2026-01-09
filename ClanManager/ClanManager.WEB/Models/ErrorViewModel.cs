namespace ClanManager.WEB.Models
{
    /// <summary>
    /// View model used to display error details to the end user while providing 
    /// tracking information for administrative diagnostics.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets the unique identifier for the current HTTP request. 
        /// This ID is used to correlate user-facing errors with server-side logs.
        /// </summary>
        public string? RequestId { get; } = null;

        /// <summary>
        /// Logic flag determining if the <see cref="RequestId"/> should be displayed in the UI.
        /// Returns true only if a valid RequestId is present.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorViewModel"/> with a specific request identifier.
        /// </summary>
        /// <param name="requestId">The unique ID of the request where the error occurred.</param>
        public ErrorViewModel(string? requestId)
        {
            RequestId = requestId;
        }
    }
}
