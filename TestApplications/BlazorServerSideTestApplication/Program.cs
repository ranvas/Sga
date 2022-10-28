using BlazorServerSideTestApplication;
using Integrators.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;

await WebIntegrator.RunWebAsync<Startup>(args);