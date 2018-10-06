using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JinZhou.Handler;
using JinZhou.Models;
using JinZhou.Models.Configuration;
using JinZhou.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Open.Entities.Request;

namespace JinZhou.Controllers.api
{
    [Produces("application/json")]
    [Route("MPAccount")]
    public class MpAccountController : Controller
    {
        private readonly WxConfig _wxConfig;
        private JzDbContext db;
        public MpAccountController(IOptions<WxConfig> wxConfig, JzDbContext db)
        {
            this.db = db;
            _wxConfig = wxConfig.Value;
        }
        /// <summary>
        /// 授权事件接收URL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Auth")]
        public IActionResult Auth()
        {
            LogService.GetInstance().AddLog("MPAccount:Auth","Thread"+Thread.CurrentThread.ManagedThreadId,"Begin Handle Auth","","Info");
            
            PostModel pm = new PostModel();
            pm.Timestamp = Request.Query["timestamp"];
            pm.Nonce = Request.Query["nonce"];
            pm.Msg_Signature = Request.Query["msg_signature"];

            pm.AppId = _wxConfig.AppId;
            pm.Token = _wxConfig.Token;
            pm.EncodingAESKey = _wxConfig.AesKey;
            
            // LogService.GetInstance().AddLog("MPAccount:Auth", "Thread" + Thread.CurrentThread.ManagedThreadId, "Query is "+Request.QueryString, "", "Info");
          
            InsideMsgHandler handler = new InsideMsgHandler(db, Request.Body, _wxConfig, pm);
            handler.Execute();

            LogService.GetInstance().AddLog("MPAccount:Auth", "Thread" + Thread.CurrentThread.ManagedThreadId, "End Handle Auth", "", "Info");
            return Ok("success");
        }
    }
}