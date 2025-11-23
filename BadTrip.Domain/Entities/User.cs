using BadTrip.Domain.Common;
using BadTrip.Domain.Enums;
using BadTrip.Domain.Exceptions;

namespace BadTrip.Domain.Entities
{
    public class User : BaseEntity
    {
        // Auth
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }

        // Meta
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        // Personal
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? PhoneNumber { get; private set; } // Not set for Admin


        // Tourist specifics
        public DateTime? DateOfBirth { get; private set; }

        // TourOperator specifics
        public string? CompanyName { get; private set; }

        protected User()
        {
        }

        private User(string email, string passwordHash, UserRole role, string firstName, string lastName)
        {
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            FirstName = firstName;
            LastName = lastName;
        }

        public static User CreateTourist(
            string email,
            string passwordHash,
            string firstName,
            string lastName,
            string phoneNumber,
            DateTime dateOfBirth)
        {
            var user = new User(email, passwordHash, UserRole.Tourist, firstName, lastName);

            if (dateOfBirth > DateTime.UtcNow.AddYears(-18))
                throw new ValidationException("Tourist must be an adult.");

            user.PhoneNumber = phoneNumber;
            user.DateOfBirth = dateOfBirth;

            return user;
        }

        public static User CreateOperator(
            string email,
            string passwordHash,
            string firstName,
            string lastName,
            string phoneNumber,
            string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ValidationException("Company name is required.");

            var user = new User(email, passwordHash, UserRole.TourOperator, firstName, lastName);

            user.PhoneNumber = phoneNumber;
            user.CompanyName = companyName;

            return user;
        }

        public static User CreateAdmin(string email, string passwordHash, string firstName, string lastName)
        {
            return new User(email, passwordHash, UserRole.Admin, firstName, lastName);
        }

        public void UpdateRefreshToken(string token, DateTime expiry)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiry;
        }   
    }
}
