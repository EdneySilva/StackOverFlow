using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace InjetandoInteracaoComUsuario.Controllers
{

   

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string parametro)
        {
         
            Process process = new Process();
            var question = process.Run(async (questionBox) =>
            {
                SaleService service = new SaleService();
                var result = await service.GetValue(questionBox);
                process.End(result);
            });

            return View(question);
        }

        [HttpPost]
        public async Task<JsonResult> Answer(bool result, string id)
        {
            var response = await Process.DefineAnswer(id, result);
            
            return Json(response);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
