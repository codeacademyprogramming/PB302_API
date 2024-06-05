using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.Data.Entites;
using UniversityApi.Dtos.StudentDtos;

namespace UniversityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public StudentsController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpPost("")]
        public ActionResult Create(StudentCreateDto createDto)
        {
            Group group = _context.Groups.Include(x => x.Students).FirstOrDefault(x => x.Id == createDto.GroupId && !x.IsDeleted);

            if (group == null)
            {
                ModelState.AddModelError("GroupId", "Group not found by given GroupId");
                return BadRequest(ModelState);
            }

            if (group.Limit <= group.Students.Count) return Conflict("Group is full");
                
            if (_context.Students.Any(x => x.Email.ToUpper() == createDto.Email.ToUpper() && !x.IsDeleted))
            {
                ModelState.AddModelError("Email", "Email already exists");
                return BadRequest(ModelState);
            }


            Student entity = new Student
            {
                FullName = createDto.FullName,
                Email = createDto.Email,
                BirthDate = createDto.BirthDate,
                GroupId = createDto.GroupId,
            };

            _context.Students.Add(entity);
            _context.SaveChanges();

            return Created(Request.Path, new { id = entity.Id });
        }
    }
}
