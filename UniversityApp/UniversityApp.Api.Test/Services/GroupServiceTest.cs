using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UniversityApp.Core.Entites;
using UniversityApp.Data.Repositories.Interfaces;
using UniversityApp.Service.Dtos.GroupDtos;
using UniversityApp.Service.Exceptions;
using UniversityApp.Service.Implementations;
using UniversityApp.Service.Interfaces;

namespace UniversityApp.Api.Test.Services
{
    public class GroupServiceTest
    {
        private Mock<IGroupRepository> _groupRepository;
        private Mock<IMapper> _mapper;

        public GroupServiceTest()
        {
            _groupRepository = new Mock<IGroupRepository>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public void Create_NoExists_ThrowRestException()
        {
            //Arrange
            GroupCreateDto createDto = new GroupCreateDto
            {
                No = "P123",
                Limit = 10
            };

            _groupRepository.Setup(x=>x.Exists(It.IsAny<Expression<Func<Group,bool>>>())).Returns(true);

            IGroupService groupService = new GroupService(_groupRepository.Object, _mapper.Object);
            //Act
            var r =  Assert.Throws<RestException>(() => groupService.Create(createDto));

            //Assert
            Assert.NotNull(r);
            Assert.Equal(400,r.Code);
        }

        [Fact]
        public void Create_Success_ReturnId()
        {
            //Arrange
            GroupCreateDto createDto = new GroupCreateDto
            {
                No = "P123",
                Limit = 10
            };

            var entity = new Group { No = createDto.No, Limit = createDto.Limit };

            _mapper.Setup(x => x.Map<Group>(createDto)).Returns(entity);

            _groupRepository.Setup(x => x.Exists(It.IsAny<Expression<Func<Group, bool>>>())).Returns(false);
            _groupRepository.Setup(x => x.Add(It.IsAny<Group>()));
            _groupRepository.Setup(x=>x.Save()).Callback(()=> entity.Id =1);

            GroupService groupService = new GroupService(_groupRepository.Object, _mapper.Object);

            //Act
            int? r = groupService.Create(createDto);

            //Assert
            Assert.NotNull(r);
            Assert.Equal(1, r);
        }

        [Fact]
        public void Delete_NotFound_ThrowException()
        {
            //Arrange
            Group? entity = null;
            int id = 1;
            _groupRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Group, bool>>>())).Returns(entity);
            IGroupService groupService = new GroupService(_groupRepository.Object, _mapper.Object);

            //Act

            var exp = Assert.Throws<RestException>(() => groupService.Delete(1));

            //Assert
            Assert.NotNull(exp);
            Assert.Equal(404, exp.Code);
        }
    }
}
