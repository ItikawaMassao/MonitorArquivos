using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;

namespace MonitorBuildServerFiles
{
    public class MonitorArquivos
    {
        public string DiretorioOrigem { get; private set; }
        public string DiretorioDestino { get; private set; }
        public string FiltroArquivos { get; private set; }

        public MonitorArquivos(string diretorioOrigem, string diretorioDestino, string filtroArquivos)
        {
            DiretorioOrigem = diretorioOrigem;
            DiretorioDestino = diretorioDestino;
            FiltroArquivos = filtroArquivos;
            if (!Directory.Exists(DiretorioOrigem))
                throw new DirectoryNotFoundException($"Diretorio de origem nao encontrado: {DiretorioOrigem}");

            if (!Directory.Exists(DiretorioDestino))
                throw new DirectoryNotFoundException($"Diretorio de destino nao encontrado: {DiretorioDestino}");
        }

        public MonitorArquivos() : this(AppSettings.DiretorioOrigem, AppSettings.DiretorioDestino, AppSettings.FiltroArquivos)
        {
        }

        public MonitorArquivos(string diretorioOrigem, string diretorioDestino) : this(diretorioOrigem, diretorioDestino, AppSettings.FiltroArquivos)
        {
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void MonitorarDiretorios()
        {
            var informacao = $"Monitorando o diretorio '{DiretorioOrigem}' (Filtro: {FiltroArquivos})...";

            Console.Title = informacao;
            Console.WriteLine(informacao);

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = DiretorioOrigem;
            //watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
            watcher.Filter = FiltroArquivos;
            watcher.IncludeSubdirectories = true;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                TaskKill();
                var nomeArquivo = e.Name;
                Console.WriteLine($"Copiando '{nomeArquivo}' [{DateTime.Now.ToLongTimeString()}]...");
                if (!File.Exists(e.FullPath))
                    return;

                if (string.IsNullOrWhiteSpace(DiretorioDestino) || !Directory.Exists(DiretorioDestino))
                    return;

                File.Copy(e.FullPath, Path.Combine(DiretorioDestino, nomeArquivo), true);
                Console.WriteLine($"'{e.Name}' => '{DiretorioDestino}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao copiar arquivos:{Environment.NewLine}'{ex.Message}'");
            }
        }

        private void TaskKill()
        {
            try
            {
                var contador = 0;
                IEnumerable<Process> processos;
                do
                {
                    contador++;
                    processos = GetProcessProviders();
                    foreach (Process processo in processos)
                    {
                        Console.WriteLine($"Derrubando provider(s) [{DateTime.Now.ToLongTimeString()}]...");
                        processo.Kill();
                        Thread.Sleep(500);
                    }
                } while (processos.Any() && contador < 10);
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao finalizar processos:{Environment.NewLine}'{e.Message}'");
            }
        }

        private IEnumerable<Process> GetProcessProviders()
        {
            return (from p in Process.GetProcesses()
                         where p.ProcessName == "BPrv230" ||
                             p.ProcessName == "BPrv230.exe" ||
                             p.ProcessName == "CS1" ||
                             p.ProcessName == "CS1.exe"
                    select p);
        }
    }
}
