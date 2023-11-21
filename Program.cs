using IWantApp.Domain.Users;
using IWantApp.Endpoints.Categories;
using IWantApp.Endpoints.Clients;
using IWantApp.Endpoints.Employees;
using IWantApp.Endpoints.Orders;
using IWantApp.Endpoints.Products;
using IWantApp.Endpoints.Security;
using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseSerilog((context, configuration) =>
{
    configuration.WriteTo.Console().WriteTo.MSSqlServer(
        context.Configuration["ConnectionStrings:IWantDb"],
        sinkOptions: new MSSqlServerSinkOptions()
        {
            AutoCreateSqlTable = true,
            TableName = "LogApi"
        });
});
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:IWantDb"]);
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 3;
}).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<QueryAllUsersWithClaimName>();
builder.Services.AddScoped<QueryAllProdutsSold>();
builder.Services.AddScoped<UserCreator>();

builder.Services.AddAuthorization((options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();
    options.AddPolicy("EmployeePolicy", p => p.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));
    options.AddPolicy("CpfPolicy", p => p.RequireAuthenticatedUser().RequireClaim("Cpf"));
}));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);
app.MapMethods(EmployeePost.Template, EmployeePost.Methods, EmployeePost.Handle);
app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Methods, EmployeeGetAll.Handle);
app.MapMethods(EmployeeGetById.Template, EmployeeGetById.Methods, EmployeeGetById.Handle);
app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);
app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);
app.MapMethods(ProductGetAll.Template, ProductGetAll.Methods, ProductGetAll.Handle);
app.MapMethods(ProductGetShowcase.Template, ProductGetShowcase.Methods, ProductGetShowcase.Handle);
app.MapMethods(ProductGetSoldReport.Template, ProductGetSoldReport.Methods, ProductGetSoldReport.Handle);
app.MapMethods(ClientPost.Template, ClientPost.Methods, ClientPost.Handle);
app.MapMethods(ClientGet.Template, ClientGet.Methods, ClientGet.Handle);
app.MapMethods(OrderPost.Template, OrderPost.Methods, OrderPost.Handle);
app.MapMethods(OrderGet.Template, OrderGet.Methods, OrderGet.Handle);

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{
    var erro = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

    if (erro != null)
    {
        if (erro is SqlException)
            return Results.Problem(title: "DataBase Out", statusCode: 500);
        else if (erro is BadHttpRequestException)
            return Results.Problem(title: "Error to convert data to other type", statusCode: 500);

    }
    return Results.Problem(title: "An Error Ocurred", statusCode: 500);
});
app.Run();

