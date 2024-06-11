using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using UniversityApp.Service.Dtos.GroupDtos;
using UniversityApp.Service.Interfaces;
using UniversityApp.Service.Implementations;
using UniversityApp.Api.Middlewares;
using UniversityApp.Service.Exceptions;
using Microsoft.AspNetCore.Mvc;
using UniversityApp.Data.Repositories.Interfaces;
using UniversityApp.Data.Repositories.Implmentations;
using UniversityApp.Service.Profiles;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using UniversityApp.Core.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        string secret = "my super secret security jwt token my super secret security jwt token";
        SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateActor = true,
            ValidIssuer = "https://localhost:7061/",
            ValidAudience = "https://localhost:7061/",
            IssuerSigningKey = key
        };
    });

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {

        var errors = context.ModelState.Where(x => x.Value.Errors.Count > 0)
        .Select(x => new RestExceptionError(x.Key, x.Value.Errors.First().ErrorMessage)).ToList();

        return new BadRequestObjectResult(new {message="",errors});
    };
});

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 6;
}).AddDefaultTokenProviders().AddEntityFrameworkStores<UniversityDbContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddAutoMapper(x => x.AddProfile(typeof(MapProfile)));

builder.Services.AddHttpContextAccessor();
//builder.Services.AddAutoMapper(typeof(MapProfile).Assembly);

builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MapProfile(provider.GetService<IHttpContextAccessor>()));
}).CreateMapper());

builder.Services.AddDbContext<UniversityDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<GroupCreateDtoValidator>();

builder.Services.AddScoped<IStudentRepository,  StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IGroupRepository,GroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IAuthService,AuthService>();

builder.Services.AddFluentValidationRulesToSwagger();



var app = builder.Build();




app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.MapControllers();

app.UseMiddleware<ExceptionHandlerMiddleware>();
//app.UseMiddleware<LoggingMiddleware>();

app.Run();
