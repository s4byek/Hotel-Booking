using Microsoft.EntityFrameworkCore;
using hotel_booking.database;
using hotel_booking.dto.request;
using hotel_booking.dto.response;
using hotel_booking.interfaces;
using hotel_booking.model;

namespace hotel_booking.services;

public class RoomService(HotelDbContext db) : IRoomService
{
    public async Task<IReadOnlyList<Room>> GetAllAsync()
        => await db.Rooms
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

    public async Task<Room?> GetByIdAsync(Guid id)
        => await db.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Room> AddAsync(CreateRoomRequest request)
    {
        ValidateRoomFields(request.Name, request.NightlyRate, request.Capacity, request.AvailableCount);

        var id = request.Id ?? Guid.NewGuid();
        if (await db.Rooms.AnyAsync(x => x.Id == id))
        {
            throw new InvalidOperationException($"Тип номера с идентификатором {id} уже существует.");
        }

        var entity = new Room
        {
            Id = id,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            NightlyRate = request.NightlyRate,
            Capacity = request.Capacity,
            AvailableCount = request.AvailableCount
        };

        db.Rooms.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Room?> UpdateAsync(Guid id, UpdateRoomRequest request)
    {
        ValidateRoomFields(request.Name, request.NightlyRate, request.Capacity, request.AvailableCount);

        var entity = await db.Rooms.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return null;
        }

        entity.Name = request.Name.Trim();
        entity.Description = request.Description.Trim();
        entity.NightlyRate = request.NightlyRate;
        entity.Capacity = request.Capacity;
        entity.AvailableCount = request.AvailableCount;
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.Rooms.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return false;
        }

        var usedInBooking = await db.Bookings.AnyAsync(x => x.RoomIds.Contains(id));
        if (usedInBooking)
        {
            throw new InvalidOperationException(
                "Нельзя удалить тип номера: он указан в одном или нескольких бронированиях.");
        }

        db.Rooms.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<RoomAvailabilityResponse?> GetAvailabilityAsync(Guid id, DateOnly checkInDate, DateOnly checkOutDate)
    {
        ValidateAvailabilityDates(checkInDate, checkOutDate);

        var room = await db.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (room is null)
        {
            return null;
        }

        var overlappingBookings = await db.Bookings
            .AsNoTracking()
            .Where(x => x.CheckInDate < checkOutDate && checkInDate < x.CheckOutDate)
            .ToListAsync();

        var bookedCount = overlappingBookings.Sum(x => x.RoomIds.Count(roomId => roomId == id));
        var availableCount = Math.Max(0, room.AvailableCount - bookedCount);

        return new RoomAvailabilityResponse
        {
            RoomId = room.Id,
            Name = room.Name,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            TotalCount = room.AvailableCount,
            BookedCount = bookedCount,
            AvailableCount = availableCount
        };
    }

    private static void ValidateRoomFields(string name, decimal nightlyRate, int capacity, int availableCount)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Название типа номера не должно быть пустым.");
        }

        if (nightlyRate < 0)
        {
            throw new ArgumentException("Стоимость ночи не может быть отрицательной.");
        }

        if (capacity <= 0)
        {
            throw new ArgumentException("Вместимость номера должна быть больше нуля.");
        }

        if (availableCount < 0)
        {
            throw new ArgumentException("Количество доступных номеров не может быть отрицательным.");
        }
    }

    private static void ValidateAvailabilityDates(DateOnly checkInDate, DateOnly checkOutDate)
    {
        if (checkInDate >= checkOutDate)
        {
            throw new ArgumentException("Дата выезда должна быть позже даты заезда.");
        }
    }
}
