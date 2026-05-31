using hotel_booking.model;

namespace hotel_booking.interfaces;

public interface ILoyaltyService
{
    int CalculateEarnedPoints(decimal bookingTotal);
    void AddPoints(Guest guest, int points);
    void RemovePoints(Guest guest, int points);
}
