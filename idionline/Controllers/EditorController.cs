﻿using Idionline.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace Idionline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditorController : ControllerBase
    {
        readonly IHttpClientFactory _clientFactory;
        readonly DataAccess data;
        public EditorController(DataAccess d, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            data = d;
        }
        [HttpGet("login")]
        public async Task<StandardReturn> Login(string platTag, string code)
        {
            try
            {
                HttpClient client = _clientFactory.CreateClient();
                var res = platTag switch
                {
                    "WeChat" => await client.GetStringAsync("https://api.weixin.qq.com/sns/jscode2session?appid=wx70c0fb94c40e2986&secret=2160433a6817810b3fc6c3c7731467b9&js_code=" + code + "&grant_type=authorization_code"),
                    "QQ" => await client.GetStringAsync("https://api.q.qq.com/sns/jscode2session?appid=1109616357&secret=koKGigxqQK2HzRJe&js_code=" + code + "&grant_type=authorization_code"),
                    "QB" => null,
                    _ => null,
                };
                client.Dispose();
                JObject jObject = JObject.Parse(res);
                if (jObject["openid"] != null)
                    return new StandardReturn(result: jObject["openid"].ToString());
                else
                    return new StandardReturn(30001);

            }
            catch (Exception)
            {
                return new StandardReturn(30001);
            }
        }
        [HttpPost("register")]
        public async Task<StandardReturn> ResgisterEdi([FromBody] EditorRegistrationData ediDt)
        {
            try
            {
                HttpClient client = _clientFactory.CreateClient();
                string platTag = ediDt.PlatTag;
                var res = platTag switch
                {
                    "WeChat" => await client.GetStringAsync("https://api.weixin.qq.com/sns/jscode2session?appid=wx70c0fb94c40e2986&secret=2160433a6817810b3fc6c3c7731467b9&js_code=" + ediDt.Code + "&grant_type=authorization_code"),
                    "QQ" => await client.GetStringAsync("https://api.q.qq.com/sns/jscode2session?appid=1109616357&secret=koKGigxqQK2HzRJe&js_code=" + ediDt.Code + "&grant_type=authorization_code"),
                    "QB" => null,
                    _ => null,
                };
                client.Dispose();
                JObject json = JObject.Parse(res);
                string openId = json["openid"].ToString();
                return data.RegisterEdi(ediDt.NickName, ediDt.PlatTag + "_" + openId);
            }
            catch (Exception)
            {
                return new StandardReturn(30001);
            }
        }
    }
}