using Hqub.MusicBrainz.API;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WASM.LibraryUtils
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped(x => new MusicBrainzClient(CreateMusicBrainzHttpClient()));

            await builder.Build().RunAsync();
        }

        private static HttpClient CreateMusicBrainzHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://musicbrainz.org/ws/2/");
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Hqub.MusicBrainz", "3.0-beta"));
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("+(https://github.com/avatar29A/MusicBrainz)"));
            return client;
        }
    }
}
