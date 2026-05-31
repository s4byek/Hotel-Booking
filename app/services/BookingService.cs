using Microsoft.EntityFrameworkCore;
using hotel_booking.database;
using hotel_booking.dto.request;
using hotel_booking.interfaces;
using hotel_booking.model;

namespace hotel_booking.services;

public class BookingService(HotelDbContext db, ILoyaltyService loyaltyService) : IBookingService
{
    public async Task<IReadOnlyList<Booking>> GetAllAsync()
        => await db.Bookings
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

    public async Task<Booking?> GetByIdAsync(Guid id)
        => await db.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Booking> CreateAsync(CreateBookingRequest request)
    {
        ValidateBookingRequest(request);

        var guest = await db.Guests.FirstOrDefaultAsync(x => x.Id == request.GuestId);
        if (guest is null)
        {
            throw new InvalidOperationException("Гость не найден.");
        }

        var groupedRoomIds = request.RoomIds
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());

        var rooms = await LoadAndValidateRoomsAsync(groupedRoomIds);
        await ValidateAvailabilityAsync(groupedRoomIds, rooms, request.CheckInDate, request.CheckOutDate, null);

        var nights = CalculateNights(request.CheckInDate, request.CheckOutDate);
        var total = CalculateTotal(request.RoomIds, rooms, nights);
        var earnedPoints = loyaltyService.CalculateEarnedPoints(total);
        loyaltyService.AddPoints(guest, earnedPoints);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            GuestId = guest.Id,
            GuestName = guest.FullName,
            CreatedAtUtc = DateTime.UtcNow,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Nights = nights,
            Total = total,
            RoomIds = request.RoomIds.ToArray(),
            EarnedPoints = earnedPoints
        };

        db.Bookings.Add(booking);
        await db.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking?> UpdateAsync(Guid id, CreateBookingRequest request)
    {
        ValidateBookingRequest(request);

        var booking = await db.Bookings.FirstOrDefaultAsync(x => x.Id == id);
        if (booking is null)
        {
            return null;
        }

        var newGuest = await db.Guests.FirstOrDefaultAsync(x => x.Id == request.GuestId);
        if (newGuest is null)
        {
            throw new InvalidOperationException("Гость не найден.");
        }

        var groupedRoomIds = request.RoomIds
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());

        var rooms = await LoadAndValidateRoomsAsync(groupedRoomIds);
        await ValidateAvailabilityAsync(groupedRoomIds, rooms, request.CheckInDate, request.CheckOutDate, booking.Id);

        var oldGuest = await db.Guests.FirstAsync(x => x.Id == booking.GuestId);
        loyaltyService.RemovePoints(oldGuest, booking.EarnedPoints);

        var nights = CalculateNights(request.CheckInDate, request.CheckOutDate);
        var total = CalculateTotal(request.RoomIds, rooms, nights);
        var earnedPoints = loyaltyService.CalculateEarnedPoints(total);
        loyaltyService.AddPoints(newGuest, earnedPoints);

        booking.GuestId = newGuest.Id;
        booking.GuestName = newGuest.FullName;
        booking.CheckInDate = request.CheckInDate;
        booking.CheckOutDate = request.CheckOutDate;
        booking.Nights = nights;
        booking.Total = total;
        booking.RoomIds = request.RoomIds.ToArray();
        booking.EarnedPoints = earnedPoints;

        await db.SaveChangesAsync();
        return booking;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var booking = await db.Bookings.FirstOrDefaultAsync(x => x.Id == id);
        if (booking is null)
        {
            return false;
        }

        var guest = await db.Guests.FirstAsync(x => x.Id == booking.GuestId);
        loyaltyService.RemovePoints(guest, booking.EarnedPoints);

        db.Bookings.Remove(booking);
        await db.SaveChangesAsync();
        return true;
    }

    private static void ValidateBookingRequest(CreateBookingRequest request)
    {
        if (request.RoomIds.Count == 0)
        {
            throw new ArgumentException("Список номеров в бронировании не должен быть пустым.");
        }

        if (request.CheckInDate >= request.CheckOutDate)
        {
            throw new ArgumentException("Дата выезда должна быть позже даты заезда.");
        }
    }

    private async Task<IReadOnlyList<Room>> LoadAndValidateRoomsAsync(Dictionary<Guid, int> groupedRoomIds)
    {
        var rooms = await db.Rooms
            .Where(x => groupedRoomIds.Keys.Contains(x.Id))
            .ToListAsync();

        foreach (var pair in groupedRoomIds)
        {
            var room = rooms.FirstOrDefault(x => x.Id == pair.Key);
            if (room is null)
            {
                throw new InvalidOperationException($"Тип номера {pair.Key} не найден.");
            }
        }

        return rooms;
    }

    private async Task ValidateAvailabilityAsync(
        Dictionary<Guid, int> requestedRooms,
        IReadOnlyList<Room> rooms,
        DateOnly checkInDate,
        DateOnly checkOutDate,
        Guid? excludedBookingId)
    {
        var overlappingBookings = await db.Bookings
            .AsNoTracking()
            .Where(x => x.CheckInDate < checkOutDate && checkInDate < x.CheckOutDate)
            .Where(x => excludedBookingId == null || x.Id != excludedBookingId.Value)
            .ToListAsync();

        foreach (var pair in requestedRooms)
        {
            var room = rooms.First(x => x.Id == pair.Key);
            var alreadyBooked = overlappingBookings.Sum(x => x.RoomIds.Count(roomId => roomId == pair.Key));

            if (alreadyBooked + pair.Value > room.AvailableCount)
            {
                var available = Math.Max(0, room.AvailableCount - alreadyBooked);
                throw new InvalidOperationException(
                    $"Недостаточно свободных номеров типа \"{room.Name}\" на выбранные даты. Доступно: {available}.");
            }
        }
    }

    private static int CalculateNights(DateOnly checkInDate, DateOnly checkOutDate)
        => checkOutDate.DayNumber - checkInDate.DayNumber;

    private static decimal CalculateTotal(IReadOnlyList<Guid> roomIds, IReadOnlyList<Room> rooms, int nights)
    {
        var prices = rooms.ToDictionary(x => x.Id, x => x.NightlyRate);
        return roomIds.Sum(roomId => prices[roomId]) * nights;
    }
}
