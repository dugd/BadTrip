namespace BadTrip.Application.Features.Tours.DTO
{
    public record MoneyDto(decimal Amount, string Currency);

    public record TourResponse(
        Guid Id,
        string Title,
        string Description,
        MoneyDto Price,
        string ImageUrl,
        Guid? HotelId,
        string? HotelName,
        int MaxParticipants,
        int SoldSpots,
        int AvailableSpots,
        DateTime StartDate,
        DateTime EndDate,
        Guid OperatorId,
        string OperatorName,
        DateTime CreatedAt
    );
}
