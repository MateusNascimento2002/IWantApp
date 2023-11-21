using IWantApp.Endpoints.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IWantApp.Endpoints.Products;

public class EmployeeGetById
{
    public static string Template => "/employees/{id:guid}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] Guid id, UserManager<IdentityUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        var claims = await userManager.GetClaimsAsync(user);
        var name = claims.FirstOrDefault(c => c.Type == "Name");
        var response = new EmployeeResponse(user.UserName, name.Value);
        return Results.Ok(response);
    }
}
