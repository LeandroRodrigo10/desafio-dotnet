using AutoMapper;

// Application
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.SearchSales;

// WebApi DTOs
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    /// <summary>
    /// AutoMapper profile para mapear DTOs de Sales (WebApi) <-> Commands/Results (Application).
    /// </summary>
    public sealed class SalesProfile : Profile
    {
        public SalesProfile()
        {
            // CREATE
            CreateMap<CreateSaleRequest, CreateSaleCommand>();
            CreateMap<CreateSaleItemRequest, CreateSaleCommand.CreateSaleItemDto>();

            // Itens na resposta
            CreateMap<CreateSaleResult.SaleItemResult, CreateSaleResponse.SaleItemResponse>();

            // Resultado completo -> Resposta completa
            CreateMap<CreateSaleResult, CreateSaleResponse>()
                .ForMember(d => d.Number, o => o.MapFrom(s => s.Number))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
                .ForMember(d => d.Customer, o => o.MapFrom(s => s.Customer))
                .ForMember(d => d.Branch, o => o.MapFrom(s => s.Branch))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.Total, o => o.MapFrom(s => s.TotalAmount))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

            // UPDATE
            // Observação: o Id é atribuído no controller (route), então não mapeamos aqui.
            CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
            CreateMap<UpdateSaleResult, UpdateSaleResponse>();

            // DELETE (apenas resposta de sucesso)
            CreateMap<DeleteSaleResult, DeleteSaleResponse>();

            // (Opcional) GET/SEARCH -> Se você tiver DTOs específicos na WebApi, adicione aqui.
            // Ex.: CreateMap<GetSaleResult, GetSaleResponse>();
            // Ex.: CreateMap<SearchSalesResult, SearchSalesResponse>();
        }
    }
}
