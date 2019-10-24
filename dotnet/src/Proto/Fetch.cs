using System.Net;
using System.Collections;
using System;
using System.Security;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NerdyMishka.Validation;

namespace NerdyMishka
{
    public class Fetch
    {
        private static string s_invalidChars = null;
        public static ProxyConfig Proxy { get; set; }

        public static Options Defaults { get; set; }

        static Fetch()
        {
            var invalidChars = System.IO.Path.GetInvalidFileNameChars().ToString();
            invalidChars = System.Text.RegularExpressions.Regex.Escape(invalidChars);
            invalidChars = $"[{invalidChars}\\=\\;]";
            s_invalidChars = invalidChars;
            Defaults = new Options();
        }

        public class ProxyConfig
        {
            public Uri Uri { get; set; }

            public string Username { get; set; }

            public SecureString Password { get; set; }

            public string[] BypassList { get; set; }

            public bool BypassOnLocal { get; set; }
        }

        public class Options
        {
            public Options()
            {
                this.Accept = "*/*";
                this.Decompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
                this.MaxiumRedirections = 20;
                this.Timeout = 3000;
                this.Headers = new Dictionary<string, string>() {
                    { "Accept" , "*/*" },
                    { "User-Agent", "nerdy console" }
                };
            }
            public string Accept { get; set; }

            public WebProxy Proxy { get; set; }

            public string Referrer { get; set; }

            public string Cookie { get; set; }

            public int MaxiumRedirections { get; set; }

            public int Timeout { get; set; }

            public int? ResponseTimeout { get; set; }

            public string UserAgent { get; set; }

            public DecompressionMethods Decompression { get; set; }
             
            public IDictionary Headers { get; set; }

        }


        private static WebProxy GetProxy()
        {
            var config = Proxy;
            if(config == null || config.Uri == null)
                return null;

            var proxy = new System.Net.WebProxy(config.Uri);

            if(config.Password != null)
                proxy.Credentials = new NetworkCredential(config.Username, config.Password);
            
            if(config.BypassList != null && config.BypassList.Length > 0)
                proxy.BypassList = config.BypassList;

            proxy.BypassProxyOnLocal = config.BypassOnLocal;

            return proxy;
        }

