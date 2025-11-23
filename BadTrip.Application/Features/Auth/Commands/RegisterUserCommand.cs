using BadTrip.Application.Common.Interfaces;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Enums;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BadTrip.Application.Features.Auth.Commands
{
    public record RegisterUserCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string PhoneNumber,
        UserRole Role,

        string? CompanyName, // TourOperator
        DateTime? DateOfBirth // Tourist
        ) : IRequest<Guid>;

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _hasher;

        public RegisterUserCommandHandler(IUserRepository userRepo, IUnitOfWork unitOfWork, IPasswordHasher hasher)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _hasher = hasher;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (!await _userRepo.IsEmailUniqueAsync(request.Email))
            {
                throw new DomainException($"Email {request.Email} is already taken.");
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                if (!await _userRepo.IsPhoneNumberUniqueAsync(request.PhoneNumber))
                {
                    throw new DomainException($"Phone number {request.PhoneNumber} is already taken.");
                }
            }

            var passwordHash = _hasher.Hash(request.Password);

            User user = request.Role switch
            {
                UserRole.Tourist => User.CreateTourist(
                    request.Email,
                    passwordHash,
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber,
                    request.DateOfBirth ?? throw new ValidationException("DateOfBirth is missing")
                ),

                UserRole.TourOperator => User.CreateOperator(
                    request.Email,
                    passwordHash,
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber,
                    request.CompanyName ?? throw new ValidationException("CompanyName is missing")
                ),

                _ => throw new NotImplementedException($"Role {request.Role} is not supported yet.")
            };

            await _userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
