using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// ===============================
// JWT Authentication Configuration
// ===============================

// Register authentication services in the dependency injection container.
// JwtBearerDefaults.AuthenticationScheme tells ASP.NET Core that
// JWT Bearer authentication will be the default authentication method.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // TokenValidationParameters define how incoming JWTs will be validated.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Ensures the token was issued by a trusted issuer.
            ValidateIssuer = true,


            // Ensures the token is intended for this API (audience check).
            ValidateAudience = true,


            // Ensures the token has not expired.
            ValidateLifetime = true,


            // Ensures the token signature is valid and was signed by the API.
            ValidateIssuerSigningKey = true,


            // The expected issuer value (must match the issuer used when creating the JWT).
            ValidIssuer = "MyStudentsApp",


            // The expected audience value (must match the audience used when creating the JWT).
            ValidAudience = "ApiUsers",


            // The secret key used to validate the JWT signature.
            // This must be the same key used when generating the token.
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")))
        };
    });


// ===============================
// Authorization Configuration
// ===============================

// Register authorization services.
// This enables attributes like [Authorize] and role-based authorization.
builder.Services.AddAuthorization();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This converter allows Enums to be displayed as strings (e.g., "UserName") 
        // instead of integers (e.g., 1) in Swagger and JSON responses.
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Register Swagger generator and customize its behavior.
builder.Services.AddSwaggerGen(options =>
{
    // ===============================
    // 1) Define the JWT Bearer security scheme
    // ===============================
    //
    // This tells Swagger that our API uses JWT Bearer authentication
    // through the HTTP Authorization header.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // The name of the HTTP header where the token will be sent.
        Name = "Authorization",


        // Indicates this is an HTTP authentication scheme.
        Type = SecuritySchemeType.Http,


        // Specifies the authentication scheme name.
        // Must be exactly "Bearer" for JWT Bearer tokens.
        Scheme = "Bearer",


        // Optional metadata to describe the token format.
        BearerFormat = "JWT",


        // Specifies that the token is sent in the request header.
        In = ParameterLocation.Header,


        // Text shown in Swagger UI to guide the user.
        Description = "Enter: Bearer {your JWT token}"
    });


    // ===============================
    // 2) Require the Bearer scheme for secured endpoints
    // ===============================
    //
    // This tells Swagger that endpoints protected by [Authorize]
    // require the Bearer token defined above.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                // Reference the previously defined "Bearer" security scheme.
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },


            // No scopes are required for JWT Bearer authentication.
            // This array is empty because JWT does not use OAuth scopes here.
            new string[] {}
        }
    });
});

// --- PHASE: CORS Configuration ---
// Definition: CORS is a browser security feature that restricts cross-origin HTTP requests.
// Pro-Tip: "If the request does not come from a browser, CORS does not apply."
builder.Services.AddCors(options =>
{
    // Define a specific policy name for better management
    options.AddPolicy("StudentApiCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5197", "https://localhost:7169") // Add your frontend URLs here
              .AllowAnyMethod()  // Allows GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader();  // Allows custom headers like Authorization or Content-Type

    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- STEP 1: Verify HTTPS Redirection Middleware ---
// This middleware ensures that any incoming HTTP request is automatically 
// redirected to its HTTPS counterpart, forcing a secure connection.
//It must appear before controller mapping.
app.UseHttpsRedirection();

//UseCors MUST be placed after app.UseHttpsRedirection();
app.UseCors("AllowLocalClient");

app.UseAuthentication();
// Authorization checks access rules (e.g., [Authorize], roles, policies).
app.UseAuthorization();

// Map controller routes (e.g., /api/Auth/login, /api/Users/All).
app.MapControllers();

// Start the web application.
app.Run();
