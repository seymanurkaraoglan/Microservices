using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Common;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        // private static readonly List<ItemDto> items = new List<ItemDto>{
        //     new ItemDto(Guid.NewGuid(), "Potion", "Restores 10 health points", 5, DateTime.UtcNow),
        //     new ItemDto(Guid.NewGuid(), "Rage", "Increases attack", 15, DateTime.UtcNow),
        //     new ItemDto(Guid.NewGuid(), "Poison", "Removes 10 health points", 25, DateTime.UtcNow)
        // };

        private readonly IRepository<Item> repository;

        private static int requestCounter = 0;

        public ItemsController(IRepository<Item> repo)
        {
            repository = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            requestCounter++;
            Console.WriteLine($"Request {requestCounter}: Starting...");

            if (requestCounter <= 2)
            {
                Console.WriteLine($"Request {requestCounter}: Delaying...");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            if (requestCounter <= 4)
            {
                Console.WriteLine($"Request {requestCounter}: (t)ERROR 500...");
                return StatusCode(500);
            }

            var items = (await repository.GetAllAsync())
                        .Select(x => x.AsDto());

            Console.WriteLine($"Request {requestCounter}: This is fine!");

            return Ok(items);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await repository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ItemDto>> CreateAsync(CreateItemDto dto)
        {
            var item = new Item
            {
                Name = dto.Name,
                Description = dto.Desc,
                Price = dto.Price,
                CreateDate = DateTime.UtcNow
            };

            await repository.CreateAsync(item);

            return Ok(item);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<ItemDto>> UpdateAsync(Guid id, UpdateItemDto dto)
        {
            var itemToUpdate = await repository.GetAsync(id);

            if (itemToUpdate == null)
            {
                return NotFound();
            }

            itemToUpdate.Name = dto.Name;
            itemToUpdate.Description = dto.Desc;
            itemToUpdate.Price = dto.Price;

            await repository.UpdateAsync(itemToUpdate);

            return NoContent();
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> RemoveAsync(Guid id)
        {
            var itemToRemove = await repository.GetAsync(id);

            if (itemToRemove == null)
            {
                return NotFound();
            }

            await repository.DeleteAsync(itemToRemove.Id);

            return NoContent();
        }
    }
}