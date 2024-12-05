using Microsoft.AspNetCore.Mvc;
using Pharmacy_3.Interfaces;
using Pharmacy_3.Models;
using System.Threading.Tasks;

namespace Pharmacy_3.Controllers
{
	public class UsersController : Controller
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		public async Task<IActionResult> Index()
		{
			var users = await _userService.GetAllUsersAsync();
			return View(users);
		}

		public async Task<IActionResult> Details(int id)
		{
			var user = await _userService.GetUserByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			return View(user);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(User user)
		{
			if (ModelState.IsValid)
			{
				await _userService.AddUserAsync(user);
				return RedirectToAction(nameof(Index));
			}
			return View(user);
		}

		public async Task<IActionResult> Edit(int id)
		{
			var user = await _userService.GetUserByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			return View(user);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, User user)
		{
			if (id != user.UserId)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				await _userService.UpdateUserAsync(user);
				return RedirectToAction(nameof(Index));
			}
			return View(user);
		}

		public async Task<IActionResult> Delete(int id)
		{
			var user = await _userService.GetUserByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			return View(user);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			await _userService.DeleteUserAsync(id);
			return RedirectToAction(nameof(Index));
		}
	}
}
