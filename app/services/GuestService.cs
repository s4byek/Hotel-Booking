using Microsoft.EntityFrameworkCore;
using hotel_booking.database;
using hotel_booking.dto.request;
using hotel_booking.interfaces;
using hotel_booking.model;

namespace hotel_booking.services;

public class GuestService(HotelDbContext db) : IGuestService
{
    public async Task<IReadOnlyList<Guest>> GetAllAsync()
        => await db.Guests
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .ToListAsync();

    public async Task<Guest?> GetByIdAsync(Guid id)
        => await db.Guests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Guest> AddAsync(CreateGuestRequest request)
    {
        ValidateGuestFields(request.FullName, request.Email);

        var id = request.Id ?? Guid.NewGuid();
        if (await db.Guests.AnyAsync(x => x.Id == id))
        {
            throw new InvalidOperationException($"Гость с идентификатором {id} уже существует.");
        }

        var entity = new Guest
        {
            Id = id,
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            LoyaltyPoints = Math.Max(0, request.LoyaltyPoints)
        };

        db.Guests.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Guest?> UpdateAsync(Guid id, UpdateGuestRequest request)
    {
        ValidateGuestFields(request.FullName, request.Email);

        var entity = await db.Guests.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return null;
        }

        entity.FullName = request.FullName.Trim();
        entity.Email = request.Email.Trim();
        entity.LoyaltyPoints = Math.Max(0, request.LoyaltyPoints);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.Guests.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return false;
        }

        var hasBookings = await db.Bookings.AnyAsync(x => x.GuestId == id);
        if (hasBookings)
        {
            throw new InvalidOperationException("Нельзя удалить гостя: есть связанные бронирования.");
        }

        db.Guests.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    private static void ValidateGuestFields(string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("ФИО гостя не должно быть пустым.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email не должен быть пустым.");
        }
    }
}
