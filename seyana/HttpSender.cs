using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace seyana
{
    static class HttpSender
    {
        private static Encoding encode = Encoding.GetEncoding("utf-8");
        public static string pastUrl = "";

        static HttpSender()
        {
            HtmlNode.ElementsFlags.Remove("form");
        }

        private static Stream httpGet_stream(string url, CookieContainer cc, string host, string referer, bool con)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = cc;
                req.UserAgent = "Mozilla/5.0 (iPad; CPU OS 5_1_1 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9B206 Safari/7534.48.3";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                if(host != "") req.Host = host;
                if(referer != "") req.Referer = referer;
                req.KeepAlive = con;

                WebResponse res = req.GetResponse();
                pastUrl = ((HttpWebResponse)res).ResponseUri.ToString();

                return res.GetResponseStream();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }
        private static Stream httpGet_stream(string url, CookieContainer cc)
        {
            return httpGet_stream(url, cc, "", "", false);
        }

        public static string httpGet_str(string url, CookieContainer cc)
        {
            Stream resStream = httpGet_stream(url, cc);
            if (resStream == null) return "";
            StreamReader sr = new StreamReader(resStream, encode);
            string str = sr.ReadToEnd();
            sr.Close();
            resStream.Close();
            return str;
        }
        public static string httpGet_str(string url, CookieContainer cc, string host, string referer, bool con)
        {
            Stream resStream = httpGet_stream(url, cc, host, referer, con);
            if (resStream == null) return "";
            StreamReader sr = new StreamReader(resStream, encode);
            string str = sr.ReadToEnd();
            sr.Close();
            resStream.Close();
            return str;
        }

        public static HtmlDocument httpGet_doc(string url, CookieContainer cc)
        {
            var ret = new HtmlDocument();
            var stream = httpGet_stream(url, cc);
            if (stream == null) return null;
            ret.Load(stream);
            stream.Close();
            return ret;
        }
        public static HtmlDocument httpGet_doc(string url, CookieContainer cc, string host, string referer, bool con)
        {
            var ret = new HtmlDocument();
            var stream = httpGet_stream(url, cc, host, referer, con);
            if (stream == null) return null;
            ret.Load(stream);
            stream.Close();
            return ret;
        }

        private static Stream httpPost_stream(string url, Hashtable val, CookieContainer cc, string host, string referer, bool con)
        {
            try
            {
                string palam = "";
                foreach (object key in val.Keys)
                {
                    palam += string.Format("{0}={1}&", HttpUtility.UrlEncode((string)key, encode), HttpUtility.UrlEncode((string)val[key], encode));
                }
                byte[] data = new byte[0];
                if (palam.Length > 0) data = Encoding.ASCII.GetBytes(palam.Substring(0, palam.Length - 1));

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.CookieContainer = cc;
                req.UserAgent = "Mozilla/5.0 (iPad; CPU OS 5_1_1 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9B206 Safari/7534.48.3";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = data.Length;
                if(host != "") req.Host = host;
                if (referer != "") req.Referer = referer;
                req.KeepAlive = con;

                Stream reqStream = req.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                WebResponse res = req.GetResponse();
                pastUrl = ((HttpWebResponse)res).ResponseUri.ToString();

                return res.GetResponseStream();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }
        private static Stream httpPost_stream(string url, Hashtable val, CookieContainer cc)
        {
            return httpPost_stream(url, val, cc, "", "", false);
        }

        public static string httpPost_str(string url, Hashtable val, CookieContainer cc)
        {
            Stream resStream = httpPost_stream(url, val, cc);
            if (resStream == null) return "";
            StreamReader sr = new StreamReader(resStream, encode);
            string str = sr.ReadToEnd();
            sr.Close();
            resStream.Close();
            return str;
        }
        public static string httpPost_str(string url, Hashtable val, CookieContainer cc, string host, string referer, bool con)
        {
            Stream resStream = httpPost_stream(url, val, cc, host, referer, con);
            if (resStream == null) return "";
            StreamReader sr = new StreamReader(resStream, encode);
            string str = sr.ReadToEnd();
            sr.Close();
            resStream.Close();
            return str;
        }

        public static HtmlDocument httpPost_doc(string url, Hashtable val, CookieContainer cc)
        {
            var ret = new HtmlDocument();
            var stream = httpPost_stream(url, val, cc);
            if (stream == null) return null;
            ret.Load(stream);
            stream.Close();
            return ret;
        }
        public static HtmlDocument httpPost_doc(string url, Hashtable val, CookieContainer cc, string host, string referer, bool con)
        {
            var ret = new HtmlDocument();
            var stream = httpPost_stream(url, val, cc, host, referer, con);
            if (stream == null) return null;
            ret.Load(stream);
            stream.Close();
            return ret;
        }
    }
}
