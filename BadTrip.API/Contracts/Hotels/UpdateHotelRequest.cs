namespace BadTrip.API.Contracts.Booking
{
    public record UpdateHotelRequest(
            string Name,
            string Street,
            string City,
            string Country,
            int Stars,
            string ImageUrl
        );
}

