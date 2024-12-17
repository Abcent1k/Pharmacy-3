using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pharmacy_3.Controllers
{
	public class ProductsController : Controller
	{
		// Повертає тільки View без обробки даних
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Create()
		{
			return View();
		}

		public IActionResult Edit(uint id)
		{
			ViewBag.ProductId = id; // Передає ID у View для обробки через AJAX
			return View();
		}

		public IActionResult Details(uint id)
		{
			ViewBag.ProductId = id; // Передає ID у View
			return View();
		}

		public IActionResult Delete(uint id)
		{
			ViewBag.ProductId = id; // Передає ID у View
			return View();
		}
	}
}
