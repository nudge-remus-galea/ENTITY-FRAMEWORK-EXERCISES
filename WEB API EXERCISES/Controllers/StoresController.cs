using Microsoft.AspNetCore.Mvc;
using WEB_API_EXERCISES.Models;

namespace WEB_API_EXERCISES.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoresController : ControllerBase
    {
        private static readonly List<Store> _stores = new List<Store>
        {
            new Store
            {
                Id = Guid.NewGuid(),
                Name = "Store 1",
                Country = "Country 1",
                City = "City 1",
                MonthlyIncome = 5000,
                OwnerName = "Owner 1",
                ActiveSince = DateTime.UtcNow.AddYears(-5)
            },
            new Store
            {
                Id = Guid.NewGuid(),
                Name = "Store 2",
                Country = "Country 2",
                City = "City 2",
                MonthlyIncome = 7000,
                OwnerName = "Owner 2",
                ActiveSince = DateTime.UtcNow.AddYears(-3)
            }
        };

        [HttpGet]
        public IEnumerable<Store> GetAllStores()
        {
            return _stores.ToArray();
        }

        [HttpGet("search/{keyword}")]
        public Store[] GetStoresByKeyword(string keyword)
        {
            List<Store> result = new List<Store>();
            foreach (var store in _stores)
            {
                if (store.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    store.Country.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    store.City.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(store);
                }
            }
            return result.ToArray();
        }

        [HttpGet("by-country/{country}")]
        public IEnumerable<Store> GetStoresByCountry(string country)
        {
            List<Store> result = new List<Store>();
            foreach (var store in _stores)
            {
                if (store.Country.Equals(country, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(store);
                }
            }
            return result.ToArray();
        }

        [HttpGet("by-city/{city}")]
        public IEnumerable<Store> GetStoresByCity(string city)
        {
            List<Store> result = new List<Store>();
            foreach (var store in _stores)
            {
                if (store.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(store);
                }
            }
            return result.ToArray();
        }

        [HttpGet("sorted-by-income")]
        public IEnumerable<Store> GetStoresSortedByIncome([FromQuery] string sortOrder = "asc")
        {
            List<Store> sortedStores = _stores.ToList();
            if (sortOrder.ToLower() == "desc")
            {
                sortedStores.Sort((x, y) => y.MonthlyIncome.CompareTo(x.MonthlyIncome));
            }
            else
            {
                sortedStores.Sort((x, y) => x.MonthlyIncome.CompareTo(y.MonthlyIncome));
            }
            return sortedStores.ToArray();
        }

        [HttpPut("transfer-ownership/{storeId}")]
        public IActionResult TransferOwnership(Guid storeId, [FromBody] string newOwnerName)
        {
            foreach (var store in _stores)
            {
                if (store.Id == storeId)
                {
                    store.OwnerName = newOwnerName;
                    return NoContent();
                }
            }
            return NotFound("Store not found.");
        }

        [HttpGet("oldest")]
        public IActionResult GetOldestStore()
        {
            Store oldestStore = null;
            foreach (var store in _stores)
            {
                if (oldestStore == null || store.ActiveSince < oldestStore.ActiveSince)
                {
                    oldestStore = store;
                }
            }
            if (oldestStore == null)
                return NotFound("No stores found.");

            return Ok(oldestStore);
        }

        [HttpPost]
        public IActionResult CreateStore([FromBody] Store store)
        {
            if (store == null)
                return BadRequest("Store data is invalid.");

            foreach (var existingStore in _stores)
            {
                if (existingStore.Id == store.Id)
                    return Conflict("Store with the same Id already exists.");
            }

            _stores.Add(store);
            return CreatedAtAction(nameof(GetStoreById), new { id = store.Id }, store);
        }

        [HttpPut("{id}")]
        public IActionResult EditStore(Guid id, [FromBody] Store store)
        {
            if (store == null || id != store.Id)
                return BadRequest("Store data is invalid.");

            foreach (var existingStore in _stores)
            {
                if (existingStore.Id == id)
                {
                    existingStore.Name = store.Name;
                    existingStore.Country = store.Country;
                    existingStore.City = store.City;
                    existingStore.MonthlyIncome = store.MonthlyIncome;
                    return NoContent();
                }
            }
            return NotFound("Store not found.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStore(Guid id)
        {
            for (int i = 0; i < _stores.Count; i++)
            {
                if (_stores[i].Id == id)
                {
                    _stores.RemoveAt(i);
                    return NoContent();
                }
            }
            return NotFound("Store not found.");
        }

        [HttpGet("{id}", Name = "GetStoreById")]
        public IActionResult GetStoreById(Guid id)
        {
            foreach (var store in _stores)
            {
                if (store.Id == id)
                    return Ok(store);
            }
            return NotFound("Store not found.");
        }
    }
}
