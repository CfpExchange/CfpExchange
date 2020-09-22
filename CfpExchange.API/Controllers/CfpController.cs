using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;

using CfpExchange.API.Models;
using CfpExchange.Common.Services.Interfaces;

namespace CfpExchange.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CfpController : ControllerBase
    {
        #region Fields

        private readonly ICfpService _cfpService;

        #endregion

        #region Constructors

        public CfpController(ICfpService cfpService)
        {
            _cfpService = cfpService;
        }

        #endregion

        [HttpGet]
        public IEnumerable<CfpData> Get()
        {
            var cfps = _cfpService.GetAllActiveCfps();
            
            return TinyMapper.Map<List<CfpData>>(cfps);
        }

        [HttpGet("{id}")]
        public CfpData Get(Guid id)
        {
            var cfp = _cfpService.GetCfpById(id);

            return TinyMapper.Map<CfpData>(cfp);
        }

        [HttpPost]
        public CfpData Post(CfpData cfpData)
        {
            // TODO: implement
            return cfpData;
        }
    }
}
