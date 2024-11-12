using AutoMapper;
using TuberTreats.Models;


namespace TuberTreats.Mapper;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Example mappings between model and DTO classes
        CreateMap<Customer, CustomerDTO>();
        CreateMap<CustomerCreateDTO, Customer>();

        CreateMap<TuberDriver, TuberDriverDTO>();
        CreateMap<TuberDriverCreateDTO, TuberDriver>();

        CreateMap<Topping, ToppingDTO>();
        CreateMap<ToppingCreateDTO, Topping>();

            CreateMap<TuberOrder, TuberOrderDTO>()
            .ForMember(dest => dest.Customer, opt => opt.Ignore()) // Manually mapped
            .ForMember(dest => dest.TuberDriver, opt => opt.Ignore()); // Manually mapped

        CreateMap<TuberOrderCreateDTO, TuberOrder>()
        .ForMember(dest => dest.Toppings, opt => opt.Ignore())
        .ForMember(dest => dest.OrderPlacedOnDate, opt => opt.Ignore());

        CreateMap<TuberTopping, TuberToppingDTO>();
    }
}
