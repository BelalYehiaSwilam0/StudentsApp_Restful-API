var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
