using System.Net.Http.Json;
using ContosoInventory.Shared.DTOs;

namespace ContosoInventory.Client.Services;

public class ProductApiService
{
    private readonly HttpClient _httpClient;

    public ProductApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProductResponseDto>> GetAllProductsAsync(int? categoryId = null)
    {
        var url = categoryId.HasValue ? $"/api/products?categoryId={categoryId.Value}" : "/api/products";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>() ?? new();
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/api/products/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProductResponseDto>();
        }

        return null;
    }

    public async Task<ProductResponseDto?> CreateProductAsync(CreateProductDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/products", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProductResponseDto>();
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(error);
    }

    public async Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/products/{id}", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProductResponseDto>();
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(error);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/products/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<ProductResponseDto?> RestockProductAsync(int id, int quantity)
    {
        var dto = new RestockProductDto { Quantity = quantity };
        var response = await _httpClient.PostAsJsonAsync($"/api/products/{id}/restock", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProductResponseDto>();
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(error);
    }
}
