using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Core.Interfaces;
using System.Net.Mime;

namespace Payment.API.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
  
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Returns Transaction History in Pages.
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("transaction-history/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTransactionHistoryAsync([FromRoute] string walletId, int pageNumber)
        {
            var transactionHistory = await _transactionService.GetTransactionHistoryAsync(walletId, pageNumber);
            return StatusCode(transactionHistory.StatusCode, transactionHistory);
        }
    }
}
