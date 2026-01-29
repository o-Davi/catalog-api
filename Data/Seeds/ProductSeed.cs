using CatalogApi.Data;
using CatalogApi.Domain.Entities;

namespace CatalogApi.Data.Seeds;
//Seed inicial para popular a tabela de Produtos
public static class ProductSeed
{
    public static void Seed(CatalogDbContext context)
    {
        if (context.Products.Any())
            return;

        var products = new List<Product>();

        for (int i = 1; i <= 50; i++)
        {
            products.Add(new Product
            {
                Name = $"Produto {i}",
                Price = Random.Shared.Next(50, 5000)
            });
        }

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
