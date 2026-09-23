using Microsoft.EntityFrameworkCore;
using ContosoInventory.Server.Data;
using ContosoInventory.Server.Models;
using ContosoInventory.Shared.DTOs;

namespace ContosoInventory.Server.Services;

/// <summary>
/// Provides operations for managing inventory products.
/// </summary>
public class ProductService : IProductService
{
    private readonly InventoryContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(InventoryContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<ProductResponseDto>> GetAllProductsAsync(int? categoryId = null)
    {
        try
        {
            IQueryable<Product> query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query
                .OrderBy(p => p.Name)
                .ToListAsync();

            return products.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all products.");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? null : MapToDto(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}.", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
    {
        try
        {
            var normalizedSku = NormalizeSku(dto.Sku);
            dto.Sku = normalizedSku;

            ValidateProductInput(dto.Name, dto.Sku, dto.Description, dto.Price, dto.StockQuantity, dto.CategoryId);

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
            {
                throw new InvalidOperationException("The specified category does not exist.");
            }

            var skuExists = await _context.Products
                .AnyAsync(p => p.Sku == dto.Sku);

            if (skuExists)
            {
                throw new InvalidOperationException($"A product with the SKU '{dto.Sku}' already exists.");
            }

            var product = new Product
            {
                Name = dto.Name.Trim(),
                Sku = dto.Sku,
                Description = dto.Description.Trim(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product created: {ProductName} (ID: {ProductId}).", product.Name, product.Id);

            return MapToDto(product);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product '{ProductName}'.", dto.Name);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        try
        {
            var normalizedSku = NormalizeSku(dto.Sku);
            dto.Sku = normalizedSku;

            ValidateProductInput(dto.Name, dto.Sku, dto.Description, dto.Price, dto.StockQuantity, dto.CategoryId);

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return null;
            }

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
            {
                throw new InvalidOperationException("The specified category does not exist.");
            }

            var skuExists = await _context.Products
                .AnyAsync(p => p.Sku == dto.Sku && p.Id != id);

            if (skuExists)
            {
                throw new InvalidOperationException($"A product with the SKU '{dto.Sku}' already exists.");
            }

            product.Name = dto.Name.Trim();
            product.Sku = dto.Sku;
            product.Description = dto.Description.Trim();
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;
            product.LastUpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Product updated: {ProductName} (ID: {ProductId}).", product.Name, product.Id);

            return MapToDto(product);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {ProductId}.", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product deleted: {ProductName} (ID: {ProductId}).", product.Name, product.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID {ProductId}.", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto?> RestockProductAsync(int id, RestockProductDto dto)
    {
        try
        {
            if (dto.Quantity <= 0)
            {
                throw new InvalidOperationException("Restock quantity must be greater than zero.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return null;
            }

            if (product.StockQuantity > int.MaxValue - dto.Quantity)
            {
                throw new InvalidOperationException("Restock quantity would exceed the maximum stock value.");
            }

            product.StockQuantity += dto.Quantity;
            product.LastUpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Product restocked: {ProductName} (ID: {ProductId}) by {Quantity} units.",
                product.Name,
                product.Id,
                dto.Quantity);

            return MapToDto(product);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restocking product with ID {ProductId}.", id);
            throw;
        }
    }

    private static string NormalizeSku(string sku)
    {
        return sku.Trim().ToUpperInvariant();
    }

    private static void ValidateProductInput(string name, string sku, string description, decimal price, int stockQuantity, int categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Product name is required.");
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new InvalidOperationException("Product SKU is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new InvalidOperationException("Product description is required.");
        }

        if (price <= 0)
        {
            throw new InvalidOperationException("Product price must be greater than zero.");
        }

        if (stockQuantity < 0)
        {
            throw new InvalidOperationException("Stock quantity cannot be negative.");
        }

        if (categoryId <= 0)
        {
            throw new InvalidOperationException("A valid category must be selected.");
        }
    }

    private static ProductResponseDto MapToDto(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            CreatedDate = product.CreatedDate,
            LastUpdatedDate = product.LastUpdatedDate
        };
    }
}
