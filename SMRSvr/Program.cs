using SMRSvr.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Shared Memory Services
builder.Services.AddSingleton<SharedMemoryService>();

// Read machine settings from appsettings.json
// MachineStatusService Ŭ������ totalToolCount�� �ֱ� ������ �Ŵ���� ��ü�� �����Ͽ� ����Ѵ� 
int totalToolCount = builder.Configuration.GetValue<int>("MachineSettings:TotalToolCount", 16);
builder.Services.AddSingleton<MachineStatusService>(sp => 
    new MachineStatusService(sp.GetRequiredService<SharedMemoryService>(), totalToolCount));

builder.Services.AddSingleton<MachineControlService>();
builder.Services.AddSingleton<NcFileService>();
builder.Services.AddTransient<SharedMemoryTestService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

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
