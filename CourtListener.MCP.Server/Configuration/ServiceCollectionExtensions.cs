using System.Net;
using CourtListener.MCP.Server.Services;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace CourtListener.MCP.Server.Configuration;

/// <summary>
/// Extension methods for IServiceCollection to register CourtListener services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the CourtListener HTTP client with Polly resilience policies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCourtListenerClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind CourtListenerOptions
        services.Configure<CourtListenerOptions>(configuration.GetSection("CourtListener"));

        // Add typed HTTP client with Polly policies
        services.AddHttpClient<ICourtListenerClient, CourtListenerClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<CourtListenerOptions>>().Value;
            var logger = sp.GetService<ILogger<CourtListenerClient>>();

            // Configure base address and timeout
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);

            // Add authorization header (Token format for CourtListener)
            if (!string.IsNullOrEmpty(options.ApiKey))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Token {options.ApiKey}");
                logger?.LogInformation("API Key configured (ends with: ...{Last4})", options.ApiKey.Substring(Math.Max(0, options.ApiKey.Length - 4)));
            }
            else
            {
                logger?.LogWarning("No API key configured!");
            }

            // Add user agent
            client.DefaultRequestHeaders.Add("User-Agent", "CourtListener-MCP-DotNet/1.0");
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    /// <summary>
    /// Gets the Polly retry policy for transient HTTP failures.
    /// Retries 3 times with exponential backoff (2s, 4s, 8s).
    /// Respects 429 rate limit Retry-After headers.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles 5xx and 408
            .Or<HttpRequestException>() // Network failures
            .OrResult(msg => msg.StatusCode == (HttpStatusCode)429) // Rate limit
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryAttempt, outcome, context) =>
                {
                    // Handle 429 rate limit with Retry-After header
                    if (outcome.Result?.StatusCode == (HttpStatusCode)429)
                    {
                        var retryAfter = outcome.Result.Headers.RetryAfter;

                        // Check for Delta (duration)
                        if (retryAfter?.Delta != null)
                        {
                            return retryAfter.Delta.Value;
                        }

                        // Check for Date (absolute time)
                        if (retryAfter?.Date != null)
                        {
                            var delay = retryAfter.Date.Value - DateTimeOffset.UtcNow;
                            return delay > TimeSpan.Zero ? delay : TimeSpan.FromSeconds(60);
                        }

                        // Default rate limit delay
                        return TimeSpan.FromSeconds(60);
                    }

                    // Exponential backoff: 2s, 4s, 8s
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                },
                onRetryAsync: (outcome, timespan, retryAttempt, context) =>
                {
                    // Log retry attempts (logger will be available via context in production)
                    var statusCode = outcome.Result?.StatusCode ?? HttpStatusCode.InternalServerError;

                    Console.WriteLine(
                        $"[Polly Retry] Attempt {retryAttempt} after {timespan.TotalSeconds:F1}s delay (Status: {(int)statusCode} {statusCode})"
                    );

                    return Task.CompletedTask;
                }
            );
    }

    /// <summary>
    /// Gets the Polly circuit breaker policy.
    /// Opens circuit after 5 consecutive failures for 60 seconds.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<HttpRequestException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(60),
                onBreak: (outcome, duration) =>
                {
                    var statusCode = outcome.Result?.StatusCode ?? HttpStatusCode.InternalServerError;

                    Console.WriteLine(
                        $"[Polly Circuit Breaker] Circuit OPENED due to {(int)statusCode} {statusCode}. Breaking for {duration.TotalSeconds}s."
                    );
                },
                onReset: () =>
                {
                    Console.WriteLine("[Polly Circuit Breaker] Circuit CLOSED. Resuming normal operations.");
                },
                onHalfOpen: () =>
                {
                    Console.WriteLine("[Polly Circuit Breaker] Circuit HALF-OPEN. Testing if service recovered.");
                }
            );
    }
}
