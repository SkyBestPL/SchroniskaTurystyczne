using Microsoft.AspNetCore.Mvc.Testing;
using SchroniskaTurystyczne;
namespace SchroniskaTurystyczneTestProject
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UnitTest1()
        {
            var factory = new WebApplicationFactory<Program>();
            _factory = factory;
        }

        [Fact]
        public void Test1()
        {

        }
    }
}