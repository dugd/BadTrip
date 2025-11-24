using BadTrip.Application.Features.Bookings.DTO;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Queries.GetMyBookings;

public record GetMyBookingsQuery(Guid UserId) : IRequest<IReadOnlyList<BookingResponse>>;
