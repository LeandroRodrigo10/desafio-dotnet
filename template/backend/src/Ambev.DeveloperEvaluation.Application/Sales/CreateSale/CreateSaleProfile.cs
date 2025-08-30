using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleProfile : Profile
    {
        public CreateSaleProfile()
        {
            // Command -> Entity
            CreateMap<CreateSaleCommand, Sale>()
                .ForMember(dest => dest.Items, opt => opt.Ignore()) // <<< ignore a coleção
                .ConstructUsing(cmd => new Sale(cmd.Number, cmd.Date, cmd.Customer, cmd.Branch))
                .AfterMap((cmd, sale) =>
                {
                    foreach (var item in cmd.Items)
                    {
                        sale.AddItem(item.Sku, item.Name, item.Quantity, item.UnitPrice, item.Discount);
                    }
                });

            // Entity -> Result
            CreateMap<Sale, CreateSaleResult>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<SaleItem, CreateSaleResult.SaleItemResult>();
        }
    }
}
