using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy_3.Models.Products;
using Pharmacy_3.Interfaces;
using PagedList;

namespace Pharmacy_3.Controllers
{
	public class ProductsController : Controller
	{
		private readonly IProductService _productService;

		public ProductsController(IProductService productService)
		{
			_productService = productService;
		}

		// Method to display products list with pagination, sorting, and filtering
		public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, int? pageSize)
		{
			// Storing sort order and filter parameters
			ViewBag.CurrentSort = sortOrder;
			ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
			ViewBag.UPCSortParm = sortOrder == "UPC" ? "upc_desc" : "UPC";
			ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

			// If a new search is initiated, reset to the first page
			if (searchString != null)
			{
				page = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			// Store the current filter
			ViewBag.CurrentFilter = searchString;
			int currentPageSize = pageSize ?? 10;
			int pageNumber = page ?? 1;
			ViewBag.CurrentPageSize = currentPageSize;
			ViewBag.CurrentPage = pageNumber;

			// Retrieve all products
			var products = await _productService.GetAllProductsAsync();

			// Search by name or UPC (only those that start with the provided search string)
			if (!String.IsNullOrEmpty(searchString))
			{
				products = products.Where(p => p.Name.StartsWith(searchString, StringComparison.OrdinalIgnoreCase) ||
											   p.UPC.ToString().StartsWith(searchString));
			}

			// Sorting
			switch (sortOrder)
			{
				case "name_desc":
					products = products.OrderByDescending(p => p.Name);
					break;
				case "UPC":
					products = products.OrderBy(p => p.UPC);
					break;
				case "upc_desc":
					products = products.OrderByDescending(p => p.UPC);
					break;
				case "Price":
					products = products.OrderBy(p => p.Price);
					break;
				case "price_desc":
					products = products.OrderByDescending(p => p.Price);
					break;
				default:
					products = products.OrderBy(p => p.Name);
					break;
			}

			// Calculate the total number of items after filtering
			int totalItems = products.Count();
			ViewBag.TotalItems = totalItems;

			// Implement pagination: skip elements and take current page items
			var pagedProducts = products
				.Skip((pageNumber - 1) * currentPageSize)
				.Take(currentPageSize)
				.ToList();

			return View(pagedProducts);
		}

		// Method to display details of a product by its ID
		public async Task<IActionResult> Details(uint? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _productService.GetProductByIdAsync(id.Value);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// Method to display the product creation form
		public IActionResult Create()
		{
			return View();
		}

		// Method to handle POST request for creating a new product
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ProductType,UPC,Name,Price,EDRPOU,ExpirationDate,ConsumableType,DeviceType,DrugType,NeedRecipe")] ProductViewModel productViewModel)
		{
			if (ModelState.IsValid)
			{
				Product product;

				// Create the appropriate product type based on the productType value
				switch (productViewModel.ProductType)
				{
					case "Consumables":
						if (productViewModel.ExpirationDate.HasValue && productViewModel.ConsumableType.HasValue)
						{
							product = new Consumables(
								productViewModel.UPC,
								productViewModel.Name,
								productViewModel.Price,
								productViewModel.EDRPOU,
								productViewModel.ExpirationDate.Value,
								productViewModel.ConsumableType.Value
							);
						}
						else
						{
							// Adding error if required fields are missing
							ModelState.AddModelError("", "ExpirationDate and ConsumableType are required for Consumables.");
							return View(productViewModel);
						}
						break;

					case "Devices":
						if (productViewModel.DeviceType.HasValue)
						{
							product = new Devices(
								productViewModel.UPC,
								productViewModel.Name,
								productViewModel.Price,
								productViewModel.EDRPOU,
								productViewModel.DeviceType.Value
							);
						}
						else
						{
							// Adding error if required fields are missing
							ModelState.AddModelError("", "DeviceType is required for Devices.");
							return View(productViewModel);
						}
						break;

					case "Drugs":
						if (productViewModel.ExpirationDate.HasValue && productViewModel.DrugType.HasValue)
						{
							product = new Drugs(
								productViewModel.UPC,
								productViewModel.Name,
								productViewModel.Price,
								productViewModel.EDRPOU,
								productViewModel.ExpirationDate.Value,
								productViewModel.DrugType.Value,
								productViewModel.NeedRecipe
							);
						}
						else
						{
							// Adding error if required fields are missing
							ModelState.AddModelError("", "ExpirationDate, DrugType, and NeedRecipe are required for Drugs.");
							return View(productViewModel);
						}
						break;

					default:
						ModelState.AddModelError("", "Invalid product type.");
						return View(productViewModel);
				}

				try
				{
					// Attempt to add the product
					await _productService.AddProductAsync(product);
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					// Add error if something goes wrong during product addition
					ModelState.AddModelError("", $"An error occurred while adding the product: {ex.Message}");
					return View(productViewModel);
				}
			}

			return View(productViewModel);
		}

		// Method to display the edit form for a product by its ID
		public async Task<IActionResult> Edit(uint? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _productService.GetProductByIdAsync(id.Value);
			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}

		// GET Method to edit a product
		[HttpGet]
		public async Task<IActionResult> Edit(uint id)
		{
			var product = await _productService.GetProductByIdAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			// Create a view model with product information to be edited
			var viewModel = new ProductViewModel
			{
				ProductType = product is Consumables ? "Consumables" :
							  product is Devices ? "Devices" :
							  product is Drugs ? "Drugs" : "",
				UPC = product.UPC,
				Name = product.Name,
				Price = product.Price,
				EDRPOU = product.EDRPOU,
				ExpirationDate = (product as Consumables)?.ExpirationDate ?? (product as Drugs)?.ExpirationDate,
				ConsumableType = (product as Consumables)?.ConsumableType,
				DeviceType = (product as Devices)?.DeviceType,
				DrugType = (product as Drugs)?.DrugType,
				NeedRecipe = (product as Drugs)?.NeedRecipe ?? false
			};

			return View(viewModel);
		}

		// POST Method to save edited product information
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(uint id, [Bind("ProductType,UPC,Name,Price,ExpirationDate,ConsumableType,DeviceType,DrugType,NeedRecipe")] ProductViewModel productViewModel)
		{
			if (id != productViewModel.UPC)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					var product = await _productService.GetProductByIdAsync(id);
					if (product == null)
					{
						return NotFound();
					}

					// Update product data
					product.Name = productViewModel.Name;
					product.Price = productViewModel.Price;

					switch (productViewModel.ProductType)
					{
						case "Consumables":
							var consumable = product as Consumables;
							if (consumable != null)
							{
								consumable.ExpirationDate = productViewModel.ExpirationDate ?? consumable.ExpirationDate;
								consumable.ConsumableType = productViewModel.ConsumableType ?? consumable.ConsumableType;
							}
							break;

						case "Devices":
							var device = product as Devices;
							if (device != null)
							{
								device.DeviceType = productViewModel.DeviceType ?? device.DeviceType;
							}
							break;

						case "Drugs":
							var drug = product as Drugs;
							if (drug != null)
							{
								drug.ExpirationDate = productViewModel.ExpirationDate ?? drug.ExpirationDate;
								drug.DrugType = productViewModel.DrugType ?? drug.DrugType;
								drug.NeedRecipe = productViewModel.NeedRecipe;
							}
							break;

						default:
							ModelState.AddModelError("", "Invalid product type.");
							return View(productViewModel);
					}

					await _productService.UpdateProductAsync(product);
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					// Add error if something goes wrong during product update
					ModelState.AddModelError("", $"An error occurred while updating the product: {ex.Message}");
				}
			}

			return View(productViewModel);
		}

