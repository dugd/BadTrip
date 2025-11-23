using BadTrip.Application.Features.Hotels.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using BadTrip.Domain.ValueObjects;
using MediatR;

namespace BadTrip.Application.Features.Hotels.Commands.UpdateHotel
{
    public record UpdateHotelCommand(
        Guid Id,
        string Name,
        string Street,
        string City,
        string Country,
        int Stars,
        string ImageUrl
    ) : IRequest<HotelResponse>;

    public class UpdateHotelCommandHandler : IRequestHandler<UpdateHotelCommand, HotelResponse>
    {
        private readonly IHotelRepository _hotelRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateHotelCommandHandler(IHotelRepository hotelRepo, IUnitOfWork unitOfWork)
        {
            _hotelRepo = hotelRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<HotelResponse> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
        {
            // Get existing hotel
            var hotel = await _hotelRepo.GetByIdAsync(request.Id);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.Id);
            }

            // Check name uniqueness (excluding current hotel)
            if (!await _hotelRepo.IsNameUniqueAsync(request.Name, request.Id))
            {
                throw new DomainException($"Hotel with name '{request.Name}' already exists.");
            }

            // Create Address Value Object
            var address = new Address(request.Street, request.City, request.Country);

            // Update hotel via domain method
            hotel.Update(request.Name, address, request.Stars, request.ImageUrl);

            // Save
            _hotelRepo.Update(hotel);
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
