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
            CreateMap<CreateSaleResult, CreateSaleResponse>();

            // UPDATE
            // Observação: o Id é atribuído no controller (route), então não mapeamos aqui.
            CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
            CreateMap<UpdateSaleResult, UpdateSaleResponse>();

            // DELETE (apenas resposta de sucesso)
            CreateMap<DeleteSaleResult, DeleteSaleResponse>();

            // (Opcional) GET/SEARCH -> Se você tiver DTOs específicos na WebApi, adicione aqui.
            // Ex.: CreateMap<GetSaleResult, GetSaleResponse>();  // se criar DTO de resposta
            // Ex.: CreateMap<SearchSalesResult, SearchSalesResponse>();
        }
    }
}
