using BlazorServerSideTestApplication;
using Integrators.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace AllTests
{
    public class BasicTests
    {
        [Fact]
        public async Task CheckIntegrator()
        {
            var builder = WebIntegrator.IntegrateBuilder<Startup>(new string[] { });
            builder.UseTestServer();
            var server = new TestServer(builder);
            var client = server.CreateClient();
            
            var response = await client.GetAsync("/");
            
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}