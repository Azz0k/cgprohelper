using CGPGK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CGPGK.Utils.Utils;



namespace CGPGK.Services
{
    internal class WorkerService
    {

        private readonly AppSettings _appSettings;
        IServiceProvider _serviceProvider;
        public WorkerService(AppSettings appSettings, IServiceProvider provider)
        {
            _appSettings = appSettings;
            _serviceProvider = provider;
        }
        public async Task Work()
        {
            while (true)
            {
                string? line = await Console.In.ReadLineAsync();
                if (line == null)
                {
                    break;
                }
                _ = Task.Run(() =>
                {
                    //ProcessMessage(line);
                });
            }
        }


    }
}
