using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Preventyon;
using Preventyon.Data;
using Preventyon.EndPoints;
using Preventyon.Hubs;
using Preventyon.Middlewares;
using Preventyon.Models;
using Preventyon.Repository;
using Preventyon.Repository.IRepository;
using Preventyon.Service;
using Preventyon.Service.IService;
using Serilog;
using System.Net.Mail;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", builder =>
    {
        builder.WithOrigins("http://localhost:4203", "http://172.16.4.89:9002","http://localhost:4200","https://security-mngmt-fe.vercel.app/") 
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials() 
               .WithExposedHeaders("AccessToken"); 
    });
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Preventyon API", Version = "v1" });
});

/*********************************************** Authontication and Authorization ***************************************************/

builder.Services.AddSwaggerGen(option => {
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
             "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
             "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
             "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement()
               {
                   {
                       new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                           {
                               Type = ReferenceType.SecurityScheme,
                               Id = "Bearer"
                           },
                           Scheme = "oauth2",
                           Name = "Bearer",
                           In = ParameterLocation.Header,
                       },
                       new List<string>()
                   }
               });

});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            builder.Configuration.GetValue<string>("Jwt:Key"))),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("AdminIncidentsOnly", policy => policy.RequireRole("Admin-Incidents"));
    options.AddPolicy("AdminsUserOnly", policy => policy.RequireRole("Admins-User"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("user"));
    options.AddPolicy("UserOrSuperAdmin", policy =>
       policy.RequireAssertion(context =>
           context.User.IsInRole("user") || context.User.IsInRole("SuperAdmin")));
    options.AddPolicy("AdminUserOrSuperAdmin", policy =>
       policy.RequireAssertion(context =>
           context.User.IsInRole("Admins-User") || context.User.IsInRole("SuperAdmin")));
    options.AddPolicy("AdminIncidentsOrSuperAdmin", policy =>
       policy.RequireAssertion(context =>
           context.User.IsInRole("Admin-Incidents") || context.User.IsInRole("SuperAdmin")));
    options.AddPolicy("AdminsUserOrAdminIncidentsOrSuperAdmin", policy =>
       policy.RequireAssertion(context =>
           context.User.IsInRole("Admins-User") || context.User.IsInRole("Admin-Incidents") || context.User.IsInRole("SuperAdmin")));
    options.AddPolicy("AllowAll", policy =>
       policy.RequireAssertion(context =>
           context.User.IsInRole("user") || context.User.IsInRole("Admins-User") || context.User.IsInRole("Admin-Incidents") || context.User.IsInRole("SuperAdmin")));
});

/******************************************** End of Authontication and Authorization ************************************************/


/*###############################################  SERILOGGRR #######################################################################*/
var logerConfig = builder.Configuration.GetSection("Serilog");
Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(logerConfig)
           .WriteTo.Console()
           .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

builder.Host.UseSerilog()  // Integrate Serilog into the application
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();
            });

/*#########################################################################################################################*/

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped< IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAssignedIncidentRepository, AssignedIncidentRepository>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped< EmployeeRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
// Register Services
builder.Services.AddScoped<IAssignedIncidentService, AssignedIncidentService>();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IEmployeeService,EmployeeService>();
builder.Services.AddScoped <IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailRepository,EmailRepository>();
builder.Services.AddScoped<AccessTokenService>();  //JWT Access access token

var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
builder.Services.AddFluentEmail(smtpSettings.FromEmail, smtpSettings.FromName)
.AddRazorRenderer()
.AddSmtpSender(new SmtpClient(smtpSettings.Host)
{
    Port = smtpSettings.Port,
    Credentials = new System.Net.NetworkCredential(smtpSettings.UserName, smtpSettings.Password),
    EnableSsl = true,
    DeliveryMethod = SmtpDeliveryMethod.Network,
    UseDefaultCredentials = false,
});

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

app.UseCors("AllowAngularApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.ConfigureEndPoints();
app.UseHttpsRedirection();
app.UseMiddleware<JwtMiddleware>(builder.Configuration["Jwt:Key"]);
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "images"))
,
    RequestPath = "/images"
});


app.MapControllers();
app.MapHub<IncidentHub>("/incidentHub");
app.Run();
