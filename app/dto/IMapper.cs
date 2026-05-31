using hotel_booking.dto.response;
using hotel_booking.model;

namespace hotel_booking.dto;

public interface IMapper
{
    RoomResponse Map(Room room);
    GuestResponse Map(Guest guest);
    GuestLoyaltyResponse MapLoyalty(Guest guest);
    BookingResponse Map(Booking booking);
}
