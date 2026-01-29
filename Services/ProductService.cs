using CatalogApi.Common;
using CatalogApi.Data;
using CatalogApi.Domain.Entities;
using CatalogApi.Dtos;
using CatalogApi.Dtos.Queries;
using CatalogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Services;

public class ProductService : IProductService
{
    private readonly CatalogDbContext _context;

    public ProductService(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> GetAllAsync(ProductQueryParams query)
    {
        var productsQuery = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
            productsQuery = productsQuery
                .Where(p => p.Name.ToLower().Contains(query.Name.ToLower()));

        if (query.MinPrice.HasValue)
            productsQuery = productsQuery
                .Where(p => p.Price >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            productsQuery = productsQuery
                .Where(p => p.Price <= query.MaxPrice.Value);

        var sortBy = query.SortBy?.Trim().ToLower();
        var sortDirection = query.SortDirection?.Trim().ToLower();

        productsQuery = sortBy switch
        {
            "name" => sortDirection == "desc"
                ? productsQuery.OrderByDescending(p => p.Name)
                : productsQuery.OrderBy(p => p.Name),

            "price" => sortDirection == "desc"
                ? productsQuery.OrderByDescending(p => p.Price)
                : productsQuery.OrderBy(p => p.Price),

            _ => productsQuery.OrderByDescending(p => p.Id)
        };

        var totalItems = await productsQuery.CountAsync();

        var items = await productsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })
            .ToListAsync();

        return new PagedResult<ProductDto>
        {
            Data = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.Name = dto.Name;
        product.Price = dto.Price;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
