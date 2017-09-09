using System;
using System.Threading.Tasks;
using AutoMapper;
using Evento.Core.Domain;
using Evento.Core.Repositories;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Evento.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task register_async_should_invoke_add_async_on_user_repository()
        {

            // ARRANGE
            // testujemy UserService
            // user service wymaga w konstruktorze 3 instancji, dlatego je moqujemy

            var userRepositoryMock = new Mock<IUserRepository>();
            var jwtHandlerMock = new Mock<IJwtHandler>();
            var mapperMock = new Mock<IMapper>();

            var userService = new UserService(userRepositoryMock.Object, jwtHandlerMock.Object,
                mapperMock.Object);

            // ACT
            await userService.RegisterAsync(Guid.NewGuid(), "test@test.com", "test", "secret", "user");

            // ASSERT
            userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public async Task when_invoking_get_async_it_should_invoke__get_async_on_user_repository()
        {

            // ARRANGE
            // testujemy UserService
            // user service wymaga w konstruktorze 3 instancji, dlatego je moqujemy
            var user = new User(Guid.NewGuid(), "user", "test", "test@test.com", "secret");
            var accountDto = new AccountDto
            {
                Id = user.Id,
                Role = user.Role,
                Email = user.Email,
                Name = user.Name
            };
            var userRepositoryMock = new Mock<IUserRepository>();
            var jwtHandlerMock = new Mock<IJwtHandler>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(Xunit => Xunit.Map<AccountDto>(user)).Returns(accountDto);

            var userService = new UserService(userRepositoryMock.Object, jwtHandlerMock.Object,
                mapperMock.Object);

            userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(user);

            // ACT
            var existingAccountDto = await userService.GetAccountAsync(user.Id);

            // ASSERT
            userRepositoryMock.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once());
            accountDto.Should().NotBeNull();
            accountDto.Email.ShouldAllBeEquivalentTo(user.Email);
            accountDto.Role.ShouldAllBeEquivalentTo(user.Role);
            accountDto.Name.ShouldAllBeEquivalentTo(user.Name);
        }
    }
}