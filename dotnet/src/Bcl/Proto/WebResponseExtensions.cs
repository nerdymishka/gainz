using System;

namespace NerdyMishka
{

    public static class WebResponseExtensions
    {
        private static readonly System.Text.Encoding utf8NoBom = new System.Text.UTF8Encoding(false);

        public static string ToContent(this System.Net.HttpWebResponse response, Action<Fetch.Progress> updateProgress = null)
        {
            long length = response.ContentLength, bytesWritten = 0;
            var buffer = new byte[1048576];
            int bytesRead = 0, i =0;
            using(var stream = response.GetResponseStream())
            using(var writer = new System.IO.MemoryStream())
            {
                do{
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    writer.Write(buffer, 0, bytesRead);
            
                    bytesWritten += bytesRead;
                    if(length > 0 && ++i % 10 == 0) {
                        var percentComplete = Math.Truncate((decimal)(bytesWritten/length)*100);
                        if(updateProgress != null)
                        {
                            updateProgress(new Fetch.Progress(){
                                BytesRead = bytesWritten,
                                PercentComplete = percentComplete,
                                Length = length
                            });
                        }
                    }

                    if (bytesWritten == length && bytesRead == 0 && updateProgress != null) {
                        
                        updateProgress(new Fetch.Progress(){
                            BytesRead = bytesWritten,
                            PercentComplete = 100,
                            Length = length,
                            Completed = true
                        });
                    }
                } while(bytesRead > 0);

                var bytes = writer.ToArray();
                return utf8NoBom.GetString(bytes);
            }
        }
    }
}