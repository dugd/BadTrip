using BadTrip.Application.Common.Interfaces;
using BadTrip.Application.Features.Auth.DTO;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using FluentValidation;
using MediatR;

namespace BadTrip.Application.Features.Auth.Commands
{
    public record LoginUserCommand(string Email, string Password) : IRequest<AuthResponse>;

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public LoginUserCommandHandler(IUserRepository userRepo, IUnitOfWork unitOfWork, IPasswordHasher hasher, IJwtTokenGenerator tokenGenerator)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _hasher = hasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken ct)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
                throw new ValidationException("Invalid credentials");

            var token = _tokenGenerator.GenerateToken(user);

            return new AuthResponse(user.Id, user.Email, token, user.Role);
        }
    }
}
