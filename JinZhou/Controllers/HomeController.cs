using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JinZhou.Models;
using JinZhou.Models.CommEntity;
using JinZhou.Models.Configuration;
using Microsoft.Extensions.Options;
using JinZhou.Models.ViewModels;
using Senparc.Weixin.Open.ComponentAPIs;

namespace JinZhou.Controllers
{
    public class HomeController : Controller
    {
        private readonly WxConfig _wxConfig;
        public HomeController(IOptions<WxConfig> wxConfig)
        {
            _wxConfig = wxConfig.Value;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Install()
        {
            HomeInstallViewModel vm = new HomeInstallViewModel();
            vm.WxAppId = _wxConfig.AppId;
            vm.RedirectUri = _wxConfig.RedirectUri;
            var tokenResult = ComponentApi.GetComponentAccessToken(_wxConfig.AppId, _wxConfig.AppSecret,"");
            var preAuthCodeResult = ComponentApi.GetPreAuthCode(_wxConfig.AppId, tokenResult.component_access_token);
            vm.PreAuthCode = preAuthCodeResult.pre_auth_code;
            return View(vm);
        }

        public IActionResult WxComm()
        {
            HomeWxCommViewModel vm = new HomeWxCommViewModel();
            vm.AccessData = ComponentKeys.GetInstance().AccessData;
            vm.PreAuthData = ComponentKeys.GetInstance().PreAuthData;
            vm.VerifyData = ComponentKeys.GetInstance().VerifyData;
            return View(vm);
        }
    }
}
