using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadArquivos.Model
{
    public class RelatorioMunicipio
    {
        public RelatorioMunicipio(string caminhoBaseArquivo)
        {
            CaminhoBase = caminhoBaseArquivo;
        }
        public string CaminhoBase { get; set; }
        public Municipio Municipio { get; set; }

        public string AbrirArquivo()
        {
            return System.IO.File.ReadAllText(System.IO.Path.Combine(CaminhoBase, Municipio.Nome + ".txt"));
        }
    }
}
