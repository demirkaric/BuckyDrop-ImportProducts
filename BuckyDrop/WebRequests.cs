using System.IO;
using System.Net;
using System.Text;

namespace BuckyDrop
{
    public class WebRequests
    {
        private readonly CookieContainer cookies = new CookieContainer();

        /// <summary>
        /// Sending GET request using custom options
        /// </summary>
        /// <param name="link"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public string Get(string link, string referer)
        {
            var request = (HttpWebRequest)WebRequest.Create(link);
            request.CookieContainer = cookies;
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Headers.Add("accept-language", "en,hr;q=0.9");
            request.Headers.Add("accept-encoding", "");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Referer = referer;
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return responseFromServer;
        }

        /// <summary>
        /// Sending POST request using custom options
        /// </summary>
        /// <param name="link"></param>
        /// <param name="data"></param>
        /// <param name="content_type"></param>
        /// <param name="referer"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public string Post(string link, string data, string content_type, string referer, bool json)
        {
            var request = (HttpWebRequest)WebRequest.Create(link);
            request.CookieContainer = cookies;
            var dataLength = Encoding.ASCII.GetBytes(data);
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36";
            request.Accept = "*/*";
            request.Headers.Add("accept-language", "en,hr;q=0.9");
            request.Headers.Add("accept-encoding", "");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.ContentType = content_type;
            request.ContentLength = data.Length;
            if (referer != null)
            {
                request.Referer = referer;
            }
            if (json)
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            else
            {
                var newStream = request.GetRequestStream();
                newStream.Write(dataLength, 0, data.Length);
                newStream.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return responseFromServer;
        }
    }
}