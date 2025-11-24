namespace BadTrip.API.Contracts.Tours
{
    public record UpdateTourRequest(
        string Title,
        string Description,
        decimal PriceAmount,
        string PriceCurrency,
        string ImageUrl,
        Guid? HotelId,
        int MaxParticipants,
        DateTime StartDate,
        DateTime EndDate
    );
}
