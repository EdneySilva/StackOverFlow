using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UploadArquivos.Model;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace UploadArquivos.Controllers
{
    public class HomeController : Controller
    {
        IHostingEnvironment Enviroment { get; set; }

        public HomeController(IHostingEnvironment enviroment)
        {
            Enviroment = enviroment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GerarRelatorio(Municipio municipio)
        {
            if (municipio.Arquivo != null)
            {
                // salvando arquivo em disco
                var reader = new BinaryReader(municipio.Arquivo.OpenReadStream());
                System.IO.File.WriteAllBytes(System.IO.Path.Combine(Enviroment.WebRootPath, $"{municipio.Nome}.txt"), reader.ReadBytes((int)municipio.Arquivo.Length));
            }
            return RedirectToAction("Exibir", "Relatorio", new { nome = municipio.Nome });
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
