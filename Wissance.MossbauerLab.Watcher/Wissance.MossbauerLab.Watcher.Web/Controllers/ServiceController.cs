using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wissance.MossbauerLab.Watcher.Web.Managers;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.MossbauerLab.Watcher.Web.Controllers
{
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        public ServiceController(ServiceManager manager)
        {
            _manager = manager;
        }

        [HttpGet("send/email/last")]
        public async Task<OperationResultDto<bool>> ManualSendLastSpectraByEmail()
        {
            return await _manager.ManualSendEmailOfLastSavedSpectraAsync();
        }

        private readonly ServiceManager _manager;
    }
}