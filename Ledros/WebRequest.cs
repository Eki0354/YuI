using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StringDict = System.Collections.Generic.Dictionary<string, string>;
using System.IO;

namespace Ledros
{
    public class NetRequest
    {
        public static string Post(string port, string cookieString, string param)
        {
            HttpWebRequest httpReq = WebRequest.Create(new Uri(port)) as HttpWebRequest;
            httpReq.Proxy = null;
            httpReq.Method = "POST";
            httpReq.Accept = "text/html, application/xhtml+xml, */*";
            httpReq.ContentType = "application/x-www-form-urlencoded";
            httpReq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            httpReq.KeepAlive = true;
            httpReq.AllowAutoRedirect = false;
            httpReq.Credentials = CredentialCache.DefaultCredentials;
            httpReq.Headers.Add("Cookie", cookieString);
            byte[] paramBytes = Encoding.GetEncoding("GB2312").GetBytes(param);
            httpReq.ContentLength = paramBytes.Length;
            Stream postStream = httpReq.GetRequestStream();
            postStream.Write(paramBytes, 0, paramBytes.Length);
            postStream.Close();
            var httpResp = httpReq.GetResponse() as HttpWebResponse;
            Stream stream = (httpResp).GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            reader.Close();
            stream.Close();
            if (httpReq != null)
            {
                httpResp.Close();
                httpReq = null;
            }
            //Console.WriteLine(result);
            return result;
        }

        public static string Get(string port, string cookieString, string param)
        {
            HttpWebRequest httpReq = WebRequest.Create(new Uri(port)) as HttpWebRequest;
            httpReq.Proxy = null;
            httpReq.Method = "Get";
            httpReq.Accept = "text/html, application/xhtml+xml, */*";
            httpReq.ContentType = "application/x-www-form-urlencoded";
            httpReq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            httpReq.KeepAlive = true;
            httpReq.AllowAutoRedirect = false;
            httpReq.Credentials = CredentialCache.DefaultCredentials;
            if (!string.IsNullOrEmpty(cookieString))
                httpReq.Headers.Add("Cookie", cookieString);
            if (!string.IsNullOrEmpty(param))
            {
                byte[] paramBytes = Encoding.GetEncoding("GB2312").GetBytes(param);
                httpReq.ContentLength = paramBytes.Length;
                Stream postStream = httpReq.GetRequestStream();
                postStream.Write(paramBytes, 0, paramBytes.Length);
                postStream.Close();
            }
            var httpResp = httpReq.GetResponse() as HttpWebResponse;
            Stream stream = (httpResp).GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            reader.Close();
            stream.Close();
            if (httpReq != null)
            {
                httpResp.Close();
                httpReq = null;
            }
            //Console.WriteLine(result);
            return result;
        }
    }
}
