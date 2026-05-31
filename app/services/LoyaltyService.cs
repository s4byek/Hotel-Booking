using hotel_booking.interfaces;
using hotel_booking.model;

namespace hotel_booking.services;

public class LoyaltyService : ILoyaltyService
{
    private const decimal PointsRate = 0.01m;

    public int CalculateEarnedPoints(decimal bookingTotal)
    {
        if (bookingTotal <= 0)
        {
            return 0;
        }

        return (int)Math.Floor(bookingTotal * PointsRate);
    }

    public void AddPoints(Guest guest, int points)
    {
        if (points <= 0)
        {
            return;
        }

        guest.LoyaltyPoints += points;
    }

    public void RemovePoints(Guest guest, int points)
    {
        if (points <= 0)
        {
            return;
        }

        guest.LoyaltyPoints = Math.Max(0, guest.LoyaltyPoints - points);
    }
}
