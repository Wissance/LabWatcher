using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wissance.MossabuerLab.Watcher.Dto;
using Wissance.MossbauerLab.Watcher.Data.Entities;
using Wissance.MossbauerLab.Watcher.Web.Managers;
using Wissance.WebApiToolkit.Controllers;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.MossbauerLab.Watcher.Web.Controllers
{
    public class SpectrumController : BasicReadController<SpectrumInfoDto, SpectrumEntity, int>
    {
        public SpectrumController(SpectrumManager manager)
        {
            Manager = manager;
            _manager = manager;
        }

        // http://localhost:7890/api/Spectrum/3/Samples
        [HttpGet]
        [Route("api/[controller]/{id}/samples")]
        public async Task<SpectrumSamplesInfoDto> ReadSamplesByIdAsync([FromRoute] int id)
        {
            OperationResultDto<SpectrumSamplesInfoDto> result = await _manager.GetSpectrumSamplesAsync(id);
            HttpContext.Response.StatusCode = result.Status;
            return result.Data;
        }

        [HttpGet]
        [Route("api/[controller]/{id}/samples/{sampleName}/spectrum")]
        public async Task<HttpResponseMessage> ReadSpectrumSampleFileAsync([FromRoute] int id, [FromRoute] string sampleName)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            OperationResultDto<byte[]> result = await _manager.GetSpectrumSampleFileAsync(id, sampleName);
            HttpContext.Response.StatusCode = result.Status;
            if (result.Data != null)
            {
                //return File(result.Data, )
                resp.Content = new ByteArrayContent(result.Data);
                resp.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                resp.Content.Headers.ContentDisposition.FileName = sampleName;
                resp.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            }

            return resp;
        }

        private readonly SpectrumManager _manager;
    }
}
