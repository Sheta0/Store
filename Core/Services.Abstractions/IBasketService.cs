using Shared.Dtos;

namespace Services.Abstractions
{
    public interface IBasketService
    {
        Task<BasketDto?> GetBasketAsync(string id);
        Task<BasketDto?> UpdateBasketAsync(BasketDto basketdto);
        Task<bool> DeleteBasketAsync(string id);
    }
}