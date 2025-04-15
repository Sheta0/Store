using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using Services.Abstractions;
using Services.Specifications;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
    {

        public async Task<PaginationResponse<ProductDto>> GetAllProductsAsync(ProductSpecificationsParameters specParams)
        {
            var spec = new ProductWithTypesAndBrandsSpecifications(specParams);

            var products = await unitOfWork.GetRepository<Product, int>().GetAllAsync(spec);

            var specCount = new ProductWithCountSpecifications(specParams);

            var count = await unitOfWork.GetRepository<Product, int>().CountAsync(specCount);

            var result = mapper.Map<IEnumerable<ProductDto>>(products);

            return new PaginationResponse<ProductDto>(specParams.PageIndex, specParams.PageSize, count, result);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var spec = new ProductWithTypesAndBrandsSpecifications(id);
            var product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec);
            if (product is null) return null;

            var result = mapper.Map<ProductDto>(product);
            return result;

        }
        public async Task<IEnumerable<TypeDto>> GetAllTypesAsync()
        {
            var types = await unitOfWork.GetRepository<ProductType, int>().GetAllAsync();
            var result = mapper.Map<IEnumerable<TypeDto>>(types);
            return result;
        }
        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync();
            var result = mapper.Map<IEnumerable<BrandDto>>(brands);
            return result;
        }


    }
}
