using System.Security.Permissions;
using System.Threading;

namespace MonitorBuildServerFiles
{
    public class Program
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        static void Main(string[] args)
        {
            var monitorArquivos = (args.Length == 2) ? new MonitorArquivos(args[0], args[1]) : new MonitorArquivos();
            monitorArquivos.MonitorarDiretorios();

            new AutoResetEvent(false).WaitOne();
        }
    }
}
