using hotel_booking.dto.request;
using hotel_booking.dto.response;
using hotel_booking.model;

namespace hotel_booking.interfaces;

public interface IRoomService
{
    Task<IReadOnlyList<Room>> GetAllAsync();
    Task<Room?> GetByIdAsync(Guid id);
    Task<Room> AddAsync(CreateRoomRequest request);
    Task<Room?> UpdateAsync(Guid id, UpdateRoomRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<RoomAvailabilityResponse?> GetAvailabilityAsync(Guid id, DateOnly checkInDate, DateOnly checkOutDate);
}
