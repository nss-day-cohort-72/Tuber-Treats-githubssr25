
using TuberTreats.Models;
using AutoMapper;
using TuberTreats.Mapper;
using Microsoft.AspNetCore.Mvc;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

 // Scans for profiles automatically
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Lists to store each entity type
List<TuberDriver> drivers = new List<TuberDriver>();
List<Customer> customers = new List<Customer>();
List<Topping> toppings = new List<Topping>();
List<TuberOrder> orders = new List<TuberOrder>();
List<TuberTopping> tuberToppings = new List<TuberTopping>();

drivers.Add(new TuberDriver { Id = 1, Name = "Driver One" });
drivers.Add(new TuberDriver { Id = 2, Name = "Driver Two" });
drivers.Add(new TuberDriver { Id = 3, Name = "Driver Three" });

customers.Add(new Customer { Id = 1, Name = "Customer One", Address = "123 Potato Lane" });
customers.Add(new Customer { Id = 2, Name = "Customer Two", Address = "456 Spud Street" });
customers.Add(new Customer { Id = 3, Name = "Customer Three", Address = "789 Tuber Terrace" });
customers.Add(new Customer { Id = 4, Name = "Customer Four", Address = "101 Potato Ave" });
customers.Add(new Customer { Id = 5, Name = "Customer Five", Address = "202 Potato Blvd" });

toppings.Add(new Topping { Id = 1, Name = "Cheese" });
toppings.Add(new Topping { Id = 2, Name = "Sour Cream" });
toppings.Add(new Topping { Id = 3, Name = "Chives" });
toppings.Add(new Topping { Id = 4, Name = "Bacon Bits" });
toppings.Add(new Topping { Id = 5, Name = "Butter" });

orders.Add(new TuberOrder { Id = 1, OrderPlacedOnDate = DateTime.Now, CustomerId = 1, TuberDriverId = 2, DeliveredOnDate = new DateTime(2024, 11, 8, 13, 30, 0), Toppings = new List<Topping> { toppings[0], toppings[1] } });
orders.Add(new TuberOrder { Id = 2, OrderPlacedOnDate = DateTime.Now, CustomerId = 2, TuberDriverId = 1, DeliveredOnDate = new DateTime(2024, 11, 8, 12, 45, 0), Toppings = new List<Topping> { toppings[2], toppings[3] } });
orders.Add(new TuberOrder { Id = 3, OrderPlacedOnDate = DateTime.Now, CustomerId = 3, TuberDriverId = 3, DeliveredOnDate =  new DateTime(2024, 11, 8, 11, 0, 0), Toppings = new List<Topping> { toppings[4] } });

orders.Add(new TuberOrder { Id = 4, OrderPlacedOnDate = DateTime.Now, CustomerId = 4, TuberDriverId = null, DeliveredOnDate = null, Toppings = new List<Topping> { toppings[1], toppings[4] } });
orders.Add(new TuberOrder { Id = 5, OrderPlacedOnDate = DateTime.Now, CustomerId = 5, TuberDriverId = null, DeliveredOnDate = null, Toppings = new List<Topping> { toppings[0], toppings[3] } });

tuberToppings.Add(new TuberTopping { Id = 1, TuberOrderId = 1, ToppingId = 1 });
tuberToppings.Add(new TuberTopping { Id = 2, TuberOrderId = 1, ToppingId = 2 });
tuberToppings.Add(new TuberTopping { Id = 3, TuberOrderId = 2, ToppingId = 3 });
tuberToppings.Add(new TuberTopping { Id = 4, TuberOrderId = 2, ToppingId = 4 });
tuberToppings.Add(new TuberTopping { Id = 5, TuberOrderId = 3, ToppingId = 5 });

//add endpoints here

app.MapGet("/api/tuberorders", () => Results.Ok(orders));

app.MapGet("/api/tuberorders/{id}", (int id, IMapper mapper) =>
{

    TuberOrder tuberOrder = orders.FirstOrDefault(order => order.Id == id);

    Customer ourCustomer = customers.FirstOrDefault(customer => customer.Id == tuberOrder.CustomerId);

    TuberDriver tuberDrive = drivers.FirstOrDefault(driver => driver.Id == tuberOrder.TuberDriverId);



    // Map the TuberOrder to TuberOrderDTO
    var tuberOrderDTO = mapper.Map<TuberOrderDTO>(tuberOrder);

    tuberOrderDTO.Customer = mapper.Map<CustomerDTO>(ourCustomer);

    if (tuberDrive != null)
    {
        tuberOrderDTO.TuberDriver = mapper.Map<TuberDriverDTO>(tuberDrive);
    }

return Results.Ok(tuberOrderDTO);


});


