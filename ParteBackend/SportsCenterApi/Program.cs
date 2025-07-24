using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SportsCenterApi.Models;
using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Services;
using SportsCenterApi.Repositories;
using System.Text;
using Microsoft.OpenApi.Models;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             //CODE BY SAMUEL RADU DRAGOMIR

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


builder.Services.AddDbContext<SportsCenterContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRespository<>));
builder.Services.AddScoped<ItokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IFacilitiesRepository, FacilitiesRepository>();
builder.Services.AddScoped<IFacilityService, FacilityService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddEndpointsApiExplorer();


//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//Autorization for the code permission 
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MakeReservations", policy =>
           policy.RequireClaim("permission", "MAKE_RESERVATIONS"));

    options.AddPolicy("CancelOwnReservations", policy =>
        policy.RequireClaim("permission", "CANCEL_OWN_RESERVATIONS"));

    options.AddPolicy("CancelOthersReservationsLimited", policy =>
        policy.RequireClaim("permission", "CANCEL_OTHERS_RESERVATIONS_LIMITED"));

    options.AddPolicy("CancelOthersReservationsUnlimited", policy =>
        policy.RequireClaim("permission", "CANCEL_OTHERS_RESERVATIONS_UNLIMITED"));

    options.AddPolicy("ViewOwnReservations", policy =>
        policy.RequireClaim("permission", "VIEW_OWN_RESERVATIONS"));

    options.AddPolicy("ViewOthersReservations", policy =>
        policy.RequireClaim("permission", "VIEW_OTHERS_RESERVATIONS"));

    options.AddPolicy("ChangeOwnPassword", policy =>
        policy.RequireClaim("permission", "CHANGE_OWN_PASSWORD"));

    options.AddPolicy("ChangeOthersPassword", policy =>
        policy.RequireClaim("permission", "CHANGE_OTHERS_PASSWORD"));

    options.AddPolicy("RegisterAsMemberOneself", policy =>
        policy.RequireClaim("permission", "REGISTER_AS_MEMBER_ONESELF"));

    options.AddPolicy("RegisterAnyoneAsMember", policy =>
        policy.RequireClaim("permission", "REGISTER_ANYONE_AS_MEMBER"));

    options.AddPolicy("ModifyOwnMemberData", policy =>
        policy.RequireClaim("permission", "MODIFY_OWN_MEMBER_DATA"));

    options.AddPolicy("ModifyOthersMemberData", policy =>
        policy.RequireClaim("permission", "MODIFY_OTHERS_MEMBER_DATA"));

    options.AddPolicy("DeactivateMember", policy =>
        policy.RequireClaim("permission", "DEACTIVATE_MEMBER"));

    options.AddPolicy("GenerateReports", policy =>
        policy.RequireClaim("permission", "GENERATE_REPORTS"));

    options.AddPolicy("ViewCharts", policy =>
        policy.RequireClaim("permission", "VIEW_CHARTS"));

    options.AddPolicy("ModifyRolesAndPermissions", policy =>
        policy.RequireClaim("permission", "MODIFY_ROLES_AND_PERMISSIONS"));

    options.AddPolicy("ImportExportOperations", policy =>
        policy.RequireClaim("permission", "IMPORT_EXPORT_OPERATIONS"));

    options.AddPolicy("ModifyFacilitySchedules", policy =>
        policy.RequireClaim("permission", "MODIFY_FACILITY_SCHEDULES"));

    options.AddPolicy("HaveMoreThanOneActiveReservation", policy =>
        policy.RequireClaim("permission", "HAVE_MORE_THAN_1_ACTIVE_RESERVATION"));

    options.AddPolicy("ViewAdministrationMenu", policy =>
        policy.RequireClaim("permission", "VIEW_ADMINISTRATION_MENU"));

    options.AddPolicy("ReserveUnlimited", policy =>
       policy.RequireClaim("permission", "RESERVE_UNLIMITED"));

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
