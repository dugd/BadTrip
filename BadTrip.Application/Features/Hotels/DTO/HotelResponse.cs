namespace BadTrip.Application.Features.Hotels.DTO
{
    public record AddressDto(
        string Street,
        string City,
        string Country
    );

    public record HotelResponse(
        Guid Id,
        string Name,
        AddressDto Address,
        int Stars,
        string ImageUrl,
        DateTime CreatedAt
    );
}
