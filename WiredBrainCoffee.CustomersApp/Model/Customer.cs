    using System.Text.Json.Serialization;

    namespace WiredBrainCoffee.CustomersApp.Model
    {
        public record Customer
        {
            [JsonRequired]
            public int Id { get; set; }

            [JsonRequired]
            public string FirstName { get; set; } = default!;

            [JsonRequired]
            public string LastName { get; set; } = default!;

            [JsonRequired]
            public bool IsDeveloper { get; set; }
        }
    }
