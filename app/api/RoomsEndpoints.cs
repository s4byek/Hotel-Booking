using hotel_booking.dto;
using hotel_booking.dto.request;
using hotel_booking.dto.response;
using hotel_booking.interfaces;

namespace hotel_booking.api;

public static class RoomsEndpoints
{
    public static RouteGroupBuilder MapRoomsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/rooms").WithTags("Rooms");

        group.MapGet("/", async (IRoomService rooms, IMapper mapper) =>
            {
                var result = await rooms.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список типов номеров")
            .WithDescription("Возвращает все типы номеров с ценой за ночь, вместимостью и количеством номеров.")
            .Produces<IEnumerable<RoomResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{roomId:guid}",
                async (Guid roomId, IRoomService rooms, IMapper mapper) =>
            {
                var room = await rooms.GetByIdAsync(roomId);
                return room is null
                    ? Results.NotFound(new ErrorResponse { Message = "Тип номера не найден." })
                    : Results.Ok(mapper.Map(room));
            })
            .WithSummary("Получить тип номера по идентификатору")
            .Produces<RoomResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/{roomId:guid}/availability",
                async (Guid roomId, DateOnly checkInDate, DateOnly checkOutDate, IRoomService rooms) =>
            {
                try
                {
                    var availability = await rooms.GetAvailabilityAsync(roomId, checkInDate, checkOutDate);
                    return availability is null
                        ? Results.NotFound(new ErrorResponse { Message = "Тип номера не найден." })
                        : Results.Ok(availability);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Получить доступность типа номера на даты")
            .WithDescription("Показывает, сколько номеров выбранного типа свободно с учетом пересекающихся бронирований.")
            .Produces<RoomAvailabilityResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateRoomRequest body, IRoomService rooms, IMapper mapper) =>
            {
                try
                {
                    var created = await rooms.AddAsync(body);
                    return Results.Created($"/api/rooms/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить тип номера")
            .Produces<RoomResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{roomId:guid}",
                async (Guid roomId, UpdateRoomRequest body, IRoomService rooms, IMapper mapper) =>
            {
                try
                {
                    var updated = await rooms.UpdateAsync(roomId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Тип номера не найден." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить тип номера")
            .Produces<RoomResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{roomId:guid}",
                async (Guid roomId, IRoomService rooms) =>
            {
                try
                {
                    var deleted = await rooms.DeleteAsync(roomId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Тип номера не найден." });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить тип номера")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}
