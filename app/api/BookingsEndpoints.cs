using hotel_booking.dto;
using hotel_booking.dto.request;
using hotel_booking.dto.response;
using hotel_booking.interfaces;

namespace hotel_booking.api;

public static class BookingsEndpoints
{
    public static RouteGroupBuilder MapBookingsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/bookings").WithTags("Bookings");

        group.MapGet("/", async (IBookingService bookings, IMapper mapper) =>
            {
                var result = await bookings.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список бронирований")
            .Produces<IEnumerable<BookingResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{bookingId:guid}/confirmation",
                async (Guid bookingId, IBookingService bookings, IConfirmationService confirmations) =>
            {
                var booking = await bookings.GetByIdAsync(bookingId);
                if (booking is null)
                {
                    return Results.NotFound(new ErrorResponse { Message = "Бронирование не найдено." });
                }

                var confirmation = confirmations.BuildConfirmation(booking);
                return Results.File(confirmation.Content, confirmation.ContentType, confirmation.FileName);
            })
            .WithSummary("Скачать подтверждение бронирования")
            .Produces(StatusCodes.Status200OK, contentType: "text/plain")
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/{bookingId:guid}",
                async (Guid bookingId, IBookingService bookings, IMapper mapper) =>
            {
                var booking = await bookings.GetByIdAsync(bookingId);
                return booking is null
                    ? Results.NotFound(new ErrorResponse { Message = "Бронирование не найдено." })
                    : Results.Ok(mapper.Map(booking));
            })
            .WithSummary("Получить бронирование по идентификатору")
            .Produces<BookingResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateBookingRequest request, IBookingService bookings, IMapper mapper) =>
            {
                try
                {
                    var booking = await bookings.CreateAsync(request);
                    return Results.Created($"/api/bookings/{booking.Id}", mapper.Map(booking));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Создать бронирование")
            .WithDescription("Проверяет гостя, выбранные типы номеров и календарную доступность на даты.")
            .Produces<BookingResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{bookingId:guid}",
                async (Guid bookingId, CreateBookingRequest request, IBookingService bookings, IMapper mapper) =>
            {
                try
                {
                    var booking = await bookings.UpdateAsync(bookingId, request);
                    return booking is null
                        ? Results.NotFound(new ErrorResponse { Message = "Бронирование не найдено." })
                        : Results.Ok(mapper.Map(booking));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить бронирование")
            .Produces<BookingResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{bookingId:guid}",
                async (Guid bookingId, IBookingService bookings) =>
            {
                var deleted = await bookings.DeleteAsync(bookingId);
                return deleted
                    ? Results.NoContent()
                    : Results.NotFound(new ErrorResponse { Message = "Бронирование не найдено." });
            })
            .WithSummary("Удалить бронирование")
            .WithDescription("Удаляет бронь и списывает начисленные за нее бонусы.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}
