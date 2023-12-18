using System.Web.Mvc;
using SerilogPoc.Contracts;

namespace SerilogPoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogService _logService;

        public HomeController(ILogService logService)
        {
            _logService = logService;
        }

        public ActionResult Index()
        {
            _logService.LogInformation("User is attempting to access the {RouteEndPoint} page", "Index");
            return View();
        }

        public ActionResult About()
        {
            _logService.LogInformation("User is attempting to access the {RouteEndPoint} page", "About");
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            _logService.LogInformation("User is attempting to access the {RouteEndPoint} page", "Contact");
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}