namespace hotel_booking.model;

public class Guest
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int LoyaltyPoints { get; set; }
}
