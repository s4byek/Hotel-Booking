using System.Text;
using hotel_booking.interfaces;
using hotel_booking.model;

namespace hotel_booking.services;

public class ConfirmationService : IConfirmationService
{
    public ConfirmationFile BuildConfirmation(Booking booking)
    {
        var builder = new StringBuilder();
        builder.AppendLine("HOTEL BOOKING");
        builder.AppendLine("Подтверждение бронирования");
        builder.AppendLine(new string('-', 40));
        builder.AppendLine($"Бронирование: {booking.Id}");
        builder.AppendLine($"Дата создания (UTC): {booking.CreatedAtUtc:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine($"Гость: {booking.GuestName} ({booking.GuestId})");
        builder.AppendLine($"Заезд: {booking.CheckInDate:yyyy-MM-dd}");
        builder.AppendLine($"Выезд: {booking.CheckOutDate:yyyy-MM-dd}");
        builder.AppendLine($"Ночей: {booking.Nights}");
        builder.AppendLine(new string('-', 40));
        builder.AppendLine("Типы номеров (id):");
        foreach (var roomId in booking.RoomIds)
        {
            builder.AppendLine($"- {roomId}");
        }

        builder.AppendLine(new string('-', 40));
        builder.AppendLine($"ИТОГО: {booking.Total:F2} RUB");
        builder.AppendLine($"Начислено бонусов: {booking.EarnedPoints}");
        builder.AppendLine("Спасибо за бронирование!");

        return new ConfirmationFile
        {
            FileName = $"booking-confirmation-{booking.Id}.txt",
            ContentType = "text/plain; charset=utf-8",
            Content = Encoding.UTF8.GetBytes(builder.ToString())
        };
    }
}
