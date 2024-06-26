using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NAID_Users.Authorization;
using NAID_Users.Data;
using NAID_Users.Interfaces;
using NAID_Users.Mappers;
using NAID_Users.Models;
using NAID_Users.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Alow API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


// Register AutoMapper
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddIdentity<AppUser, Role>(Options => {
    Options.Password.RequireDigit = true;
    Options.Password.RequireLowercase = true;
    Options.Password.RequireUppercase = true;
    Options.Password.RequireNonAlphanumeric = true;
    Options.Password.RequiredLength = 8;
    Options.User.RequireUniqueEmail = true;
    Options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    Options.Lockout.MaxFailedAccessAttempts = 5;
    Options.Lockout.AllowedForNewUsers = true;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddTokenProvider<DataProtectorTokenProvider<AppUser>>("Default");

builder.Services.AddAuthentication(Options => {
    Options.DefaultScheme =
    Options.DefaultAuthenticateScheme =
    Options.DefaultChallengeScheme =
    Options.DefaultForbidScheme =
    Options.DefaultSignInScheme =
    Options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(Options => {
    Options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigninKey"])
        )
    };
});

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("PermissionPolicy", policy =>
    //        policy.Requirements.Add(new PermissionRequirement("PermissionAddProduct")));
});

builder.Services.AddDbContext<ApplicationDbContext>(Options => {
    Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRolePermissionService, RolePermssionService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    //.WithOrigins()
    .SetIsOriginAllowed(origin => true)
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
