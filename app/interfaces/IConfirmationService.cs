using hotel_booking.model;

namespace hotel_booking.interfaces;

public interface IConfirmationService
{
    ConfirmationFile BuildConfirmation(Booking booking);
}
