using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;

using CfpExchange.API.Models;
using CfpExchange.Common.Services.Interfaces;
using CfpExchange.Common.Models;

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
        public IActionResult Get()
        {
            var cfps = _cfpService.GetAllActiveCfps();
            
            return new OkObjectResult(TinyMapper.Map<List<CfpData>>(cfps));
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var cfp = _cfpService.GetCfpById(id);

            return new OkObjectResult(TinyMapper.Map<CfpData>(cfp));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CfpData cfpData)
        {
            var cfp = TinyMapper.Map<Cfp>(cfpData);
            await _cfpService.AddCfpAsync(cfp);

            return new OkObjectResult(TinyMapper.Map<CfpData>(cfp));
        }
    }
}
