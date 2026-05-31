namespace hotel_booking.dto.request;

public record UpdateGuestRequest
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int LoyaltyPoints { get; init; }
}
