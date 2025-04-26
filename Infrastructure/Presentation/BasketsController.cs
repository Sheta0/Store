using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketsController(IBasketService basketService) : ControllerBase
    {
        [HttpGet] // GET: api/baskets?id=asd
        public async Task<IActionResult> GetBasketById(string id)
        {
            var result = await basketService.GetBasketAsync(id);
            return Ok(result);
        }

        [HttpPost] // POST: api/baskets
        public async Task<IActionResult> UpdateBasket(BasketDto basketDto)
        {
            var result = await basketService.UpdateBasketAsync(basketDto);
            return Ok(result);
        }

        [HttpDelete] // DELETE: api/baskets?id=asd
        public async Task<IActionResult> DeleteBasket(string id)
        {
            await basketService.DeleteBasketAsync(id);
            return NoContent();
        }
    }
}
