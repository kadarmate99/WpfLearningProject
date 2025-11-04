using System.Text.Json.Serialization;

namespace WiredBrainCoffee.CustomersApp.Model
{
    public record Customer
    {
        [JsonRequired]
        public int Id { get; set; }

        [JsonRequired]
        public string FirstName { get; set; } = string.Empty;

        [JsonRequired]
        public string LastName { get; set; } = string.Empty;

        [JsonRequired]
        public bool IsDeveloper { get; set; }
    }
}
