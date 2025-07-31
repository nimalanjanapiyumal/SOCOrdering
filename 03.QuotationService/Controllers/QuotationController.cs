using _01.Contracts.Models;
using _03.QuotationService.Entities;
using _03.QuotationService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _03.QuotationService.Controllers
{
    [ApiController]
    [Route("api/quotations")]
    public class QuotationController : ControllerBase
    {
        private readonly IQuotationRepository _repo;

        public QuotationController(IQuotationRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestQuotes([FromBody] QuoteRequestDto request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
                return BadRequest("Invalid request payload.");

            // Simulate three distributors providing quotes for the actual OrderId
            var distributors = new[] { "TechWorld", "ElectroCom", "GadgetCentral" };
            foreach (var dist in distributors)
            {
                var quote = new Quotation
                {
                    OrderId = request.OrderId,
                    Distributor = dist,
                    EstimatedDays = dist == "TechWorld" ? 3 : dist == "ElectroCom" ? 4 : 5,
                    Items = request.Items.Select(i => new QuotationItem
                    {
                        ProductId = i.ProductId,
                        UnitPrice = 100 + i.ProductId + (dist == "ElectroCom" ? -5 : 0), // dummy variation
                        Available = 10
                    }).ToList()
                };
                await _repo.AddAsync(quote);
            }

            return Accepted();
        }

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var quotes = await _repo.GetByOrderAsync(orderId);
            return Ok(quotes.Select(q => new QuotationResultDto
            {
                OrderId = q.OrderId,
                Distributor = q.Distributor,
                EstimatedDays = q.EstimatedDays,
                Items = q.Items.Select(i => new QuotationItemResultDto
                {
                    ProductId = i.ProductId,
                    UnitPrice = i.UnitPrice,
                    Available = i.Available
                })
            }));
        }
    }
}