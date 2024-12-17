using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pharmacy_3.Interfaces;
using Pharmacy_3.Models.Products;

namespace Pharmacy_3.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin")]
	public class ProductsApiController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductsApiController(IProductService productService)
		{
			_productService = productService;
		}

		// GET: api/products?searchString=...&sortOrder=...&page=...&pageSize=...
		[HttpGet]
		public async Task<IActionResult> GetProducts(
	[FromQuery] string searchString = null,
	[FromQuery] string sortOrder = null,
	[FromQuery] int page = 1,
	[FromQuery] int pageSize = 10)
		{
			var products = await _productService.GetAllProductsAsync();

			// Фільтрація
			if (!string.IsNullOrEmpty(searchString))
			{
				products = products.Where(p => p.Name.StartsWith(searchString, StringComparison.OrdinalIgnoreCase) ||
											   p.UPC.ToString().StartsWith(searchString));
			}

			// Сортування
			products = sortOrder switch
			{
				"name_desc" => products.OrderByDescending(p => p.Name),
				"UPC" => products.OrderBy(p => p.UPC),
				"upc_desc" => products.OrderByDescending(p => p.UPC),
				"Price" => products.OrderBy(p => p.Price),
				"price_desc" => products.OrderByDescending(p => p.Price),
				_ => products.OrderBy(p => p.Name)
			};

			// Пагінація
			var totalItems = products.Count();
			var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize);

			// Проекція на DTO із визначенням ProductType
			var result = pagedProducts.Select(p => new
			{
				p.UPC,
				p.Name,
				p.Price,
				p.EDRPOU,
				ProductType = p switch
				{
					Consumables => "Consumables",
					Devices => "Devices",
					Drugs => "Drugs",
					_ => "Unknown"
				},
				ExpirationDate = (p is Consumables consumables) ? (DateTime?)consumables.ExpirationDate :
				 (p is Drugs drugs) ? (DateTime?)drugs.ExpirationDate : null,

				ConsumableType = (p is Consumables consumablesType) ? (ConsumableType?)consumablesType.ConsumableType : null,

				DeviceType = (p is Devices devicesType) ? (DeviceType?)devicesType.DeviceType : null,

				DrugType = (p is Drugs drugsType) ? (DrugType?)drugsType.DrugType : null,

				NeedRecipe = (p is Drugs drugsNeedRecipe) ? (bool?)drugsNeedRecipe.NeedRecipe : null

			});

			return Ok(new
			{
				TotalItems = totalItems,
				PageSize = pageSize,
				CurrentPage = page,
				Products = result
			});
		}



		// GET: api/products/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetProductById(uint id)
		{
			var product = await _productService.GetProductByIdAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			// Повертаємо чистий об'єкт без проксі
			var productDto = new
			{
				product.UPC,
				product.Name,
				product.Price,
				product.EDRPOU,
				ProductType = product switch
				{
					Consumables => "Consumables",
					Devices => "Devices",
					Drugs => "Drugs",
					_ => "Unknown"
				},
				ExpirationDate = product is Consumables cons ? (DateTime?)cons.ExpirationDate :
						 product is Drugs drug ? (DateTime?)drug.ExpirationDate : null,

				ConsumableType = product is Consumables consType ? consType.ConsumableType.ToString() : null,

				DeviceType = product is Devices devType ? devType.DeviceType.ToString() : null,

				DrugType = product is Drugs drugType ? drugType.DrugType.ToString() : null,

				NeedRecipe = product is Drugs drugNeedRecipe ? (bool?)drugNeedRecipe.NeedRecipe : null
			};

			return Ok(productDto);
		}


		// PUT: api/products/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProduct(uint id, [FromBody] ProductViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var product = await _productService.GetProductByIdAsync(id);
			if (product == null) return NotFound();

			product.Name = model.Name;
			product.Price = model.Price;

			switch (model.ProductType)
			{
				case "Consumables":
					if (product is Consumables consumable)
					{
						consumable.ExpirationDate = model.ExpirationDate ?? consumable.ExpirationDate;
						consumable.ConsumableType = model.ConsumableType ?? consumable.ConsumableType;
					}
					break;

				case "Devices":
					if (product is Devices device)
					{
						device.DeviceType = model.DeviceType ?? device.DeviceType;
					}
					break;

				case "Drugs":
					if (product is Drugs drug)
					{
						drug.ExpirationDate = model.ExpirationDate ?? drug.ExpirationDate;
						drug.DrugType = model.DrugType ?? drug.DrugType;
						drug.NeedRecipe = model.NeedRecipe; // Обробляємо NeedRecipe лише для Drugs
					}
					break;

				default:
					return BadRequest("Invalid product type.");
			}

			await _productService.UpdateProductAsync(product);
			return NoContent();
		}


		// DELETE: api/products/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(uint id)
		{
			var product = await _productService.GetProductByIdAsync(id);
			if (product == null) return NotFound();

			await _productService.DeleteProductAsync(id);
			return NoContent(); // Повертає статус 204, що означає успішне видалення
		}


		[HttpPost]
		public async Task<IActionResult> CreateProduct([FromBody] ProductViewModel model)
		{
			if (!ModelState.IsValid)
			{
				// Повертаємо всі помилки з ModelState
				var errors = ModelState
					.Where(e => e.Value.Errors.Count > 0)
					.ToDictionary(
						e => e.Key,
						e => e.Value.Errors.Select(err => err.ErrorMessage).ToArray()
					);

				return BadRequest(new
				{
					error = "Invalid input data.",
					details = errors
				});
			}

			Product product = null;

			try
			{
				switch (model.ProductType)
				{
					case "Consumables":
						if (model.ExpirationDate.HasValue && model.ConsumableType.HasValue)
						{
							product = new Consumables(
								model.UPC,
								model.Name,
								model.Price,
								model.EDRPOU,
								model.ExpirationDate.Value,
								model.ConsumableType.Value
							);
						}
						else
						{
							return BadRequest(new { error = "ExpirationDate and ConsumableType are required for Consumables." });
						}
						break;

					case "Devices":
						if (model.DeviceType.HasValue)
						{
							product = new Devices(
								model.UPC,
								model.Name,
								model.Price,
								model.EDRPOU,
								model.DeviceType.Value
							);
						}
						else
						{
							return BadRequest(new { error = "DeviceType is required for Devices." });
						}
						break;

					case "Drugs":
						if (model.ExpirationDate.HasValue && model.DrugType.HasValue)
						{
							product = new Drugs(
								model.UPC,
								model.Name,
								model.Price,
								model.EDRPOU,
								model.ExpirationDate.Value,
								model.DrugType.Value,
								model.NeedRecipe
							);
						}
						else
						{
							return BadRequest(new { error = "ExpirationDate, DrugType, and NeedRecipe are required for Drugs." });
						}
						break;

					default:
						return BadRequest(new { error = "Invalid product type." });
				}

				// Додаємо продукт до бази даних
				await _productService.AddProductAsync(product);
				return Ok(new { message = "Product created successfully!", product });
			}
			catch (Exception ex)
			{
				// Логування помилки
				Console.WriteLine($"Error: {ex.Message}");
				return StatusCode(500, new
				{
					error = "An error occurred while creating the product.",
					details = ex.Message
				});
			}
		}


	}
}
