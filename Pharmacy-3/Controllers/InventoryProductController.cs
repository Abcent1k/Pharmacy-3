using Microsoft.AspNetCore.Mvc;
using Pharmacy_3.Interfaces;
using Pharmacy_3.Models;
using System.Threading.Tasks;

namespace Pharmacy_3.Controllers
{
	public class InventoryProductsController : Controller
	{
		private readonly IInventoryProductService _inventoryProductService;

		public InventoryProductsController(IInventoryProductService inventoryProductService)
		{
			_inventoryProductService = inventoryProductService;
		}

		public async Task<IActionResult> Index()
		{
			var inventoryProducts = await _inventoryProductService.GetAllInventoryProductsAsync();
			return View(inventoryProducts);
		}

		public async Task<IActionResult> Details(int id)
		{
			var inventoryProduct = await _inventoryProductService.GetInventoryProductByIdAsync(id);
			if (inventoryProduct == null)
			{
				return NotFound();
			}
			return View(inventoryProduct);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(InventoryProduct inventoryProduct)
		{
			if (ModelState.IsValid)
			{
				await _inventoryProductService.AddInventoryProductAsync(inventoryProduct);
				return RedirectToAction(nameof(Index));
			}
			return View(inventoryProduct);
		}

		public async Task<IActionResult> Edit(int id)
		{
			var inventoryProduct = await _inventoryProductService.GetInventoryProductByIdAsync(id);
			if (inventoryProduct == null)
			{
				return NotFound();
			}
			return View(inventoryProduct);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, InventoryProduct inventoryProduct)
		{
			if (id != inventoryProduct.OrderId)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				await _inventoryProductService.UpdateInventoryProductAsync(inventoryProduct);
				return RedirectToAction(nameof(Index));
			}
			return View(inventoryProduct);
		}

		public async Task<IActionResult> Delete(int id)
		{
			var inventoryProduct = await _inventoryProductService.GetInventoryProductByIdAsync(id);
			if (inventoryProduct == null)
			{
				return NotFound();
			}
			return View(inventoryProduct);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			await _inventoryProductService.DeleteInventoryProductAsync(id);
			return RedirectToAction(nameof(Index));
		}
	}
}
