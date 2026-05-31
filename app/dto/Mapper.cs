using hotel_booking.dto.response;
using hotel_booking.model;

namespace hotel_booking.dto;

public class Mapper : IMapper
{
    public RoomResponse Map(Room room) => new()
    {
        Id = room.Id,
        Name = room.Name,
        Description = room.Description,
        NightlyRate = room.NightlyRate,
        Capacity = room.Capacity,
        AvailableCount = room.AvailableCount
    };

    public GuestResponse Map(Guest guest) => new()
    {
        Id = guest.Id,
        FullName = guest.FullName,
        Email = guest.Email,
        LoyaltyPoints = guest.LoyaltyPoints
    };

    public GuestLoyaltyResponse MapLoyalty(Guest guest) => new()
    {
        Id = guest.Id,
        FullName = guest.FullName,
        LoyaltyPoints = guest.LoyaltyPoints
    };

    public BookingResponse Map(Booking booking) => new()
    {
        Id = booking.Id,
        GuestId = booking.GuestId,
        GuestName = booking.GuestName,
        CreatedAtUtc = booking.CreatedAtUtc,
        CheckInDate = booking.CheckInDate,
        CheckOutDate = booking.CheckOutDate,
        Nights = booking.Nights,
        Total = booking.Total,
        RoomIds = booking.RoomIds,
        EarnedPoints = booking.EarnedPoints
    };
}
