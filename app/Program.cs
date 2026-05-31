using Microsoft.EntityFrameworkCore;
using hotel_booking.api;
using hotel_booking.database;
using hotel_booking.dto;
using hotel_booking.interfaces;
using hotel_booking.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Hotel Booking API",
        Version = "v1",
        Description = "API гостиницы: номера, гости, бронирования, бонусы, подтверждение брони."
    });
});

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Не найдена строка подключения ConnectionStrings:Postgres.");

builder.Services.AddDbContext<HotelDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IConfirmationService, ConfirmationService>();
builder.Services.AddScoped<IMapper, Mapper>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Booking API v1");
        options.RoutePrefix = "swagger";
    });
}

var api = app.MapGroup("/api");
api.MapRoomsEndpoints();
api.MapGuestsEndpoints();
api.MapBookingsEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    message = "Hotel Booking API работает. Откройте /api/rooms, /api/guests, /api/bookings."
}));

await app.RunAsync();
