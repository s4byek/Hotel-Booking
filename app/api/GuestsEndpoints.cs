using hotel_booking.dto;
using hotel_booking.dto.request;
using hotel_booking.dto.response;
using hotel_booking.interfaces;

namespace hotel_booking.api;

public static class GuestsEndpoints
{
    public static RouteGroupBuilder MapGuestsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/guests").WithTags("Guests");

        group.MapGet("/", async (IGuestService guests, IMapper mapper) =>
            {
                var result = await guests.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список гостей")
            .WithDescription("Возвращает всех зарегистрированных гостей и их текущие бонусные баллы.")
            .Produces<IEnumerable<GuestResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{guestId:guid}/loyalty",
                async (Guid guestId, IGuestService guests, IMapper mapper) =>
            {
                var guest = await guests.GetByIdAsync(guestId);
                return guest is null
                    ? Results.NotFound(new ErrorResponse { Message = "Гость не найден." })
                    : Results.Ok(mapper.MapLoyalty(guest));
            })
            .WithSummary("Получить бонусный баланс гостя")
            .Produces<GuestLoyaltyResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/{guestId:guid}",
                async (Guid guestId, IGuestService guests, IMapper mapper) =>
            {
                var guest = await guests.GetByIdAsync(guestId);
                return guest is null
                    ? Results.NotFound(new ErrorResponse { Message = "Гость не найден." })
                    : Results.Ok(mapper.Map(guest));
            })
            .WithSummary("Получить гостя по идентификатору")
            .Produces<GuestResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateGuestRequest body, IGuestService guests, IMapper mapper) =>
            {
                try
                {
                    var created = await guests.AddAsync(body);
                    return Results.Created($"/api/guests/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить гостя")
            .Produces<GuestResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{guestId:guid}",
                async (Guid guestId, UpdateGuestRequest body, IGuestService guests, IMapper mapper) =>
            {
                try
                {
                    var updated = await guests.UpdateAsync(guestId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Гость не найден." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить гостя")
            .Produces<GuestResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{guestId:guid}",
                async (Guid guestId, IGuestService guests) =>
            {
                try
                {
                    var deleted = await guests.DeleteAsync(guestId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Гость не найден." });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить гостя")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}
