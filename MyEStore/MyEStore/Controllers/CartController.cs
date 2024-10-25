using Microsoft.AspNetCore.Mvc;

namespace MyEStore.Controllers
{
	public class CartController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult AddToCart(int id)
		{
			return View();
		}
	}
}
