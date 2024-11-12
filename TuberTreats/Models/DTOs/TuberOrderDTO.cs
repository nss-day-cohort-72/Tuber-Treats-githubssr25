namespace TuberTreats.Models;
public class TuberOrderDTO
{
    public int Id { get; set; }
    public DateTime OrderPlacedOnDate { get; set; }
    public CustomerDTO Customer { get; set; } // Holds full customer data
    public TuberDriverDTO? TuberDriver { get; set; } // Holds full driver data, nullable
    public DateTime? DeliveredOnDate { get; set; }
    public List<ToppingDTO> Toppings { get; set; } = new List<ToppingDTO>(); // List of toppings
}
