var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- PHASE: CORS Configuration ---
// Definition: CORS is a browser security feature that restricts cross-origin HTTP requests.
// Pro-Tip: "If the request does not come from a browser, CORS does not apply."
//builder.Services.AddCors(options =>
//{
//    // Define a specific policy name for better management
//    options.AddPolicy("StudentApiCorsPolicy", policy =>
//    {
//        policy.WithOrigins("http://localhost:5197", "https://localhost:7169") // Add your frontend URLs here
//              .AllowAnyMethod()  // Allows GET, POST, PUT, DELETE, etc.
//              .AllowAnyHeader();  // Allows custom headers like Authorization or Content-Type
              
//    });
//});

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
app.UseAuthorization();

app.MapControllers();

app.Run();
