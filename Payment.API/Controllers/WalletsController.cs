using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Payment.Core.DTOs.BeneficiaryDto;
using Payment.Core.DTOs.PaystackDtos.FundAccountDto;
using Payment.Core.DTOs.TransactionsDto;
using Payment.Core.DTOs.WalletDtos;
using Payment.Core.Interfaces;
using System.Net.Mime;

namespace Payment.API.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;

        public WalletsController(IWalletService walletService, ITransactionService transactionService)
        {
            _walletService = walletService;
            _transactionService = transactionService;
        }

        /// <summary>
        /// Retrieved wallet by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns statusCode 200 if found and statusCode 417 if not found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _walletService.GetWalletByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Retrieved wallet by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns statusCode 200 if found and statusCode 417 if not found</returns>
        [HttpGet("user-wallet/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        public async Task<IActionResult> GetUserWallet(string userId)
        {
            var result = await _walletService.GetWalletByUserIdAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Creates a wallet for the user with given UserId
        /// </summary>
        /// <param name="walletRequestDto">Data transfer object containing the request parameters</param>
        /// <returns>A data transfer object containing the result of the request</returns>
        [HttpPost("create-wallet")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWallet(CreateWalletRequest walletRequestDto)
        {
            var result = await _walletService.CreateWalletAsync(walletRequestDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// initailize payment process
        /// </summary>
        /// <param name="request"></param>
        /// <param name="walletId"></param>
        /// <returns>Return PaymentInitalizationResponseModel </returns>

        [HttpPost("initialize-fund-wallet/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InitializationFundWallet([FromBody] FundWalletRequest request, [FromRoute] string walletId)
        {
            var result = await _walletService.InitializeFundWallet(request, walletId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        ///  verify payment
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="walletId"></param>
        /// <returns>Payment completed</returns>

        [HttpPost("fund-wallet-verification/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FundWallets([FromBody] VerifyTransactionRequestDto reference, [FromRoute] string walletId)
        {
            var result = await _walletService.FundWalletVerificationAsync(reference, walletId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Returns a Dto on a successful transfer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="walletId"></param>
        /// <returns>200 statusCode and a Dto on successful transfer
        /// failed if failed </returns>

        [HttpPost("bank-transfer/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestDto request,[FromRoute] string walletId)
        {
            var result = await _transactionService.Transfer(request, walletId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Return Payment Successful from wallet if payment was successful and failed if not successful
        /// </summary>
        /// /// <param name="walletId">Source wallet ID</param>
        /// <param name="request">Object containing request body</param>
        /// <returns>200 statusCode and Payment Successful if successful and 400 status if failed</returns>
        [HttpPost("local-transfer/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LocalTransfer([FromRoute] string walletId, [FromBody] LocalTransferRequestDto request)
        {
            var result = await _walletService.LocalTransferAsync(walletId, request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
