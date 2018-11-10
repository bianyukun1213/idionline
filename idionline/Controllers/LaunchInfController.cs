﻿using System;
using System.Collections.Generic;
using System.Linq;
using Idionline.Models;
using Microsoft.AspNetCore.Mvc;

namespace Idionline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaunchInfController : ControllerBase
    {
        private readonly IdionlineContext _context;
        public LaunchInfController(IdionlineContext context)
        {
            _context = context;
            if (_context.LaunchInf.Count() == 0)
            {
                _context.LaunchInf.Add(new LaunchInf { Text = "默认文本", DailyIdiomName = "成语名称", DailyIdiomId = 1, DateUT = DateTimeOffset.MinValue.ToUnixTimeSeconds() });
                _context.SaveChanges();
            }
        }
        [HttpGet("{date}")]
        public ActionResult<List<LaunchInf>> GetLaunchInf(long date)
        {
            var common = _context.LaunchInf.Find(DateTimeOffset.MinValue.ToUnixTimeSeconds());
            var item = _context.LaunchInf.Find(date);
            if (common == null)
            {
                return NotFound();
            }
            List<LaunchInf> list = new List<LaunchInf>
            {
                common
            };
            if (item != null)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
