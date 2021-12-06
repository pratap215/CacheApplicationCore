using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PnP.Framework;
using PnP.Core.Auth;
using PnP.Core.Model.SharePoint;
using PnP.Core.QueryModel;
using PnP.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Pages;

namespace TranslationApplicationCore.Services
{
    public interface ITranslatePageService
    {
        string Translate();
    }
    public class TranslatePageService : ITranslatePageService
    {
        public string Translate()
        {
            var clientId = "70beb0ae-55b2-4102-9488-2ea02c4d3184";
            using (var cc = new AuthenticationManager().GetACSAppOnlyContext("https://8p5g5n.sharepoint.com/", clientId, "a5V9AGPnB8YViSS9rDRb89KUtwudQMIBmzdqq5ZfPKI="))
            {
                cc.Load(cc.Web, p => p.Title);
                cc.ExecuteQuery();
                Console.WriteLine(cc.Web.Title);
                ClientSidePage page = ClientSidePage.Load(cc, "Home.aspx");
                var clientSideControls = page.Controls.Where(c => c.Type.Name == "ClientSideWebPart").ToList();
                var webParts = clientSideControls.ConvertAll(w => w as ClientSideWebPart);
            };
            return null;
        }


       


    }

    public class WebInfo
    {
        public string WebId { get; set; }
        public string SiteId { get; set; }
        public string WebUrl { get; set; }
    }


}
