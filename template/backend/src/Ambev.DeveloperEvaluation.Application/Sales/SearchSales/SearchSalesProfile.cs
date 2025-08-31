using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.SearchSales
{
    public class SearchSalesProfile : Profile
    {
        public SearchSalesProfile()
        {
            CreateMap<Sale, SearchSalesResult.SaleSummary>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
