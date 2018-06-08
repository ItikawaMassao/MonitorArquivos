using System.Collections.Generic;
using System.Configuration;

namespace MonitorBuildServerFiles
{
    public static class AppSettings
    {
        public static string DiretorioOrigem => ConfigurationManager.AppSettings["diretorioOrigem"];
        public static string DiretorioDestino => ConfigurationManager.AppSettings["diretorioDestino"];
        public static string FiltroArquivos => ConfigurationManager.AppSettings["filtroArquivos"];
    }
}
