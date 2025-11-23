using BadTrip.Application.Features.Hotels.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using BadTrip.Domain.ValueObjects;
using MediatR;

namespace BadTrip.Application.Features.Hotels.Commands.CreateHotel
{
    public record CreateHotelCommand(
        string Name,
        string Street,
        string City,
        string Country,
        int Stars,
        string ImageUrl
    ) : IRequest<HotelResponse>;

    public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, HotelResponse>
    {
        private readonly IHotelRepository _hotelRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateHotelCommandHandler(IHotelRepository hotelRepo, IUnitOfWork unitOfWork)
        {
            _hotelRepo = hotelRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<HotelResponse> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
        {
            // Check name uniqueness
            if (!await _hotelRepo.IsNameUniqueAsync(request.Name))
            {
                throw new DomainException($"Hotel with name '{request.Name}' already exists.");
            }

            // Create Address Value Object
            var address = new Address(request.Street, request.City, request.Country);

            // Create Hotel entity via factory method
            var hotel = Hotel.Create(request.Name, address, request.Stars, request.ImageUrl);

            // Save
            await _hotelRepo.AddAsync(hotel);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return response
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
