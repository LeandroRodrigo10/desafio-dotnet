using System.Linq;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public sealed class CreateSaleApplicationMappingProfile : Profile
    {
        public CreateSaleApplicationMappingProfile()
        {
            // Command -> Entity (construindo a agregação corretamente)
            CreateMap<CreateSaleCommand, Sale>()
                .ConstructUsing(src => new Sale(src.Number, src.Date, src.Customer, src.Branch))
                .AfterMap((src, dest) =>
                {
                    // Garante estado limpo e adiciona via método de domínio
                    if (dest.Items.Any())
                        dest.ClearItems();

                    if (src.Items != null)
                    {
                        foreach (var i in src.Items)
                            dest.AddItem(i.Sku, i.Name, i.Quantity, i.UnitPrice, i.Discount);
                    }
                });

            // Entity -> Result (inclui itens e enum -> string)
            CreateMap<SaleItem, CreateSaleResult.SaleItemResult>();

            CreateMap<Sale, CreateSaleResult>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
        }
    }
}
