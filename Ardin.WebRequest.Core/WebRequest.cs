using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ardin
{
    public class WebRequest
    {
        public Response MakeRequest(Request request)
        {
            HttpWebResponse httpWebResponse;
            Response response = new Response();

            if (Request(out httpWebResponse, out response, request))
            {
                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {
                    Stream streamToRead = responseStream;
                    if (httpWebResponse.ContentEncoding != null)
                    {
                        if (httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
                        {
                            streamToRead = new GZipStream(streamToRead, CompressionMode.Decompress);
                        }
                        else if (httpWebResponse.ContentEncoding.ToLower().Contains("deflate"))
                        {
                            streamToRead = new DeflateStream(streamToRead, CompressionMode.Decompress);
                        }
                    }

                    using (StreamReader streamReader = new StreamReader(streamToRead, Encoding.UTF8))
                    {
                        response.Body = streamReader.ReadToEnd();
                        return response;
                    }
                }
            }

            return response;
        }
        private bool Request(out HttpWebResponse httpWebResponse, out Response response, Request request)
        {
            httpWebResponse = null;
            response = new Response();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

                HttpWebRequest httpWebRequest = (HttpWebRequest)System.Net.WebRequest.Create(request.Url);

                if (request.Header != null)
                    foreach (string headerKey in request.Header.AllKeys)
                    {
                        KeyValuePair<string, string> header = new KeyValuePair<string, string>(headerKey, request.Header[headerKey]);
                        switch (header.Key)
                        {
                            case "User-Agent":
                                httpWebRequest.UserAgent = header.Value;
                                break;
                            case "Accept":
                                httpWebRequest.Accept = header.Value;
                                break;
                            case "Referer":
                                httpWebRequest.Referer = header.Value;
                                break;
                            case "Content-Type":
                                httpWebRequest.ContentType = header.Value;
                                break;
                            case "KeepAlive":
                                httpWebRequest.KeepAlive = header.Value == "true";
                                break;
                            default:
                                httpWebRequest.Headers.Set(header.Key, header.Value);
                                break;
                        }
                    }

                httpWebRequest.Timeout = (int)request.Timeout.TotalMilliseconds;
                httpWebRequest.ReadWriteTimeout = (int)request.Timeout.TotalMilliseconds;
                httpWebRequest.ContinueTimeout = (int)request.Timeout.TotalMilliseconds;

                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;

                if (request.Cookie != null && request.Cookie.Count > 0)
                {
                    if (httpWebRequest.CookieContainer == null)
                    {
                        httpWebRequest.CookieContainer = new CookieContainer();
                    }
                    foreach (Cookie cookieItem in request.Cookie)
                    {
                        httpWebRequest.CookieContainer.Add(cookieItem);
                    }
                }
                else
                    httpWebRequest.CookieContainer = new CookieContainer();

                httpWebRequest.Method = request.Method;
                httpWebRequest.ServicePoint.Expect100Continue = false;

                if (request.Proxy != null)
                {
                    httpWebRequest.Proxy = request.Proxy;
                }

                if (!string.IsNullOrEmpty(request.Body))
                {
                    byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(request.Body);
                    httpWebRequest.ContentLength = postBytes.Length;
                    Stream stream = httpWebRequest.GetRequestStream();
                    stream.Write(postBytes, 0, postBytes.Length);
                    stream.Close();
                }

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException e)
            {
                while (e.InnerException != null)
                {
                    e = (WebException)e.InnerException;
                }
                response.ExtraMessage = e.Message + " | " + e.StackTrace;

                if (e.Status == WebExceptionStatus.ProtocolError) httpWebResponse = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = (WebException)ex.InnerException;
                }
                response.ExtraMessage = ex.Message + " | " + ex.StackTrace;

                if (httpWebResponse != null) httpWebResponse.Close();
                return false;
            }

            response.Cookie = httpWebResponse.Cookies;
            response.Code = httpWebResponse.StatusCode;
            response.Header = httpWebResponse.Headers;

            return true;
        }
    }
}
