using hotel_booking.dto.request;
using hotel_booking.model;

namespace hotel_booking.interfaces;

public interface IBookingService
{
    Task<IReadOnlyList<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(Guid id);
    Task<Booking> CreateAsync(CreateBookingRequest request);
    Task<Booking?> UpdateAsync(Guid id, CreateBookingRequest request);
    Task<bool> DeleteAsync(Guid id);
}
