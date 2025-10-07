using System.Diagnostics;
using System.Net;
using System.Text.Json;
using CourtListener.MCP.Server.Configuration;
using Microsoft.Extensions.Options;

namespace CourtListener.MCP.Server.Services;

/// <summary>
/// HTTP client implementation for CourtListener API communication with logging and error handling.
/// </summary>
public class CourtListenerClient : ICourtListenerClient
{
    private readonly HttpClient _httpClient;
    private readonly CourtListenerOptions _options;
    private readonly ILogger<CourtListenerClient> _logger;

    public CourtListenerClient(
        HttpClient httpClient,
        IOptions<CourtListenerOptions> options,
        ILogger<CourtListenerClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "HTTP GET request to {Endpoint}",
                endpoint
            );

            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            stopwatch.Stop();

            // 404 is a valid response (not found)
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation(
                    "HTTP GET {Endpoint} returned 404 Not Found (Duration: {Duration}ms)",
                    endpoint,
                    stopwatch.ElapsedMilliseconds
                );
                return default;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<TResponse>(content, JsonSerializerConfig.Options);

            _logger.LogInformation(
                "HTTP GET {Endpoint} completed successfully (Status: {StatusCode}, Duration: {Duration}ms, Size: {Size} bytes)",
                endpoint,
                (int)response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                content.Length
            );

            return result;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "HTTP GET {Endpoint} failed (Duration: {Duration}ms, Error: {Error})",
                endpoint,
                stopwatch.ElapsedMilliseconds,
                ex.Message
            );
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // Timeout
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "HTTP GET {Endpoint} timed out after {Duration}ms",
                endpoint,
                stopwatch.ElapsedMilliseconds
            );
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "HTTP POST request to {Endpoint}",
                endpoint
            );

            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            stopwatch.Stop();

            // 404 is a valid response (not found)
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation(
                    "HTTP POST {Endpoint} returned 404 Not Found (Duration: {Duration}ms)",
                    endpoint,
                    stopwatch.ElapsedMilliseconds
                );
                return default;
            }

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<TResponse>(responseContent, JsonSerializerConfig.Options);

            _logger.LogInformation(
                "HTTP POST {Endpoint} completed successfully (Status: {StatusCode}, Duration: {Duration}ms, Size: {Size} bytes)",
                endpoint,
                (int)response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                responseContent.Length
            );

            return result;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "HTTP POST {Endpoint} failed (Duration: {Duration}ms, Error: {Error})",
                endpoint,
                stopwatch.ElapsedMilliseconds,
                ex.Message
            );
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // Timeout
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "HTTP POST {Endpoint} timed out after {Duration}ms",
                endpoint,
                stopwatch.ElapsedMilliseconds
            );
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TResponse?> PostFormAsync<TResponse>(string endpoint, Dictionary<string, string> formData, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "HTTP POST (form) request to {Endpoint}",
                endpoint
            );

            var content = new FormUrlEncodedContent(formData);

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            stopwatch.Stop();

            // 404 is a valid response (not found)
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation(
                    "HTTP POST (form) {Endpoint} returned 404 Not Found (Duration: {Duration}ms)",
                    endpoint,
                    stopwatch.ElapsedMilliseconds
                );
                return default;
            }

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<TResponse>(responseContent, JsonSerializerConfig.Options);

            _logger.LogInformation(
                "HTTP POST (form) {Endpoint} completed successfully (Status: {StatusCode}, Duration: {Duration}ms, Size: {Size} bytes)",
                endpoint,
                (int)response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                responseContent.Length
            );

            return result;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "HTTP POST (form) {Endpoint} failed (Duration: {Duration}ms, Error: {Error})",
                endpoint,
                stopwatch.ElapsedMilliseconds,
                ex.Message
            );
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // Timeout
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "HTTP POST (form) {Endpoint} timed out after {Duration}ms",
                endpoint,
                stopwatch.ElapsedMilliseconds
            );
            throw;
        }
    }
}
