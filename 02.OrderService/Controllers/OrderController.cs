using _01.Contracts.Messaging;
using _01.Contracts.Models;
using _01.Contracts.Repositories;
using _02.OrderService.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace _02.OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _repo;
        private readonly IMessageBus _bus;
        private readonly ILogger<OrderController> _logger;
        private readonly IQuotationClient _quotationClient;
        private readonly IComparisonClient _comparisonClient;

        public OrderController(
            IOrderRepository repo,
            IMessageBus bus,
            ILogger<OrderController> logger,
            IQuotationClient quotationClient,
            IComparisonClient comparisonClient)
        {
            _repo = repo;
            _bus = bus;
            _logger = logger;
            _quotationClient = quotationClient;
            _comparisonClient = comparisonClient;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            if (dto == null || dto.Items == null || !dto.Items.Any())
                return BadRequest("Order must contain at least one item.");

            var orderId = await _repo.CreateOrderAsync(dto.CustomerId, dto.Items);

            await _bus.PublishAsync("QuotationRequested", new
            {
                OrderId = orderId,
                Items = dto.Items.Select(i => new { i.ProductId, i.Quantity })
            });

            var quoteRequest = new QuoteRequestDto
            {
                OrderId = orderId,
                Items = dto.Items
            };
            await _quotationClient.RequestQuotesAsync(quoteRequest);

            // Trigger comparison to validate quotes and move order forward
            await _comparisonClient.CompareAsync(orderId);

            _logger.LogInformation("Created order {OrderId} and initiated quotation comparison.", orderId);

            return CreatedAtAction(nameof(Get), new { id = orderId }, new { orderId });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = await _repo.GetOrderAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}