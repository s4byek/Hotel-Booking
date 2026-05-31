namespace hotel_booking.dto.response;

public record ErrorResponse
{
    public string Message { get; init; } = string.Empty;
}
