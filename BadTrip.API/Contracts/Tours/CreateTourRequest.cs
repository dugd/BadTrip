namespace BadTrip.API.Contracts.Tours
{
    public record CreateTourRequest(
        string Title,
        string Description,
        decimal PriceAmount,
        string PriceCurrency,
        string ImageUrl,
        Guid? HotelId,
        int MaxParticipants,
        DateTime StartDate,
        DateTime EndDate,
        Guid OperatorId
    );
}
