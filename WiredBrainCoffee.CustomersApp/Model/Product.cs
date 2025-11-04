namespace WiredBrainCoffee.CustomersApp.Model
{
    public record Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
