using IWantApp.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IWantApp.Endpoints.Clients;
public class ClientPost
{
    public static string Template => "/clients";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ClientRequest clientRequest, UserCreator creator)
    {
        var userClaims = new List<Claim>
        {
            new Claim("Cpf", clientRequest.Cpf),
            new Claim("Name", clientRequest.Name),

        };
        (IdentityResult identity, string id) result = await creator.Create(clientRequest.Email, clientRequest.Password, userClaims);

        if (!result.identity.Succeeded)
            return Results.ValidationProblem(result.identity.Errors.ConvertToProblemDetails());



        return Results.Created($"/clients/{result.id}", result.id);
    }
}
