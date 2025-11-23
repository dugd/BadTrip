using BadTrip.Domain.Common;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.ValueObjects;

namespace BadTrip.Domain.Entities
{
    public class Hotel : BaseEntity
    {
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public int Stars { get; private set; }
        public string ImageUrl { get; private set; }

        protected Hotel()
        {
        }

        private Hotel(string name, Address address, int stars, string imageUrl)
        {
            Name = name;
            Address = address;
            Stars = stars;
            ImageUrl = imageUrl;
        }

        public static Hotel Create(string name, Address address, int stars, string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Hotel name is required.");

            if (address == null)
                throw new ValidationException("Address is required.");

            if (stars < 1 || stars > 5)
                throw new ValidationException("Stars must be between 1 and 5.");

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ValidationException("Image URL is required.");

            return new Hotel(name, address, stars, imageUrl);
        }

        public void Update(string name, Address address, int stars, string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Hotel name is required.");

            if (address == null)
                throw new ValidationException("Address is required.");

            if (stars < 1 || stars > 5)
                throw new ValidationException("Stars must be between 1 and 5.");

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ValidationException("Image URL is required.");

            Name = name;
            Address = address;
            Stars = stars;
            ImageUrl = imageUrl;
        }
    }
}
