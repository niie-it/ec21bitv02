using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEStore.Models;

namespace MyEStore.Controllers
{
	public class PaymentController : Controller
	{
		private readonly PaypalClient _paypalClient;

		public PaymentController(PaypalClient paypalClient)
		{
			_paypalClient = paypalClient;
		}

		public IActionResult Index()
		{
			return View();
		}

		const string CART_KEY = "MY_CART";
		public List<CartItem> CartItems
		{
			get
			{
				var carts = HttpContext.Session.Get<List<CartItem>>(CART_KEY);
				if (carts == null)
				{
					carts = new List<CartItem>();
				}
				return carts;
			}
		}

		[HttpPost]
		public async Task<IActionResult> PaypalOrder(CancellationToken cancellationToken)
		{
			// Tạo đơn hàng (thông tin lấy từ Session???)
			var tongTien = CartItems.Sum(p => p.ThanhTien).ToString();
			var donViTienTe = "USD";
			// OrderId - mã tham chiếu (duy nhất)
			var orderIdref = "DH" + DateTime.Now.Ticks.ToString();

			try
			{
				// a. Create paypal order
				var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, orderIdref);

				return Ok(response);
			}
			catch (Exception e)
			{
				var error = new
				{
					e.GetBaseException().Message
				};

				return BadRequest(error);
			}
		}

		public async Task<IActionResult> PaypalCapture(string orderId, CancellationToken cancellationToken)
		{
			try
			{
				var response = await _paypalClient.CaptureOrder(orderId);

				var reference = response.purchase_units[0].reference_id;

				// Put your logic to save the transaction here
				// You can use the "reference" variable as a transaction key
				// Lưu đơn hàng vô database

				return Ok(response);
			}
			catch (Exception e)
			{
				var error = new
				{
					e.GetBaseException().Message
				};

				return BadRequest(error);
			}
		}

		public IActionResult Success()
		{
			return View();
		}
	}
}
