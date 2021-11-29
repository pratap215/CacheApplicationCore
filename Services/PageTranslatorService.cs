using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PnP.Core.Model.SharePoint;
using PnP.Core.Services;
using System.Threading.Tasks;
using PnP.Framework;
using PnP.Core.Auth;
using System;
using PnP.Core.Auth.Services.Builder.Configuration;
using Microsoft.SharePoint.Client;
using PnP.Framework.Modernization;

namespace TranslationApplicationCore.Services
{
    public interface IPageTranslatorService
    {
        Task<string> TranslatePage();

        // void TranslatePage1();
    }

    public class PageTranslatorService : IPageTranslatorService
    {

        public async Task<string> TranslatePage()
        {

            try
            {
                var host = Host.CreateDefaultBuilder()

             .ConfigureServices((hostingContext, services) =>
             {
                 //    Add the PnP Core SDK library services
                 //services.AddPnPCore(options =>
                 //{

                 //    options.DefaultAuthenticationProvider = new InteractiveAuthenticationProvider(
                 //            "f8023692-08de-48ea-8bef-d987f10e08d2", // Client Id
                 //            "contoso.onmicrosoft.com", // Tenant Id
                 //            new Uri("http://localhost"));  // Redirect Id
                 //});

                 services.AddPnPCore();
                 services.AddPnPCoreAuthentication(options =>
                          options.Credentials.Configurations.Add("usernamepassword",
              new PnPCoreAuthenticationCredentialConfigurationOptions
              {
                  ClientId = "c545f9ce-1c11-440b-812b-0b35217d9e83",
                  UsernamePassword = new PnPCoreAuthenticationUsernamePasswordOptions
                  {
                      Username = "LP@8p5g5n.onmicrosoft.com",
                      Password = "balaji@7hills"
                  }
              }));

             }).Build();

                // Start console host
                await host.StartAsync();

                // Connect to SharePoint
                using (var scope = host.Services.CreateScope())
                {
                    // Obtain a PnP Context factory
                    var pnpContextFactory = scope.ServiceProvider.GetRequiredService<IPnPContextFactory>();
                    // Use the PnP Context factory to get a PnPContext for the given configuration
                    using (var context = await pnpContextFactory.CreateAsync(new Uri("https://8p5g5n.sharepoint.com")))
                    {
                        var web = await context.Web.GetAsync();
                        Console.WriteLine($"Title: {web.Title}");

                        // Create the page
                        var page = await context.Web.NewPageAsync();

                        // Configure the page header
                        // Check out for more detail https://pnp.github.io/pnpcore/using-the-sdk/pages-header.html
                        page.SetDefaultPageHeader();
                        page.PageHeader.LayoutType = PageHeaderLayoutType.CutInShape;
                        page.PageHeader.ShowTopicHeader = true;
                        page.PageHeader.TopicHeader = "Welcome";
                        page.PageHeader.TextAlignment = PageHeaderTitleAlignment.Center;

                        // adding sections to the page
                        page.AddSection(CanvasSectionTemplate.OneColumn, 1);

                        // Adding text control to the first section, first column
                        // Check out for more detail https://pnp.github.io/pnpcore/using-the-sdk/pages-webparts.html#working-with-text-parts
                        page.AddControl(page.NewTextPart("<p style=\"text-align:center\">" +
                                                            "<span class=\"fontSizeSuper\">" +
                                                                "<span class=\"fontColorRed\">" +
                                                                    "<strong>PnP Core SDK Rocks!</strong>" +
                                                                "</span>" +
                                                            "</span>" +
                                                         "</p>"), page.Sections[0].Columns[0]);

                        // Save the page
                        await page.SaveAsync("Awesomeness.aspx");

                        // Publish the page
                        await page.PublishAsync();
                    }
                }

                // Cleanup console host
                host.Dispose();

                return "done";
            }
            catch (Exception ex)
            {
                throw;
            }
        }








    }
}

    

