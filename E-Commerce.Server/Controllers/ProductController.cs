using E_Commerce.Server.Context;
using E_Commerce.Server.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Commerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public ProductController(ECommerceDbContext context)
        {
            _context = context;
        }

        // product list - /api/product
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                // Kullanıcı kimliğini al (JWT içinden)
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.Claims.Any())
                {
                    return Unauthorized("Token çözülemedi.");
                }

                // Kullanıcı bilgilerini log'a yazdır
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = identity.FindFirst(ClaimTypes.Email)?.Value;
                var userRole = identity.FindFirst(ClaimTypes.Role)?.Value;

                Console.WriteLine($"Token Bilgileri - UserId: {userId}, Email: {userEmail}, Role: {userRole}");

                // Eğer userId boşsa yetkilendirme başarısızdır
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Kullanıcı kimliği alınamadı.");
                }

                // Veritabanından ürünleri getir
                var products = await _context.Products.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token doğrulama hatası: {ex.Message}");
                return Unauthorized("Token doğrulanamadı.");
            }
        }

        // get product detail - /api/product/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // add product
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // update product
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.Stock = updatedProduct.Stock;
            product.ImageUrl = updatedProduct.ImageUrl;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // delete product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
