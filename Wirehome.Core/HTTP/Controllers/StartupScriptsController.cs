﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Wirehome.Core.System.StartupScripts;

namespace Wirehome.Core.HTTP.Controllers
{
    [ApiController]
    public class StartupScriptsController : Controller
    {
        private readonly StartupScriptsService _startupScriptsService;

        public StartupScriptsController(StartupScriptsService startupScriptsService)
        {
            _startupScriptsService = startupScriptsService ?? throw new ArgumentNullException(nameof(startupScriptsService));
        }

        [HttpGet]
        [Route("api/v1/startup_scripts/uids")]
        public List<string> GetStartupScriptUids()
        {
            return _startupScriptsService.GetStartupScriptUids();
        }

        [HttpGet]
        [Route("api/v1/startup_scripts")]
        [ApiExplorerSettings(GroupName = "v1")]
        public List<StartupScriptInstance> GetStartupScripts()
        {
            return _startupScriptsService.GetStartupScripts();
        }

        [HttpGet]
        [Route("api/v1/startup_scripts/{uid}")]
        [ApiExplorerSettings(GroupName = "v1")]
        public StartupScriptInstance GetStartupScript(string uid)
        {
            var startupScript = _startupScriptsService.GetStartupScripts().FirstOrDefault(s => s.Uid == uid);
            if (startupScript == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }

            return startupScript;
        }

        [HttpPost]
        [Route("api/v1/startup_scripts/{uid}/configuration")]
        [ApiExplorerSettings(GroupName = "v1")]
        public void PostConfiguration(string uid, [FromBody] StartupScriptConfiguration configuration)
        {
            _startupScriptsService.WriteStartupScripConfiguration(uid, configuration);
        }

        [HttpGet]
        [Route("api/v1/startup_scripts/{uid}/configuration")]
        [ApiExplorerSettings(GroupName = "v1")]
        public StartupScriptConfiguration GetConfiguration(string uid)
        {
            try
            {
                return _startupScriptsService.ReadStartupScriptConfiguration(uid);
            }
            catch (StartupScriptNotFoundException)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }
        }

        [HttpDelete]
        [Route("api/v1/startup_scripts/{uid}")]
        [ApiExplorerSettings(GroupName = "v1")]
        public void Delete(string uid)
        {
            _startupScriptsService.DeleteStartupScript(uid);
        }

        [HttpGet]
        [Route("api/v1/startup_scripts/{uid}/script")]
        [ApiExplorerSettings(GroupName = "v1")]
        public string GetScript(string uid)
        {
            return _startupScriptsService.ReadStartupScriptCode(uid);
        }

        [HttpPost]
        [Route("api/v1/startup_scripts/{uid}/script")]
        [ApiExplorerSettings(GroupName = "v1")]
        public async Task PostScript(string uid)
        {
            using (var streamReader = new StreamReader(HttpContext.Request.Body))
            {
                var scriptCode = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                _startupScriptsService.WriteStartupScriptCode(uid, scriptCode);
            }
        }
    }
}
