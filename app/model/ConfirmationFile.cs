namespace hotel_booking.model;

public class ConfirmationFile
{
    public string FileName { get; init; } = string.Empty;

    public string ContentType { get; init; } = "text/plain";

    public byte[] Content { get; init; } = [];
}
