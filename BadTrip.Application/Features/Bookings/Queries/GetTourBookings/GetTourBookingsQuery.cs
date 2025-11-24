using BadTrip.Application.Features.Bookings.DTO;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Queries.GetTourBookings;

public record GetTourBookingsQuery(
    Guid TourId,
    Guid OperatorId
) : IRequest<IReadOnlyList<BookingResponse>>;
