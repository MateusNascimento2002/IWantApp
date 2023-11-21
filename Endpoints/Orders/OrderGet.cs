using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IWantApp.Endpoints.Orders;

public class OrderGet
{
    public static string Template => "/orders/{id:Guid}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action(Guid id, HttpContext http, ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        var clientClaim = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
        var employeeClaim = http.User.Claims.FirstOrDefault(c => c.Type == "EmployeeCode");

        var order = context.Orders.Include(o => o.Products).FirstOrDefault(o => o.Id == id);

        var client = await userManager.FindByIdAsync(order.ClientId);

        if (order.ClientId != clientClaim.Value && employeeClaim == null)
            return Results.Forbid();

        var productsResponse = order.Products.Select(p => new OrderProduct(p.Id, p.Name));
        var orderResponse = new OrderResponse(order.Id, client.Email, productsResponse, order.DeliveryAddress);

        return Results.Ok(orderResponse);
    }
}