         private static async Task<HttpWebRequest> CreateAsync(Uri uri, string method, 
            string body = null, 
            string contentType = null, 
            Encoding encoding = null, 
            Options options = null)
        {
            options = options ?? Defaults;
            var request  =(HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
            request.Method  = method;
            request.Timeout = options.Timeout;
            request.AllowAutoRedirect = options.MaxiumRedirections > 0;
            request.MaximumAutomaticRedirections = options.MaxiumRedirections;
            request.AutomaticDecompression = options.Decompression;
            if(options.ResponseTimeout.HasValue) 
                request.ReadWriteTimeout = options.ResponseTimeout.Value;

            request.CookieContainer = new CookieContainer();
            
            var m = method.ToLowerInvariant();
            if(!string.IsNullOrWhiteSpace(body) && (m == "post" || m == "put"))
            {
                encoding = encoding ?? new System.Text.UTF8Encoding(false);
                var bytes = encoding.GetBytes(body);
                byte[] buffer = new byte[32768];
                int bytesRead = 0;
                using(var stream = await request.GetRequestStreamAsync())
                using(var ms = new MemoryStream(bytes))
                {
                    while((bytesRead = await ms.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await stream.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
            }


            foreach(var key in options.Headers.Keys)
            {
                var k = key.ToString();
                var value = options.Headers[key] as string;
                switch(k)
                {
                    case "Accept":
                        request.Accept = value;
                        break;
                    case "Referrer":
                        request.Referer = value;
                        break;
                    case "User-Agent":
                        request.UserAgent = value;
                        break;
                    case "Cookie":
                        request.CookieContainer.SetCookies(uri, value);
                    break;
                    default:
                        request.Headers.Add(key.ToString(), value.ToString());
                    break;
                }
            }
          
            var credentials = System.Net.CredentialCache.DefaultCredentials;
            if(credentials != null)
                request.Credentials = credentials;

            WebProxy proxy = options.Proxy ?? GetProxy();
            if(proxy != null)
                request.Proxy = proxy;

            return request;
        }

        private static HttpWebRequest Create(Uri uri, string method, 
            string body = null, 
            string contentType = null, 
            Encoding encoding = null, 
            Options options = null)
        {
            options = options ?? Defaults;
            var request  =(HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
            request.Method  = method;
            request.Timeout = options.Timeout;
            request.AllowAutoRedirect = options.MaxiumRedirections > 0;
            request.MaximumAutomaticRedirections = options.MaxiumRedirections;
            request.AutomaticDecompression = options.Decompression;
            if(options.ResponseTimeout.HasValue) 
                request.ReadWriteTimeout = options.ResponseTimeout.Value;

            request.CookieContainer = new CookieContainer();
            
            var m = method.ToLowerInvariant();
            if(!string.IsNullOrWhiteSpace(body) && (m == "post" || m == "put"))
            {
                encoding = encoding ?? new System.Text.UTF8Encoding(false);
                var bytes = encoding.GetBytes(body);
                byte[] buffer = new byte[32768];
                int bytesRead = 0;
                using(var stream = request.GetRequestStream())
                using(var ms = new MemoryStream(bytes))
                {
                    while((bytesRead = ms.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }


            foreach(var key in options.Headers.Keys)
            {
                var k = key.ToString();
                var value = options.Headers[key] as string;
                switch(k)
                {
                    case "Accept":
                        request.Accept = value;
                        break;
                    case "Referrer":
                        request.Referer = value;
                        break;
                    case "User-Agent":
                        request.UserAgent = value;
                        break;
                    case "Cookie":
                        request.CookieContainer.SetCookies(uri, value);
                    break;
                    default:
                        request.Headers.Add(key.ToString(), value.ToString());
                    break;
                }
            }
          
            var credentials = System.Net.CredentialCache.DefaultCredentials;
            if(credentials != null)
                request.Credentials = credentials;

            WebProxy proxy = options.Proxy ?? GetProxy();
            if(proxy != null)
                request.Proxy = proxy;

            return request;
        }


        public static HttpWebResponse Invoke(
            string uri, 
            string method, 
            string body = null,
            string contentType = null,
            Encoding encoding = null,
            Options options = null ,
            CancellationToken token = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(uri), uri);

            var request = Create(new Uri(uri), method, body, contentType, encoding, options);
            return (HttpWebResponse) request.GetResponse();
        }


        public static HttpWebResponse Invoke(
            Uri uri, 
            string method, 
            string body = null,
            string contentType = null,
            Encoding encoding = null,
            Options options = null ,
            CancellationToken token = default(CancellationToken))
        {
            var request = Create(uri, method, body, contentType, encoding, options);
            return (HttpWebResponse) request.GetResponse();
        }


        public static HttpWebResponse Invoke(
            string uri, 
            Options options = null ,
            CancellationToken token = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(uri), uri);

            return Invoke(new Uri(uri), options, token);
        }

        public static HttpWebResponse Invoke(
            Uri uri, 
            Options options = null ,
            CancellationToken token = default(CancellationToken))
        {
            var request = Create(uri, "GET", null, null, null, options);
            return (HttpWebResponse) request.GetResponse();
        }



        public static async Task<HttpWebResponse> InvokeAsync(
            string uri,
            Options options = null,
            CancellationToken token = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(uri), uri);

            var request = await CreateAsync(new Uri(uri), "GET", null, null, null, options);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        public static async Task<HttpWebResponse> InvokeAsync(
            Uri uri,
            Options options = null,
            CancellationToken token = default(CancellationToken))
        {
            Check.NotNull(nameof(uri), uri);

            var request = await CreateAsync(uri, "GET", null, null, null, options);
            return (HttpWebResponse)await request.GetResponseAsync();
        }


        public static async Task<HttpWebResponse> InvokeAsync(
            string uri, 
            string method, 
            string body = null,
            string contentType = null,
            Encoding encoding = null,
            Options options = null ,
            CancellationToken token = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(uri), uri);

            var request = await CreateAsync(new Uri(uri), method, body, contentType, encoding, options);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        public static async Task<HttpWebResponse> InvokeAsync(
            Uri uri, 
            string method, 
            string body = null,
            string contentType = null,
            Encoding encoding = null,
            Options options = null ,
            CancellationToken token = default(CancellationToken))
        {
            var request = await CreateAsync(uri, method, body, contentType, encoding, options);
            return (HttpWebResponse)await request.GetResponseAsync();
        }

       

        private static string GetFileNameFromHeaders(System.Net.WebResponse response, Uri requestUri)
        {
            string fileName = null;
            var test = new System.Text.RegularExpressions.Regex(s_invalidChars);
            var disposition = response.Headers["Content-Disposition"];
            var location = response.Headers["Location"];
            
            if(!string.IsNullOrWhiteSpace(disposition))
            {
                var index = disposition.LastIndexOf("filename=", StringComparison.OrdinalIgnoreCase);
                if(index  > -1)
                    fileName = disposition.Substring(index).Replace("\"", "");

                if(!test.IsMatch(fileName))
                    return fileName.Trim();
            }
  
            fileName = null;
   
            if(!string.IsNullOrWhiteSpace(location))
            {
                fileName = System.IO.Path.GetFileName(location);
                if(!test.IsMatch(fileName))
                    return fileName.Trim();
            }
  
            fileName = null;

            var responseUri = response.ResponseUri.ToString();

            if(!responseUri.Contains("?")) {
                fileName = System.IO.Path.GetFileName(responseUri);
                if(!test.IsMatch(fileName))
                    return fileName.Trim();
            }

            fileName = null;
    
  
            if(requestUri != null)
            {
                var uri = requestUri.ToString();
                var ext = System.IO.Path.GetExtension(uri);

                if(!uri.Contains("?") && !string.IsNullOrWhiteSpace(ext)) {
                    fileName = System.IO.Path.GetFileName(uri);
                    if(!test.IsMatch(fileName))
                        return fileName.Trim();
                }
            }

            return null;
        }

        public class Progress
        {
            public Decimal PercentComplete { get; set; }

            public long BytesRead  { get; set; }

            public bool Completed { get; set; }

            public long Length { get; set; }
        }


        public static FileInfo Download(
            Uri uri,
            string path,
            Action<Progress> updateProgress = null,

            Options options = null,
            CancellationToken token = default(CancellationToken))
        {
            if(string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var request = Create(uri, "GET", null, null, null, options);
            var response = (HttpWebResponse)request.GetResponse();
            string fileName = null, directory = path;
            var ext = System.IO.Path.GetExtension(path);
            if(ext != null)
            {
                fileName = System.IO.Path.GetFileName(path);
                directory = System.IO.Path.GetDirectoryName(path);
            }

            if(directory.StartsWith(".") || directory.StartsWith("~") && (directory[1] == '/' || directory[1] == '\\'))
            {
                var cd = Environment.CurrentDirectory;
                directory = cd  + directory.Substring(1);
            }

            if(string.IsNullOrWhiteSpace(directory))
            {
                directory = Environment.CurrentDirectory;
            }

            if(string.IsNullOrWhiteSpace(fileName))
            {
                fileName = GetFileNameFromHeaders(response, uri);
                if(fileName == null)
                {
                    fileName = Guid.NewGuid() + ".unknown";
                }
            }

            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new System.Net.WebException($"Download Failed: ({response.StatusCode}) {response.StatusDescription}");
            }

          
            var destination = Path.Combine(directory, fileName);
            long length = response.ContentLength, bytesWritten = 0;
            var buffer = new byte[1048576];
            int bytesRead = 0, i =0;
            using(var stream = response.GetResponseStream())
            using(var writer = new System.IO.FileStream(destination, FileMode.Create))
            {
                do{
                    

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    writer.Write(buffer, 0, bytesRead);
            

                    bytesWritten += bytesRead;
                    if(length > 0 && ++i % 10 == 0) {
                        var percentComplete = Math.Truncate((decimal)(bytesWritten/length)*100);
                        if(updateProgress != null)
                        {
                            updateProgress(new Progress(){
                                BytesRead = bytesWritten,
                                PercentComplete = percentComplete,
                                Length = length
                            });
                        }
                       
                    }

                    if (bytesWritten == length && bytesRead == 0 && updateProgress != null) {
                        updateProgress(new Progress(){
                            BytesRead = bytesWritten,
                            PercentComplete = 100,
                            Length = length,
                            Completed = true
                        });
                    }
                } while(bytesRead > 0);

            }
            
            return new FileInfo(destination);
        }

        public static async Task<FileInfo> DownloadAsync(
            Uri uri,
            string path,
            Action<Progress> updateProgress = null,
            Options options = null,
            CancellationToken token = default(CancellationToken))
        {
            var request = await CreateAsync(uri, "GET", null, null, null, options);
            var response = (HttpWebResponse)await request.GetResponseAsync();
            string fileName = null, directory = path;
            var ext = System.IO.Path.GetExtension(path);
            if(ext != null)
            {
                fileName = System.IO.Path.GetFileName(path);
                directory = System.IO.Path.GetDirectoryName(path);
            }

            if(directory.StartsWith(".") || directory.StartsWith("~") && (directory[1] == '/' || directory[1] == '\\'))
            {
                var cd = Environment.CurrentDirectory;
                directory = cd  + directory.Substring(1);
            }

            if(string.IsNullOrWhiteSpace(directory))
            {
                directory = Environment.CurrentDirectory;
            }

            if(string.IsNullOrWhiteSpace(fileName))
            {
                fileName = GetFileNameFromHeaders(response, uri);
                if(fileName == null)
                {
                    fileName = Guid.NewGuid() + ".unknown";
                }
            }

            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new System.Net.WebException($"Download Failed: ({response.StatusCode}) {response.StatusDescription}");
            }

          
            var destination = Path.Combine(directory, fileName);
            long length = response.ContentLength, bytesWritten = 0;
            var buffer = new byte[1048576];
            int bytesRead = 0, i =0;
            using(var stream = response.GetResponseStream())
            using(var writer = new System.IO.FileStream(destination, FileMode.Create))
            {
                do{
                    

                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    await writer.WriteAsync(buffer, 0, bytesRead);
            

                    bytesWritten += bytesRead;
                    if(length > 0 && ++i % 10 == 0) {
                        var percentComplete = Math.Truncate((decimal)(bytesWritten/length)*100);
                        if(updateProgress != null)
                        {
                            updateProgress(new Progress(){
                                BytesRead = bytesWritten,
                                PercentComplete = percentComplete,
                                Length = length
                            });
                        }
                       
                    }

                    if (bytesWritten == length && bytesRead == 0 && updateProgress != null) {
                        updateProgress(new Progress(){
                            BytesRead = bytesWritten,
                            PercentComplete = 100,
                            Length = length,
                            Completed = true
                        });
                    }
                } while(bytesRead > 0);

            }
            
            return new FileInfo(destination);
            
        }
    }
}