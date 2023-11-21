using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Authorization;

namespace IWantApp.Endpoints.Employees;

public class ProductGetSoldReport
{
    public static string Template => "/products/soldReport";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(QueryAllProdutsSold query)
    {
        var result = await query.Execute();
        return Results.Ok(result);
    }
}
