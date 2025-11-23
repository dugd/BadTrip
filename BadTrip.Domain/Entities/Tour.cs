using BadTrip.Domain.Common;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.ValueObjects;

namespace BadTrip.Domain.Entities
{
    /// <summary>
    /// Tour represents a travel package offered by a TourOperator.
    /// Manages capacity tracking with optimistic concurrency control to prevent overbooking.
    /// Tours are immutable after bookings are made - only available spots decrease.
    /// </summary>
    public class Tour : BaseEntity
    {
        public string Title { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public Money Price { get; private set; } = null!;
        public string ImageUrl { get; private set; } = null!;

        /// <summary>
        /// Foreign key to Hotel entity. Optional - tour may exist without hotel assignment.
        /// </summary>
        public Guid? HotelId { get; private set; }

        /// <summary>
        /// Maximum number of participants allowed for this tour.
        /// </summary>
        public int MaxParticipants { get; private set; }

        /// <summary>
        /// Number of spots already sold/reserved. Incremented atomically during booking.
        /// </summary>
        public int SoldSpots { get; private set; }

        /// <summary>
        /// Computed property: remaining capacity for new bookings.
        /// </summary>
        public int AvailableSpots => MaxParticipants - SoldSpots;

        /// <summary>
        /// Tour start date. Must be in the future at creation.
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Tour end date. Must be after start date.
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Foreign key to User entity (TourOperator who created this tour).
        /// </summary>
        public Guid OperatorId { get; private set; }

        /// <summary>
        /// Optimistic concurrency token. Updated on every modification to prevent race conditions
        /// during booking (e.g., two users trying to book the last available spot).
        /// </summary>
        public byte[] RowVersion { get; private set; } = null!;

        // Navigation properties
        public Hotel? Hotel { get; private set; }
        public User Operator { get; private set; } = null!;

        // EF Core constructor
        private Tour() { }

        /// <summary>
        /// Factory method to create a new Tour with validation.
        /// </summary>
        public static Tour Create(
            string title,
            string description,
            Money price,
            string imageUrl,
            Guid? hotelId,
            int maxParticipants,
            DateTime startDate,
            DateTime endDate,
            Guid operatorId)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Title is required.");

            if (title.Length > 200)
                throw new ValidationException("Title cannot exceed 200 characters.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ValidationException("Description is required.");

            if (description.Length > 2000)
                throw new ValidationException("Description cannot exceed 2000 characters.");

            if (price == null)
                throw new ValidationException("Price is required.");

            if (price.Amount <= 0)
                throw new ValidationException("Price must be greater than zero.");

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ValidationException("Image URL is required.");

            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                throw new ValidationException("Image URL must be a valid absolute URL.");

            if (maxParticipants <= 0)
                throw new ValidationException("Max participants must be greater than zero.");

            if (maxParticipants > 1000)
                throw new ValidationException("Max participants cannot exceed 1000.");

            if (startDate <= DateTime.UtcNow)
                throw new ValidationException("Start date must be in the future.");

            if (endDate <= startDate)
                throw new ValidationException("End date must be after start date.");

            if (operatorId == Guid.Empty)
                throw new ValidationException("Operator ID is required.");

            return new Tour
            {
                Title = title,
                Description = description,
                Price = price,
                ImageUrl = imageUrl,
                HotelId = hotelId,
                MaxParticipants = maxParticipants,
                SoldSpots = 0,
                StartDate = startDate,
                EndDate = endDate,
                OperatorId = operatorId
            };
        }

        /// <summary>
        /// Updates tour details. Only allowed if tour hasn't started.
        /// Cannot change MaxParticipants below current SoldSpots.
        /// </summary>
        public void Update(
            string title,
            string description,
            Money price,
            string imageUrl,
            Guid? hotelId,
            int maxParticipants,
            DateTime startDate,
            DateTime endDate)
        {
            // Validate update is allowed
            if (StartDate <= DateTime.UtcNow)
                throw new DomainException("Cannot update a tour that has already started.");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Title is required.");

            if (title.Length > 200)
                throw new ValidationException("Title cannot exceed 200 characters.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ValidationException("Description is required.");

            if (description.Length > 2000)
                throw new ValidationException("Description cannot exceed 2000 characters.");

            if (price == null)
                throw new ValidationException("Price is required.");

            if (price.Amount <= 0)
                throw new ValidationException("Price must be greater than zero.");

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ValidationException("Image URL is required.");

            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                throw new ValidationException("Image URL must be a valid absolute URL.");

            if (maxParticipants <= 0)
                throw new ValidationException("Max participants must be greater than zero.");

            if (maxParticipants > 1000)
                throw new ValidationException("Max participants cannot exceed 1000.");

            if (maxParticipants < SoldSpots)
                throw new DomainException($"Cannot reduce max participants to {maxParticipants} - {SoldSpots} spots already sold.");

            if (startDate <= DateTime.UtcNow)
                throw new ValidationException("Start date must be in the future.");

            if (endDate <= startDate)
                throw new ValidationException("End date must be after start date.");

            Title = title;
            Description = description;
            Price = price;
            ImageUrl = imageUrl;
            HotelId = hotelId;
            MaxParticipants = maxParticipants;
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Atomically reserves spots for a booking. Throws if insufficient capacity.
        /// Uses optimistic concurrency (RowVersion) to prevent race conditions.
        /// </summary>
        public void ReserveSpots(int count)
        {
            if (count <= 0)
                throw new ValidationException("Spot count must be greater than zero.");

            if (SoldSpots + count > MaxParticipants)
                throw new DomainException($"Insufficient capacity. Requested: {count}, Available: {AvailableSpots}");

            if (StartDate <= DateTime.UtcNow)
                throw new DomainException("Cannot book a tour that has already started.");

            SoldSpots += count;
        }

        /// <summary>
        /// Returns spots when a booking is cancelled. Used for refunds.
        /// </summary>
        public void ReturnSpots(int count)
        {
            if (count <= 0)
                throw new ValidationException("Spot count must be greater than zero.");

            if (count > SoldSpots)
                throw new DomainException($"Cannot return {count} spots - only {SoldSpots} are sold.");

            SoldSpots -= count;
        }

        /// <summary>
        /// Checks if tour is available for booking (has capacity and hasn't started).
        /// </summary>
        public bool IsAvailable()
        {
            return AvailableSpots > 0 && StartDate > DateTime.UtcNow;
        }
    }
}