		// Method to display the delete confirmation view
		public async Task<IActionResult> Delete(uint? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _productService.GetProductByIdAsync(id.Value);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// POST Method to confirm and delete a product by its ID
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(uint id)
		{
			await _productService.DeleteProductAsync(id);
			return RedirectToAction(nameof(Index));
		}

		// Method to check if a product exists by its ID
		private async Task<bool> ProductExists(uint id)
		{
			return await _productService.ProductExistsAsync(id);
		}

		// GET Method to display details of a product
		[HttpGet]
		public async Task<IActionResult> Details(uint id)
		{
			var product = await _productService.GetProductByIdAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			// Create a view model to display product details
			var viewModel = new ProductViewModel
			{
				ProductType = product is Consumables ? "Consumables" :
							  product is Devices ? "Devices" :
							  product is Drugs ? "Drugs" : "",
				UPC = product.UPC,
				Name = product.Name,
				Price = product.Price,
				EDRPOU = product.EDRPOU,
				ExpirationDate = (product as Consumables)?.ExpirationDate ?? (product as Drugs)?.ExpirationDate,
				ConsumableType = (product as Consumables)?.ConsumableType,
				DeviceType = (product as Devices)?.DeviceType,
				DrugType = (product as Drugs)?.DrugType,
				NeedRecipe = (product as Drugs)?.NeedRecipe ?? false
			};

			return View(viewModel);
		}
	}
}
