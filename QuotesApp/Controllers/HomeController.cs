using Microsoft.AspNetCore.Mvc;
using QuotesApp.Models;
using System.Diagnostics;

namespace QuotesApp.Controllers
{
    public class HomeController : Controller
    {

        private QuoteContext _quoteContext;

        public HomeController(QuoteContext quoteContext) => _quoteContext = quoteContext;

        public ViewResult Index(string id)
        {
            // load current filters and data needed for filter drop down in ViewBag
            // TODO: add filter var

            ViewBag.Tag = _quoteContext.Tags.ToList();

            // TODO: get all quotes from database based on current filter



            var quotes = _quoteContext.Quotes.ToList();
            return View(quotes);
        }

        [HttpGet]
        public ViewResult Add()
        {
            var quote = new Quote { };
            return View(quote);
        }

        [HttpPost]
        public IActionResult Add(Quote quote)
        {
            if (ModelState.IsValid)
            {
                _quoteContext.Quotes.Add(quote);
                _quoteContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(quote);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}