using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Techfin.Transaction.Application.DTOs;
using Techfin.Transaction.Application.Messaging;

namespace Techfin.Transaction.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly CreateTransactionHandler _handler;

        public TransactionController(CreateTransactionHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionRequest request)
        {
            var command = new CreateTransactionCommand(request.CustomerId, request.Amount);

            var result = await _handler.HandleAsync(command);

            if (!result.Success)
                return BadRequest(result);

            return Created(string.Empty, result.Data);
        }
    }
}
