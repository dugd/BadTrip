using BadTrip.Domain.Exceptions;

namespace BadTrip.Domain.ValueObjects;

public record Passenger
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string PassportNumber { get; init; }
    public DateTime DateOfBirth { get; init; }

    public Passenger(string firstName, string lastName, string passportNumber, DateTime dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ValidationException("Passenger first name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ValidationException("Passenger last name is required");

        if (string.IsNullOrWhiteSpace(passportNumber))
            throw new ValidationException("Passenger passport number is required");

        if (dateOfBirth >= DateTime.UtcNow)
            throw new ValidationException("Passenger date of birth must be in the past");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PassportNumber = passportNumber.Trim();
        DateOfBirth = dateOfBirth;
    }

    public int GetAge()
    {
        var today = DateTime.UtcNow;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

    public bool IsAdult() => GetAge() >= 18;
}
