using System.Text.Json;

namespace WiredBrainCoffee.CustomersApp.Configuration
{
    public class RepoConfig
    {
        public string CustomersFilePath { get; set; } = string.Empty;
        public string ProductsFilePath { get; set; } = string.Empty;
    }
}
