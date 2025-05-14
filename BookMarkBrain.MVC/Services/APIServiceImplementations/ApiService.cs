using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using System.Text;
using System.Text.Json;


namespace BookMarkBrain.MVC.Services.APIServiceImplementations;
public class ApiService : IApiService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<ApiService> _logger;

    public ApiService(IHttpClientFactory clientFactory, ILogger<ApiService> logger)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            var client = _clientFactory.CreateClient("API");
            _logger.LogInformation($"Making GET request to {client.BaseAddress}{endpoint}");

            var response = await client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error making GET request to {Endpoint}. Message: {Message}", endpoint, ex.Message);
            throw;
        }
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var client = _clientFactory.CreateClient("API");
            _logger.LogInformation($"Making POST request to {client.BaseAddress}{endpoint}");

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(endpoint, jsonContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error making POST request to {Endpoint}. Message: {Message}", endpoint, ex.Message);
            throw;
        }
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var client = _clientFactory.CreateClient("API");
            _logger.LogInformation($"Making PUT request to {client.BaseAddress}{endpoint}");

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync(endpoint, jsonContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error making PUT request to {Endpoint}. Message: {Message}", endpoint, ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var client = _clientFactory.CreateClient("API");
            _logger.LogInformation($"Making DELETE request to {client.BaseAddress}{endpoint}");

            var response = await client.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();

            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error making DELETE request to {Endpoint}. Message: {Message}", endpoint, ex.Message);
            throw;
        }
    }
}