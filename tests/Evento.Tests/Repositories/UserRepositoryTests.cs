using System;
using System.Threading.Tasks;
using Evento.Core.Domain;
using Evento.Core.Repositories;
using Evento.Infrastructure.Repositories;
using Xunit;

namespace Evento.Tests.Repositories
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task when_adding_new_user_it_should_be_added_correctly_to_the_list()
        {
            //Arrange - cały setup, przypisujemy zmienne, wartosci itp
            var user = new User(Guid.NewGuid(), "user", "test", "test@test.com", "secret");
            IUserRepository repository = new UserRepository();

            //Act - operacja, która ma zostać sprawdzona
            await repository.AddAsync(user);

            //Assert - sprawdzenie, czy test jest poprawny
            var existingUser = await repository.GetAsync(user.Id);
            Assert.Equal(user, existingUser);
        }
    }
}