public class TuberOrderCreateDTO
{
    public int CustomerId { get; set; }
    public int? TuberDriverId { get; set; }
    public List<int> ToppingIds { get; set; } = new List<int>(); // List of topping IDs for the order
}
