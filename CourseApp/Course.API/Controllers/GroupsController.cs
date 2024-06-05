using Course.Core.Entities;
using Course.Data;
using Course.Service.Dtos;
using Course.Service.Exceptions;
using Course.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("")]
        public ActionResult Create(GroupCreateDto createDto)
        {
            try
            {
                return StatusCode(201, new { id = _groupService.Create(createDto) });
            }
            catch (DublicateEntityException e)
            {
                return Conflict();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Bilinmedik bir xeta bas verdi");
            }
        }

        [HttpGet("")]
        public ActionResult<List<GroupGetDto>> GetAll()
        {
            return Ok(_groupService.GetAll());
        }
    }
}
