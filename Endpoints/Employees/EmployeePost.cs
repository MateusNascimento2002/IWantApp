using IWantApp.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IWantApp.Endpoints.Employees;

public class EmployeePost
{
    public static string Template => "/employees";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(EmployeeRequest employeeResquest, UserCreator creator, HttpContext http)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userClaims = new List<Claim>
        {
            new Claim("EmployeeCode", employeeResquest.EmployeeCode),
            new Claim("Name", employeeResquest.Name),
            new Claim("CreatedBy", userId),
        };

        (IdentityResult identity, string id) result = await creator.Create(employeeResquest.Email, employeeResquest.Password, userClaims);

        if (!result.identity.Succeeded)
            return Results.ValidationProblem(result.identity.Errors.ConvertToProblemDetails());

        return Results.Created($"/employees/{result.id}", result.id);
    }
}
