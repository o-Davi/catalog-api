using CatalogApi.Common;
using CatalogApi.Dtos;
using CatalogApi.Dtos.Queries;
using CatalogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] ProductQueryParams query
    )
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<ProductDto>>.Ok(
            result,
            "Produtos listados com sucesso"
        ));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);

        if (product is null)
            return NotFound(
                ApiResponse<ProductDto>.Fail("Produto não encontrado")
            );

        return Ok(
            ApiResponse<ProductDto>.Ok(
                product,
                "Produto encontrado com sucesso"
            )
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var product = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            ApiResponse<ProductDto>.Ok(
                product,
                "Produto criado com sucesso"
            )
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);

        if (!updated)
            return NotFound(
                ApiResponse<bool>.Fail("Produto não encontrado")
            );

        return Ok(
            ApiResponse<bool>.Ok(true, "Produto atualizado com sucesso")
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound(
                ApiResponse<bool>.Fail("Produto não encontrado")
            );

        return Ok(
            ApiResponse<bool>.Ok(true, "Produto removido com sucesso")
        );
    }
}
