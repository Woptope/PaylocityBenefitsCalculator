using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTests
{
    public class IntegrationTest : IDisposable
    {
        private HttpClient? _httpClient;

        protected HttpClient HttpClient
        {
            get
            {
                if (_httpClient == default)
                {
                    _httpClient = new HttpClient
                    {
                        BaseAddress = new Uri("https://localhost:7124")
                    };
                    _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                }

                return _httpClient;
            }
        }

        public IntegrationTest()
        {
            EnsureApiIsRunning().GetAwaiter().GetResult();
        }

        private async Task EnsureApiIsRunning()
        {
            const int maxAttempts = 10;
            const int delayBetweenAttempts = 1000; // 1 second
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    var response = await HttpClient.GetAsync("/api/v1/employees");
                    if (response.IsSuccessStatusCode)
                    {
                        return; // API is up and running
                    }
                }
                catch (HttpRequestException)
                {
                    // Ignore the exception and try again
                }
                await Task.Delay(delayBetweenAttempts);
            }
            throw new Exception("Unable to connect to the API after multiple attempts.");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
