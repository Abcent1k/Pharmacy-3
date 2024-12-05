using Microsoft.AspNetCore.Mvc;
using Pharmacy_3.Interfaces;
using Pharmacy_3.Models;
using System.Threading.Tasks;

namespace Pharmacy_3.Controllers
{
	public class OrdersController : Controller
	{
		private readonly IOrderService _orderService;

		public OrdersController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		public async Task<IActionResult> Index()
		{
			var orders = await _orderService.GetAllOrdersAsync();
			return View(orders);
		}

		public async Task<IActionResult> Details(int id)
		{
			var order = await _orderService.GetOrderByIdAsync(id);
			if (order == null)
			{
				return NotFound();
			}
			return View(order);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Order order)
		{
			if (ModelState.IsValid)
			{
				await _orderService.AddOrderAsync(order);
				return RedirectToAction(nameof(Index));
			}
			return View(order);
		}

		public async Task<IActionResult> Edit(int id)
		{
			var order = await _orderService.GetOrderByIdAsync(id);
			if (order == null)
			{
				return NotFound();
			}
			return View(order);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, Order order)
		{
			if (id != order.Id)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				await _orderService.UpdateOrderAsync(order);
				return RedirectToAction(nameof(Index));
			}
			return View(order);
		}

		public async Task<IActionResult> Delete(int id)
		{
			var order = await _orderService.GetOrderByIdAsync(id);
			if (order == null)
			{
				return NotFound();
			}
			return View(order);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			await _orderService.DeleteOrderAsync(id);
			return RedirectToAction(nameof(Index));
		}
	}
}
