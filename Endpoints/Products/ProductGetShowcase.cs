using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IWantApp.Endpoints.Products;

public class ProductGetShowcase
{
    public static string Template => "/products/showcase";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ApplicationDbContext context, string orderBy = "name", int page = 1, int row = 10)
    {

        if (row > 100)
            return Results.Problem(title: "Row Max is 100", statusCode: 400);

        var query = context.Products.AsNoTracking().Include(p => p.Category).Where(p => p.HasStock && p.Category.Active);

        if (orderBy == "name")
            query = query.OrderBy(p => p.Name);
        else if (orderBy == "price")
            query = query.OrderBy(p => p.Price);
        else
            return Results.Problem(title: "Order by only by price or name", statusCode: 400);


        var queryFilter = query.Skip((page - 1) * row).Take(row);
        var products = queryFilter.ToList();

        var results = products.Select(p => new ProductResponse(p.Id, p.Name, p.Category.Name, p.Description, p.HasStock, p.Price, p.Active));
        return Results.Ok(results);
    }
}
