using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using CfpExchange.API.Models;
using CfpExchange.Common.Services.Interfaces;

namespace CfpExchange.API.Controllers
{
    public class CfpController : ControllerBase
    {
        private ICfpService _cfpService;

        public CfpController(ICfpService cfpService)
        {
            _cfpService = cfpService;
        }

        [HttpGet]
        public IEnumerable<CfpData> Get()
        {
            // TODO implement
            return null;
        }

        [HttpGet]
        public CfpData GetById(Guid id)
        {
            // TODO implement
            return null;
        }
    }
}
