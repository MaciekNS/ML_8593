using System;
using System.IO;
using System.Net;
using System.Xml;

namespace WCF_CLIENT
{
    class Program
    {
        public static void CallWebService()
        {
            var _url = "https://pwirit.azurewebsites.net/PiLeibniz.svc";
            var _action = "http://tempuri.org/IPiLeibniz/CalculatePi";

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            asyncResult.AsyncWaitHandle.WaitOne();

            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(soapResult);
                XmlNodeList node = doc.GetElementsByTagName("CalculatePiResult");
                Console.WriteLine("Wynik: " + node[0].InnerText);
            }
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope()
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(
            @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
              <s:Body>
                <CalculatePi xmlns=""http://tempuri.org/"">
                    <accuracy>100000</accuracy>
                </CalculatePi>
               </s:Body>
              </s:Envelope>");
            return soapEnvelopeDocument;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Wywołuje");
            CallWebService();
            Console.WriteLine("Koniec");
        }
    }
}
