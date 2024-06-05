using Course.Core.Entities;
using Course.Data;
using Course.Service.Dtos;
using Course.Service.Exceptions;
using Course.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Course.Service.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly AppDbContext _context;

        public GroupService(AppDbContext context)
        {
            _context = context;
        }

        public int Create(GroupCreateDto dto)
        {
            if(_context.Groups.Any(x=>x.No == dto.No))
                throw new DublicateEntityException();

            Group entity = new Group
            {
                No = dto.No
            };

            _context.Groups.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public List<GroupGetDto> GetAll()
        {
            return _context.Groups.Select(x => new GroupGetDto
            {
                Id = x.Id,
                No = x.No
            }).ToList();
        }


    }
}
