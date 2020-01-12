
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog.Debugging;

namespace NerdyMishka.Extensions.Logging
{

    public class LogAnalyticsWorkspace
    {
        public string Id { get; set; }

        public string Key { get; set; }

        public string Uri { get; set; }

        public string LogType { get; set; } = "NerdyMishkaLogV1";

        public bool Debug { get; set; } = false;
    }

    public class LogAnalyticsWriter : IDisposable
    {
        private LogAnalyticsWorkspace options;

        private HttpClient httpClient;

        private ILogger logger;

        private bool dispose = false;


        public LogAnalyticsWriter(
            LogAnalyticsWorkspace workspace, 
            HttpClient httpClient = null,
            ILogger<LogAnalyticsWriter> logger = null)
        {
            this.options = workspace ?? new LogAnalyticsWorkspace() {
                Debug = true 
            };
            if(this.options.Uri == null && this.options.Id != null)
            {
                this.options.Uri = $"https://{this.options.Id}.ods.opinsights.azure.com/api/logs?api-version=2016-04-01";
            }

            this.httpClient = httpClient ?? new HttpClient();
            this.logger = logger;
        }


        public void Write(string json, string logType = null)
        {
            Task.Run(async() => {
                await this.WriteAsync(logType, json);
            });
        }

        public async Task WriteAsync(string json, string logType = null)
        {
            logType = logType ?? this.options.LogType;
            var response = await this.SendAsync(logType, json);
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string context = await response.Content.ReadAsStringAsync();
                logger?.LogError($"LogAnalytics write failed: {response.StatusCode} - {context}.");
                SelfLog.WriteLine($"LogAnalytics write failed: {response.StatusCode} - {context}.");
            } 
        }

        public async Task<HttpResponseMessage> SendAsync(string json, string logType = null)
        {
            logType = logType ?? this.options.LogType;
            string date = DateTime.UtcNow.ToString("r");

            if(this.options == null || this.options.Key == null)
            {
                this.logger?.LogDebug(json);
            }

            string authSignature = GetAuthSignature(json, date, this.options.Id, this.options.Key);

            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, this.options.Uri))
            {
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", authSignature);
                httpRequestMessage.Headers.Add("Log-Type", logType);
                httpRequestMessage.Headers.Add("Accept", "application/json");
                httpRequestMessage.Headers.Add("x-ms-date", date);                    

                httpRequestMessage.Content = new System.Net.Http.StringContent(json);
                httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return await this.httpClient.SendAsync(httpRequestMessage);                    
            }                                  
        }
     

        private static string GetAuthSignature(
            string serializedJsonObject, 
            string dateString, 
            string workspaceId,
            string workspaceKey)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"POST")
                .Append($"\n{serializedJsonObject.Length}")
                .Append("\napplication/json")
                .Append($"\nx-ms-date:{dateString}")
                .Append("\n/api/logs");

            var messageInfo = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            var key = Convert.FromBase64String(workspaceKey);
            using (var hasher = new HMACSHA256(key))
            {
                var messageHash = Convert.ToBase64String(hasher.ComputeHash(messageInfo));
                return $"{workspaceId}:{messageHash}";
            }
        }

        public void Dispose()
        {
            if(!this.dispose)
            {
                this.httpClient.Dispose();
                this.httpClient = null;
                this.dispose = true;
            }
        }
    }
}