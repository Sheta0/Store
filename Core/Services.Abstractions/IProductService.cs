using Shared;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IProductService
    {
        // Get All Products
        //Task<IEnumerable<ProductDto>> GetAllProductsAsync(int? brandId, int? typeId, string? sort, int pageIndex, int pageSize);
        Task<PaginationResponse<ProductDto>> GetAllProductsAsync(ProductSpecificationsParameters specParams);
        // Get Product By Id
        Task<ProductDto?> GetProductByIdAsync(int id);
        // Get All Types
        Task<IEnumerable<TypeDto>> GetAllTypesAsync();
        // Get All Brands
        Task<IEnumerable<BrandDto>> GetAllBrandsAsync();
    }
}
