using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IWantApp.Endpoints.Products;

public class ProductGetById
{
    public static string Template => "/products/{id:guid}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] Guid id, ApplicationDbContext context)
    {
        var product = context.Products.FirstOrDefault(p => p.Id == id);
        return Results.Ok(product);
    }
}
