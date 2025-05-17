using Microsoft.AspNetCore.Mvc;

namespace DirectoryChangesTracker.Controllers
{
	public class ScanDirectoryController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