app.MapPost("api/tuberorders", (TuberOrderCreateDTO newTuberOrder, IMapper mapper) => {

List<Topping> tuberOrderToppingList = newTuberOrder.ToppingIds
    .Select(eachToppingId => toppings.FirstOrDefault(eachTopping => eachTopping.Id == eachToppingId)).ToList();

DateTime currentTime = DateTime.Now;

   int newTuberOrderId = (orders != null && orders.Any()) ? orders.Max(order => order.Id) + 1 : 1;

TuberOrder ourFinalTuberOrder = mapper.Map<TuberOrder>(newTuberOrder);

ourFinalTuberOrder.Toppings = mapper.Map<List<Topping>>(tuberOrderToppingList);

 ourFinalTuberOrder.OrderPlacedOnDate = currentTime;

 ourFinalTuberOrder.Id = newTuberOrderId;

    orders.Add(ourFinalTuberOrder);


return Results.Created("api/tuberorders", ourFinalTuberOrder);

});



app.MapPut("api/tuberorders/{id}", (int id, [FromBody] TuberOrderCreateDTO tuberOrderDTO, IMapper mapper) => 
{     
    if (tuberOrderDTO.TuberDriverId == null)
    {
        return Results.BadRequest("Driver ID is required to assign a driver.");
    }
    int ourTuberDriverId = (int)tuberOrderDTO.TuberDriverId;
    // Find the TuberOrder by ID
    TuberOrder orderToComplete = orders.FirstOrDefault(order => order.Id == id);
    if (orderToComplete == null)
    {
        return Results.NotFound($"Order with ID {id} not found.");
    }
    // Find the TuberDriver by ID
    TuberDriver ourDriver = drivers.FirstOrDefault(driver => driver.Id == ourTuberDriverId);
    if (ourDriver == null)
    {
        return Results.NotFound($"Driver with ID {ourTuberDriverId} not found.");
    }
    // Update the order with the driver ID
    orderToComplete.TuberDriverId = ourTuberDriverId;

    orderToComplete.Toppings = tuberOrderDTO.ToppingIds.
    Select(toppingIdToTransformToTopping => toppings.FirstOrDefault(eachTopping => eachTopping.Id == toppingIdToTransformToTopping))
    .Where(topping => topping != null)
    .ToList();


    // Map the updated TuberOrder to TuberOrderDTO
    var tuberOrderDTOResult = mapper.Map<TuberOrderDTO>(orderToComplete);
    // Manually set the TuberDriver in TuberOrderDTO
    tuberOrderDTOResult.TuberDriver = mapper.Map<TuberDriverDTO>(ourDriver);

    tuberOrderDTOResult.Customer = mapper.Map<CustomerDTO>(customers.FirstOrDefault(customer => customer.Id == tuberOrderDTO.CustomerId));


    return Results.Ok(tuberOrderDTOResult);
});


app.MapPut("api/tuberorders/{id}/complete", (int id) =>
{
    // Find the order by ID
    TuberOrder orderToComplete = orders.FirstOrDefault(order => order.Id == id);

    if (orderToComplete == null)
    {
        // Return a 404 Not Found if the order doesn't exist
        return Results.NotFound($"Order with ID {id} not found.");
    }

    // Set the DeliveredOnDate to the current time to mark it as completed
    orderToComplete.DeliveredOnDate = DateTime.Now;

    // Return the updated order as a response
    return Results.Ok(orderToComplete);
});

// Assuming `toppings` is a list of Topping objects initialized in your application

// GET: /toppings - Get all toppings
app.MapGet("/toppings", (IMapper mapper) => 
{
    // Map the list of Topping objects to ToppingDTO objects
    var toppingDTOs = mapper.Map<List<ToppingDTO>>(toppings);
    return Results.Ok(toppingDTOs);
});

// GET: /toppings/{id} - Get topping by ID
app.MapGet("/toppings/{id}", (int id, IMapper mapper) =>
{
    // Find the topping by ID
    var topping = toppings.FirstOrDefault(t => t.Id == id);

    // If the topping is not found, return a 404 Not Found
    if (topping == null)
    {
        return Results.NotFound($"Topping with ID {id} not found.");
    }

    // Map the Topping object to ToppingDTO
    var toppingDTO = mapper.Map<ToppingDTO>(topping);
    return Results.Ok(toppingDTO);
});

// Assuming `tuberToppings` is a list of TuberTopping objects initialized in your application
app.MapGet("/tubertoppings", (IMapper mapper) => 
{
    // Map the list of TuberTopping objects to TuberToppingDTO objects
    var tuberToppingDTOs = mapper.Map<List<TuberToppingDTO>>(tuberToppings);
    return Results.Ok(tuberToppingDTOs);
});



