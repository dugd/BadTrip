using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using MediatR;

namespace BadTrip.Application.Features.Hotels.Commands.DeleteHotel
{
    public record DeleteHotelCommand(Guid Id) : IRequest;

    public class DeleteHotelCommandHandler : IRequestHandler<DeleteHotelCommand>
    {
        private readonly IHotelRepository _hotelRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteHotelCommandHandler(IHotelRepository hotelRepo, IUnitOfWork unitOfWork)
        {
            _hotelRepo = hotelRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteHotelCommand request, CancellationToken cancellationToken)
        {
            // Get existing hotel
            var hotel = await _hotelRepo.GetByIdAsync(request.Id);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.Id);
            }

            // Delete
            _hotelRepo.Delete(hotel);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
