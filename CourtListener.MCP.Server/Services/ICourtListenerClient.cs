namespace CourtListener.MCP.Server.Services;

/// <summary>
/// HTTP client interface for CourtListener API communication.
/// </summary>
public interface ICourtListenerClient
{
    /// <summary>
    /// Sends a GET request to the specified endpoint and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response to.</typeparam>
    /// <param name="endpoint">The API endpoint (relative to base URL).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deserialized response, or null if not found (404).</returns>
    Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request with JSON body to the specified endpoint and deserializes the response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the response to.</typeparam>
    /// <param name="endpoint">The API endpoint (relative to base URL).</param>
    /// <param name="request">The request body to serialize as JSON.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deserialized response, or null if not found (404).</returns>
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request with form-encoded data to the specified endpoint and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response to.</typeparam>
    /// <param name="endpoint">The API endpoint (relative to base URL).</param>
    /// <param name="formData">The form data to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deserialized response, or null if not found (404).</returns>
    Task<TResponse?> PostFormAsync<TResponse>(string endpoint, Dictionary<string, string> formData, CancellationToken cancellationToken = default);
}
