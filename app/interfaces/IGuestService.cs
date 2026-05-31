using hotel_booking.dto.request;
using hotel_booking.model;

namespace hotel_booking.interfaces;

public interface IGuestService
{
    Task<IReadOnlyList<Guest>> GetAllAsync();
    Task<Guest?> GetByIdAsync(Guid id);
    Task<Guest> AddAsync(CreateGuestRequest request);
    Task<Guest?> UpdateAsync(Guid id, UpdateGuestRequest request);
    Task<bool> DeleteAsync(Guid id);
}
