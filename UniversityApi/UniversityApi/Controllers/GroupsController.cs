using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityApi.Data.Entites;
using UniversityApi.Dtos.GroupDtos;

namespace UniversityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        public static List<Group> groups = new List<Group>();
        public GroupsController()
        {
            groups = new List<Group>
            {
                new Group
                {
                    No = "PB301",
                    Id=1,
                    Limit = 10,
                    CreatedAt = DateTime.Now.AddYears(-1),
                    ModifiedAt = DateTime.Now.AddMonths(-2)
                },
                new Group
                {
                    No = "PB302",
                    Id=2,
                    Limit = 14,
                    CreatedAt = DateTime.Now.AddYears(-2),
                    ModifiedAt = DateTime.Now.AddMonths(-4)
                }
            };
        }
        [HttpGet("")]
        public ActionResult<List<GroupGetDto>> GetAll()
        {
            List<GroupGetDto> dtos = groups.Select(x => new GroupGetDto
            {
                Id = x.Id,
                No = x.No,
                Limit = x.Limit
            }).ToList();

            return StatusCode(200, dtos);
        }

        //[Route("{id}")]
        [HttpGet("{id}")]

        public ActionResult<GroupGetDto> GetById(int id)
        {
            var data = groups.Find(x => x.Id == id);

            if(data == null)
            {
                return StatusCode(404);
            }

            GroupGetDto dto = new GroupGetDto
            {
                Id = data.Id,
                No = data.No,
                Limit = data.Limit
            };
            return StatusCode(200, dto);
        }
        [HttpPost("")]
        public ActionResult Create(Group group)
        {
            groups.Add(group);
            return StatusCode(201);
        }
    }
}
