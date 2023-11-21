using IWantApp.Domain.Orders;
using IWantApp.Domain.Products;
using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IWantApp.Endpoints.Orders;
public class OrderPost
{
    public static string Template => "/orders";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "CpfPolicy")]
    public static async Task<IResult> Action(OrderRequest orderRequest, HttpContext http, ApplicationDbContext context)
    {
        var clientId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var clientName = http.User.Claims.First(c => c.Type == "Name").Value;


        List<Product> products = null;

        if (orderRequest.ProductIds.Any() || orderRequest.ProductIds != null)
            products = context.Products.Where(p => orderRequest.ProductIds.Contains(p.Id)).ToList();

        var order = new Order(clientId, clientName, products, orderRequest.DeliveryAddress);

        if (!order.IsValid)
        {
            return Results.ValidationProblem(order.Notifications.ConvertToProblemDetails());
        }

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        return Results.Created($"/clients/{order.Id}", order.Id);
    }
}
