using SMRSvr.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Shared Memory Services
builder.Services.AddSingleton<SharedMemoryService>();
builder.Services.AddTransient<SharedMemoryTestService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Run Shared Memory Test if needed (e.g., during development)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var testService = scope.ServiceProvider.GetRequiredService<SharedMemoryTestService>();
        testService.RunAllTests();
    }
}

app.Run();
