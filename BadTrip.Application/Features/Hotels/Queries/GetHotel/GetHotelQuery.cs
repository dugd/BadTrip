using BadTrip.Application.Features.Hotels.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces.Repositories;
using MediatR;

namespace BadTrip.Application.Features.Hotels.Queries.GetHotel
{
    public record GetHotelQuery(Guid Id) : IRequest<HotelResponse>;

    public class GetHotelQueryHandler : IRequestHandler<GetHotelQuery, HotelResponse>
    {
        private readonly IHotelRepository _hotelRepo;

        public GetHotelQueryHandler(IHotelRepository hotelRepo)
        {
            _hotelRepo = hotelRepo;
        }

        public async Task<HotelResponse> Handle(GetHotelQuery request, CancellationToken cancellationToken)
        {
            var hotel = await _hotelRepo.GetByIdAsync(request.Id);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.Id);
            }

            return new HotelResponse(
                hotel.Id,
                hotel.Name,
                new AddressDto(hotel.Address.Street, hotel.Address.City, hotel.Address.Country),
                hotel.Stars,
                hotel.ImageUrl,
                hotel.CreatedAt
            );
        }
    }
}