app.MapPost("/tubertoppings/add/", ([FromBody] TuberToppingDTO tuberToppingDTO, IMapper mapper) => 
{

TuberOrder neededTuberOrder = orders.FirstOrDefault(eachOrder => eachOrder.Id == tuberToppingDTO.TuberOrderId);

Topping neededTopping = toppings.FirstOrDefault(eachTopping => eachTopping.Id == tuberToppingDTO.ToppingId);

int tuberToppingId = (tuberToppings != null && tuberToppings.Any()) ? tuberToppings.Max(eachTopping => eachTopping.Id) + 1 : 1;

TuberTopping newTuberTopping = new TuberTopping{
    Id = tuberToppingId,
    TuberOrderId = tuberToppingDTO.TuberOrderId,
    ToppingId = tuberToppingDTO.ToppingId
};

tuberToppings.Add(newTuberTopping);


TuberToppingDTO returnedTuberTopping = mapper.Map<TuberToppingDTO>(newTuberTopping);

return Results.Created("tubertoppings/add", returnedTuberTopping);

});


app.MapDelete("/tubertoppings/remove", ([FromBody] TuberToppingDTO tuberToppingDTO) =>
{
    // Find the TuberTopping based on TuberOrderId and ToppingId in DTO
    TuberTopping toppingToRemove = tuberToppings
        .FirstOrDefault(tt => tt.TuberOrderId == tuberToppingDTO.TuberOrderId && tt.ToppingId == tuberToppingDTO.ToppingId);

    if (toppingToRemove == null)
    {
        return Results.NotFound($"Topping with ID {tuberToppingDTO.ToppingId} for order {tuberToppingDTO.TuberOrderId} not found.");
    }

    tuberToppings.Remove(toppingToRemove);

    return Results.NoContent();
});


app.MapGet("/customers", (IMapper mapper) => {
    
var customerDTOs = mapper.Map<List<CustomerDTO>>(customers);
return Results.Ok(customerDTOs);

});


//without DTOs 
app.MapGet("/customers/{id}", (int id) =>
{
    // Retrieve the customer and ensure it exists
    var ourCustomer = customers.SingleOrDefault(c => c.Id == id);
    if (ourCustomer == null)
    {
        return Results.NotFound($"Customer with ID {id} not found.");
    }

    // Retrieve all orders for the customer and assign them directly to TuberOrders
    ourCustomer.TuberOrders = orders.Where(order => order.CustomerId == id).ToList();

    return Results.Ok(ourCustomer);
});


app.MapPost("/customers", ([FromBody] CustomerCreateDTO customerCreateDTO, IMapper mapper) => 
{

int generatedCustomerId = (customers != null && customers.Any()) ? customers.Max(c => c.Id) + 1 : 1;

    var customer = mapper.Map<Customer>(customerCreateDTO);

    customer.Id = generatedCustomerId;

    customer.TuberOrders = new List<TuberOrder>();

    customers.Add(customer);

    CustomerDTO customerDTO = mapper.Map<CustomerDTO>(customer);

return Results.Created("/customers", customerDTO);

});


app.MapDelete("/customers/{id}", (int id) =>
{
    // Find the customer by ID
    Customer ourCustomer = customers.SingleOrDefault(c => c.Id == id);
    if (ourCustomer == null)
    {
        return Results.NotFound("Customer not found.");
    }

    // Find all orders associated with this customer and remove the customer reference
    List<TuberOrder> tuberOrdersForCustomer = orders.Where(order => order.CustomerId == id).ToList();
    foreach (var order in tuberOrdersForCustomer)
    {
        order.CustomerId = 0; // Remove the reference to the customer
    }


    customers.Remove(ourCustomer);

    return Results.NoContent();
});

//non DTO version 
app.MapGet("/tuberdrivers", () =>
{
    // Step 1: Populate the TuberDeliveries for each driver directly
    foreach (var driver in drivers)
    {
        driver.TuberDeliveries = orders
            .Where(order => order.TuberDriverId == driver.Id)
            .ToList();
    }

    return Results.Ok(drivers);
});



//how to do it without DTOs 
app.MapGet("/tuberdrivers/{id}", (int id) => 
{
    // Find the TuberDriver by ID
    TuberDriver ourTuberDriver = drivers.SingleOrDefault(driver => driver.Id == id);
    if (ourTuberDriver == null)
    {
        return Results.NotFound($"Driver with ID {id} not found.");
    }

    // Populate TuberDeliveries directly with the orders that belong to this driver
    ourTuberDriver.TuberDeliveries = orders.Where(order => order.TuberDriverId == id).ToList();

    return Results.Ok(ourTuberDriver);
});

app.Run();
//don't touch or move this!
public partial class Program { }