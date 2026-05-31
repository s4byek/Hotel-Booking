namespace hotel_booking.model;

public class Room
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal NightlyRate { get; set; }

    public int Capacity { get; set; }

    public int AvailableCount { get; set; }
}
