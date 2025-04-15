using Domain.Interfaces;
using Domain.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications
{
    public class ProductWithTypesAndBrandsSpecifications : BaseSpecifications<Product, int>
    {
        public ProductWithTypesAndBrandsSpecifications(int id) : base(P => P.Id == id)
        {
            ApplyIncludes();
        }

        public ProductWithTypesAndBrandsSpecifications(ProductSpecificationsParameters specParams) 
            : base(
                  P =>
                  (string.IsNullOrEmpty(specParams.Search) || P.Name.ToLower().Contains(specParams.Search.ToLower())) &&
                  (!specParams.BrandId.HasValue || P.BrandId == specParams.BrandId) &&
                  (!specParams.TypeId.HasValue || P.TypeId == specParams.TypeId)
                  )
        {
            ApplyIncludes();
            ApplySorting(specParams.Sort);
            ApplyPagination(specParams.PageIndex, specParams.PageSize);
        }

        private void ApplyIncludes()
        {
            AddInclude(P => P.ProductBrand);
            AddInclude(P => P.ProductType);
        }

        private void ApplySorting(string? sort)
        {
            if (string.IsNullOrEmpty(sort)) sort = "nameasc";
            switch (sort.ToLower())
            {
                case "namedesc":
                    AddOrderByDescending(P => P.Name);
                    break;
                case "priceasc":
                    AddOrderBy(P => P.Price);
                    break;
                case "pricedesc":
                    AddOrderByDescending(P => P.Price);
                    break;
                default:
                    AddOrderBy(P => P.Name);
                    break;
            }
        }
    }
}
