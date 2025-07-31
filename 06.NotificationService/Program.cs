using _06.NotificationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<INotificationSender, ConsoleNotificationSender>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();