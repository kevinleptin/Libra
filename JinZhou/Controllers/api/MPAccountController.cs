using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JinZhou.Handler;
using JinZhou.Models.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Open.Entities.Request;

namespace JinZhou.Controllers.api
{
    [Produces("application/json")]
    [Route("api/MPAccount")]
    public class MpAccountController : Controller
    {
        private readonly WxConfig _wxConfig;

        public MpAccountController(IOptions<WxConfig> wxConfig)
        {
            _wxConfig = wxConfig.Value;
        }
        /// <summary>
        /// 授权事件接收URL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Auth()
        {
            PostModel pm = new PostModel();
            pm.Timestamp = Request.Query["timestamp"];
            pm.Nonce = Request.Query["nonce"];
            pm.Msg_Signature = Request.Query["msg_signature"];

            pm.AppId = _wxConfig.AppId;
            pm.Token = _wxConfig.Token;
            pm.EncodingAESKey = _wxConfig.AesKey;

            InsideMsgHandler handler = new InsideMsgHandler(Request.Body, _wxConfig, pm);
            handler.Execute();

            return Ok("success");
        }
    }
}