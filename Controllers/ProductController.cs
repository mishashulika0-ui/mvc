using Microsoft.AspNetCore.Mvc;
using p48_lessons_asp.Models;
using System.Collections.Generic;

namespace p48_lessons_asp.Controllers
{
    public class ProductController : Controller
    {
        // DbContext
        private static List<ProductListItemModel> products = new()
        {
            new() {Name="Macbook Pro M4", Price = 90000, CategoryName="Most expensive gadgets(Apple)"},
            new() {Name="Samsung Washmachine", Price = 30000, CategoryName = "Appliances"}
        };

        private static Dictionary<Guid, string> _categories = new (){
            {Guid.Parse("ef2e3af3-ee40-4351-be38-3231b1505a96"), "Most expensive gadgets(Apple)"},
            {Guid.Parse("11ba17ad-3ef4-49b2-8a24-194ac2db596f"), "Appliances"},
            {Guid.Parse("afe51c95-3926-40c8-8fb3-3374c447ade6"),"Clothes"}
        };

        [HttpGet]
        public IActionResult Index()
        {
            return View(products);
        }

        [HttpGet]
        [Route("products")]
        public IActionResult Products([FromQuery] int? startFrom, [FromQuery] int? endsWith)
        {
            if (startFrom.HasValue)
            {
                if (startFrom.Value < 0 || startFrom.Value > products.Count - 1)
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = $"Start from must be in 0-{products.Count - 1}"
                    });
                }
            }

            if (endsWith.HasValue)
            {
                if (endsWith.Value < 0 || endsWith.Value > products.Count - 1)
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = $"endsWith must be in 0-{products.Count - 1}"
                    });
                }
            }

            if (startFrom.HasValue && endsWith.HasValue && startFrom.Value > endsWith.Value)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "startFrom cannot be greater than endsWith"
                });
            }

            if (!startFrom.HasValue && !endsWith.HasValue)
            {
                return Ok(new
                {
                    Status = 200,
                    Message = "Ok",
                    Products = products
                });
            }

            int start = 0;
            int end = products.Count - 1;

            if (startFrom.HasValue)
            {
                start = startFrom.Value;
            }

            if (endsWith.HasValue)
            {
                end = endsWith.Value;
            }

            var result = new List<ProductListItemModel>();
            for (int i = start; i <= end; i++)
            {
                result.Add(products[i]);
            }

            return Ok(new
            {
                Status = 200,
                Message = "Ok",
                Products = result
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new CreateProductViewModel
            {
                Categories = new(_categories, "Key", "Value")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateProductViewModel model)
        {
            /*
            CSRF-атака(Cross-Site-Request-Forgery) - суть атаки наступна:
            Користувач залогінився на вашому сайті, для нього сформувався токен, який зберігається у кукі. Є ризик що цим кукі може скористуватися інший ресурс, який може бути шкідливими, цей ресурс, зможе спробувати використати ваш кукі, щоб відправити запит на сайт в якому у вас є авторизація.
            */
            if (!ModelState.IsValid)
            {
                model.Categories = new(_categories, "Key", "Value", model.CategoryId);
                return View(model);
            }

            products.Add(new ProductListItemModel
            {
                Name = model.Name,
                Price = model.Price,
                CategoryName = _categories[model.CategoryId]
            });
            return RedirectToAction(nameof(Index));
        }
    }
}