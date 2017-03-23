using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadArquivos.Model
{
    public class Municipio
    {
        public string Nome { get; set; }
        public IFormFile Arquivo{ get; set; }

    }
}
