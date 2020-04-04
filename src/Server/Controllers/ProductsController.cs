using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Marketplace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // Product, ProductTransactions, ProductServers
        private readonly IProductsRepository productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            this.productsRepository = productsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            return Ok(await productsRepository.GetProductsAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostProductAsync([FromBody] Product product)
        {
            int productId = await productsRepository.CreateProductAsync(product);
            if (productId == 0)
                return BadRequest();
            else
                return Ok(productId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> PutProductAsync([FromBody] Product product)
        {
            await productsRepository.UpdateProductAsync(product);
            return Ok();
        }
    }
}