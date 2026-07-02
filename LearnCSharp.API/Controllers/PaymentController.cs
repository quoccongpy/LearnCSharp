using LearnCSharp.Application.Interfaces;
using LearnCSharp.Infrastructure.Payments.VNPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnCSharp.API.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IVnPayService _vnPayService;

        public PaymentController(IPaymentService paymentService, IVnPayService vnPayService)
        {
            _paymentService = paymentService;
            _vnPayService = vnPayService;
        }

        [HttpPost("stripe/create-payment-intent/{orderId}")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentIntent(int orderId)
        {
            var result = await _paymentService.CreatePaymentIntentAsync(orderId);
            return Ok(result);
        }

        [HttpPost("stripe/webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];
            await _paymentService.HandleWebhookAsync(json, stripeSignature);
            return Ok();
        }

        [HttpPost("vnpay/create-vnpay-url/{orderId}")]
        [Authorize]
        public async Task<IActionResult> CreateVnPayUrl(int orderId)
        {
            var ip = KeySecurity.GetIpAddress(HttpContext);
            var paymentUrl = await _vnPayService.CreatePaymentUrl(orderId, ip);
            return Ok(new { paymentUrl });
        }

        [HttpGet("vnpay/return")]
        public  IActionResult Return()
        {
            var query = Request.Query.ToDictionary(x => x.Key,x => x.Value.ToString());
            var result = _vnPayService.PaymentExecute(query);
            return Ok(result);
        }
        [HttpGet("vnpay/ipn")]
        public async Task<IActionResult> Ipn()
        {
            var query = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
            var result = await _vnPayService.ProcessIpnAsync(query);
            return Ok(result);
        }
    }
}