using BadTrip.Application.Features.Bookings.DTO;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Queries.GetBooking;

public record GetBookingQuery(Guid BookingId) : IRequest<BookingResponse>;
