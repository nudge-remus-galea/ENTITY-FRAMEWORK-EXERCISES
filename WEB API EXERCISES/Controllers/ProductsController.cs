using Microsoft.AspNetCore.Mvc;
using WEB_API_EXERCISES.Models;

namespace WEB_API_EXERCISES.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<Product> _products = new List<Product>
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 1",
                Description = "Description of Product 1",
                Ratings = [4, 5, 3],
                CreatedOn = DateTime.UtcNow.AddDays(-5)
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 2",
                Description = "Description of Product 2",
                Ratings = [5, 5, 5],
                CreatedOn = DateTime.UtcNow.AddDays(-3)
            }
        };

        [HttpGet]
        public IEnumerable<Product> GetAllProducts()
        {
            return _products.ToArray();
        }

        [HttpGet("search/{keyword}")]
        public IEnumerable<Product> GetProductsByKeyword(string keyword)
        {
            List<Product> result = new List<Product>();
            foreach (var product in _products)
            {
                if (product.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    product.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product);
                }
            }
            return result.ToArray();
        }

        [HttpGet("sorted-by-rating")]
        public IEnumerable<Product> GetProductsSortedByRating([FromQuery] string sortOrder = "asc")
        {
            List<Product> sortedProducts = _products.ToList();
            if (sortOrder.ToLower() == "desc")
            {
                sortedProducts.Sort((x, y) => y.Ratings.Average().CompareTo(x.Ratings.Average()));
            }
            else
            {
                sortedProducts.Sort((x, y) => x.Ratings.Average().CompareTo(y.Ratings.Average()));
            }
            return sortedProducts.ToArray();
        }

        [HttpGet("recent")]
        public IActionResult GetMostRecentProduct()
        {
            Product mostRecentProduct = null;
            foreach (var product in _products)
            {
                if (mostRecentProduct == null || product.CreatedOn > mostRecentProduct.CreatedOn)
                {
                    mostRecentProduct = product;
                }
            }
            if (mostRecentProduct == null)
                return NotFound();

            return Ok(mostRecentProduct);
        }

        [HttpGet("oldest")]
        public IActionResult GetOldestProduct()
        {
            Product oldestProduct = null;
            foreach (var product in _products)
            {
                if (oldestProduct == null || product.CreatedOn < oldestProduct.CreatedOn)
                {
                    oldestProduct = product;
                }
            }
            if (oldestProduct == null)
                return NotFound();

            return Ok(oldestProduct);
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (product == null)
                return BadRequest("Product data is invalid.");

            foreach (var existingProduct in _products)
            {
                if (existingProduct.Id == product.Id)
                    return Conflict("Product with the same Id already exists.");
            }

            product.CreatedOn = DateTime.UtcNow;
            _products.Add(product);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult EditProduct(Guid id, [FromBody] Product product)
        {
            if (product == null || id != product.Id)
                return BadRequest("Product data is invalid.");

            foreach (var existingProduct in _products)
            {
                if (existingProduct.Id == id)
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Ratings = product.Ratings;
                    return NoContent();
                }
            }
            return NotFound("Product not found.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(Guid id)
        {
            for (int i = 0; i < _products.Count; i++)
            {
                if (_products[i].Id == id)
                {
                    _products.RemoveAt(i);
                    return NoContent();
                }
            }
            return NotFound("Product not found.");
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public IActionResult GetProductById(Guid id)
        {
            foreach (var product in _products)
            {
                if (product.Id == id)
                    return Ok(product);
            }
            return NotFound("Product not found.");
        }
    }
}
