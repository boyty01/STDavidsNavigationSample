using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StDavidsQRNavigation.Models;
using StDavidsQRNavigation.Services;
using System.Net;

namespace StDavidsQRNavigation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantEventController : ControllerBase
    {
        private readonly IRepository<TenantEventModel> _repo;
        private readonly ITenantService _tenantService;
        public TenantEventController(IRepository<TenantEventModel> repository, ITenantService tenantService)
        {
            _repo = repository;
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TenantEventModel>>> GetAll([FromQuery] bool includeRelated = false)
        {
            var result = await _repo.GetAll(includeRelated);
            return HandleResponse<List<TenantEventModel>>(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TenantEventModel>> Get(int id, [FromQuery] bool includeRelated = false)
        {
            var result = await _repo.Get(id, includeRelated);
            return HandleResponse<TenantEventModel>(result);
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<TenantEventModel>> Update(int id, TenantEventModel model)
        {
            var result = await _repo.Update(model);
            return HandleResponse<TenantEventModel>(result);
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TenantEventModel>> Create(TenantEventDto dto)
        {
            var model = new TenantEventModel
            {
                Title = dto.Title,
                Representative = dto.Representative,
                TenantId = dto.TenantId,
                InternalNavId = dto.InternalNavId
            };

            var result = await _repo.Create(model);
            return HandleResponse<TenantEventModel>(result);
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _repo.Delete(id);
            return HandleResponse<bool>(result);
        }


        [Authorize]
        [HttpPut("{eventId}/link-path/{navId}")]
        public async Task<ActionResult<bool>> LinkPath(int eventId, int navId)
        {
            var result = await _tenantService.AssignNavPathToEvent(eventId, navId);
            return HandleResponse<bool>(result);
        }

        private ActionResult<T> HandleResponse<T>(ServiceResult<T> result, Uri? uri = null)
        {
            if (result.IsSuccess)
            {
                if (result.HttpCode == HttpStatusCode.Created)
                    return Created(uri, result.Data);

                return StatusCode((int)result.HttpCode, result.Data);
            }
            return StatusCode((int)result.HttpCode, new { Error = result.ErrorMessage });
        }
    }
}
