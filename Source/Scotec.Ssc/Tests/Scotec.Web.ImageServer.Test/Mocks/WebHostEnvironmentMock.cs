using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Scotec.Web.ImageServer.Test.Mocks
{
    [SuppressMessage("ReSharper", "NotNullOrRequiredMemberIsNotInitialized")]
    public class WebHostEnvironmentMock : IWebHostEnvironment
    {
        public WebHostEnvironmentMock()
        {
            WebRootPath = AppContext.BaseDirectory;
        }
        public string ApplicationName { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
    }
}
