using BadTrip.Application.Features.Hotels.DTO;
using BadTrip.Domain.Interfaces.Repositories;
using MediatR;

namespace BadTrip.Application.Features.Hotels.Queries.GetAllHotels
{
    public record GetAllHotelsQuery : IRequest<IReadOnlyList<HotelResponse>>;

    public class GetAllHotelsQueryHandler : IRequestHandler<GetAllHotelsQuery, IReadOnlyList<HotelResponse>>
    {
        private readonly IHotelRepository _hotelRepo;

        public GetAllHotelsQueryHandler(IHotelRepository hotelRepo)
        {
            _hotelRepo = hotelRepo;
        }

        public async Task<IReadOnlyList<HotelResponse>> Handle(GetAllHotelsQuery request, CancellationToken cancellationToken)
        {
            var hotels = await _hotelRepo.GetAllAsync();

            return hotels.Select(hotel => new HotelResponse(
                hotel.Id,
                hotel.Name,
                new AddressDto(hotel.Address.Street, hotel.Address.City, hotel.Address.Country),
                hotel.Stars,
                hotel.ImageUrl,
                hotel.CreatedAt
            )).ToList();
        }
    }
}
