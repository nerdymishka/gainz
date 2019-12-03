using NerdyMishka;
using Xunit;
using System.Net.Http;

namespace Tests
{
    //[Integration]
    [Trait("tag", "integration")]
    public class FetchTests
    {

        [Fact]
        public void Invoke()
        {
            var response = Fetch.Invoke("https://www.google.com");
            Assert.NotNull(response);

            var content = response.ToContent();
            Assert.NotNull(content);
            Assert.Contains("<title>Google</title>", content);
        }

        [Fact]
        public static async void InvokeAsync()
        {
            var response = await Fetch.InvokeAsync("https://www.google.com");
            Assert.NotNull(response);

            var content = response.ToContent();
            Assert.NotNull(content);
            Assert.Contains("<title>Google</title>", content);
        }

        [Fact]
        public void Invoke_ExplicitGet()
        {
            var response = Fetch.Invoke("https://www.google.com/search?q=test", "GET");
            Assert.NotNull(response);

            var content = response.ToContent();
            Assert.NotNull(content);
            Assert.Contains("<title>test - Google Search</title>", content);  
        }

        [Fact]
        public async void Invoke_ExplicitGetAsync()
        {
            var response = await Fetch.InvokeAsync("https://www.google.com/search?q=test", "GET");
            Assert.NotNull(response);

            var content = response.ToContent();
            Assert.NotNull(content);
            Assert.Contains("<title>test - Google Search</title>", content);  
        }

        [Fact]
        public void Download()
        {
            // TODO: have a URI on nerdymishka.com or github
            var uri = new System.Uri("https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7BEEB48E36-53F1-A5A0-E42E-A3F14270B410%7D%26lang%3Den%26browser%3D4%26usagestats%3D0%26appname%3DGoogle%2520Chrome%26needsadmin%3Dtrue%26ap%3Dx64-stable-statsdef_0%26brand%3DGCEO/dl/chrome/install/GoogleChromeEnterpriseBundle64.zip");
            var path =  Env.ResolvePath("~/Resources/googlechrome.zip");
            var dir = Env.ResolvePath("~/Resources");

            if(!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            if(System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            Assert.False(System.IO.File.Exists(path));
            
            var fi = Fetch.Download(uri, path);

            Assert.True(fi.Exists);
            fi.Delete();
        }


        [Fact]
        public async void DownloadAsync()
        {
            // TODO: have a URI on nerdymishka.com or github
            var uri = new System.Uri("https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7BEEB48E36-53F1-A5A0-E42E-A3F14270B410%7D%26lang%3Den%26browser%3D4%26usagestats%3D0%26appname%3DGoogle%2520Chrome%26needsadmin%3Dtrue%26ap%3Dx64-stable-statsdef_0%26brand%3DGCEO/dl/chrome/install/GoogleChromeEnterpriseBundle64.zip");
            var path =  Env.ResolvePath("~/Resources/googlechrome2.zip");
            var dir = Env.ResolvePath("~/Resources");

            if(!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            if(System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            Assert.False(System.IO.File.Exists(path));
            
            var fi = await Fetch.DownloadAsync(uri, path);

            Assert.True(fi.Exists);
            System.IO.File.Delete(path);
        }

    }
}