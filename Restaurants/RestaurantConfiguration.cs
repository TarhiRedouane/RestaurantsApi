using System;

namespace Restaurants
{
    public class RestaurantConfiguration
    {
        public readonly string ConnectionString;

        public RestaurantConfiguration(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            ConnectionString = connectionString;
        }
    }
}