using CachingWithRedisDemo.Data;
using CachingWithRedisDemo.Middleware;
using CachingWithRedisDemo.Models;
using CachingWithRedisDemo.Services;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(x => x.EnableEndpointRouting = false);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("default")));
builder.Services.AddScoped(typeof(IStudentService<>), typeof(StudentService<>));
builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = builder.Configuration.GetSection("RedisConnection").GetValue<string>("Configuration");
    option.InstanceName = builder.Configuration.GetSection("RedisConnection").GetValue<string>("InstanceName");
});
builder.Services.AddOData();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseMiddleware<RedisCachingMiddleware>();

app.UseMvc(routeBuilder =>
{
    routeBuilder.Select().Expand().Filter().OrderBy().MaxTop(1);
    routeBuilder.MapODataServiceRoute("odata", "odata", getEdmModel());
});
IEdmModel getEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Student>("Student");
    return builder.GetEdmModel();
}
app.UseAuthorization();

app.MapControllers();

app.Run();
