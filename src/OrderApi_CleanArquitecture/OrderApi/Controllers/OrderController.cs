using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.Dtos;
using OrderApi.Application.Interfaces;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderRequest request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            return order == null ? NotFound() : Ok(order);
        }
    }

}
