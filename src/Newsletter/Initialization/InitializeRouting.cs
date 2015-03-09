using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace BVNetwork.EPiSendMail.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class InitializeRouting : IInitializableModule
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(InitializeRouting));

        public void Initialize(InitializationEngine context)
        {
            // Web API routing
            var config = GlobalConfiguration.Configuration;
            string apiRouteTemplate = Configuration.NewsLetterConfiguration.GetModuleBaseDir().TrimStart(new[] {'/'}) +
                                   "/api/{controller}/{action}/{id}";
            _log.DebugFormat("Initializing Newsletter API on route: {0}", apiRouteTemplate);
            config.Routes.MapHttpRoute(
                name: "NewsletterApi",
                routeTemplate: apiRouteTemplate,
                defaults: new { id = RouteParameter.Optional }
            );

           
            //// Standard routing - needs to be after API (as this catches all below the module root)
            // string pluginRouteTemplate = Configuration.NewsLetterConfiguration.GetModuleBaseDir().TrimStart(new[] { '/' }) +
            //                        "/{controller}/{action}/{id}";
            // _log.DebugFormat("Initializing Newsletter plugin on route: {0}", pluginRouteTemplate);
            //RouteTable.Routes.MapRoute("NewsletterPlugin", pluginRouteTemplate,
            //                            new { action = "Index", id = UrlParameter.Optional },
            //                            new string[] { "BVNetwork.EPiSendMail.Controllers" });


        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }
    }
}