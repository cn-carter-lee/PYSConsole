using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PYS.Entity;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Web;

public class TargetEncryptedSession
{
    public string GetTargetEncryptedCodeWithEmail(string email)
    {
        TargetEncryptedSessionInfo targetEncryptedSessionInfo = GetTargetSession(email);
        return targetEncryptedSessionInfo.listId;
    }

    private TargetEncryptedSessionInfo GetTargetSession(string email)
    {
        TargetEncryptedSessionInfo targetEncryptedSessionInfo = new TargetEncryptedSessionInfo();
        WebRequest request = WebRequest.Create("http://www.target.com/GiftRegistrySearchViewCmd");
        request.Method = "POST";
        string postData = string.Format("registryType=WD&searchType=emailSearch&registryEmail={0}&search-registry-main=1&jsRequest=true", HttpUtility.UrlEncode(email));
        byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteArray.Length;
        Stream dataStream = request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();
        WebResponse response = request.GetResponse();
        dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        string responseFromServer = reader.ReadToEnd();
        reader.Close();
        dataStream.Close();
        response.Close();

        targetEncryptedSessionInfo = JsonConvert.DeserializeObject(responseFromServer, typeof(TargetEncryptedSessionInfo)) as TargetEncryptedSessionInfo;
        if (!String.IsNullOrEmpty(targetEncryptedSessionInfo.listId))
        {
            Console.WriteLine(string.Format("Code1 :{0}", targetEncryptedSessionInfo.listId));
        }
        else
        {
            string cookieValue = "";
            string[] cookieArray = response.Headers["Set-Cookie"].Split(new char[] { ';', ',' });
            foreach (string str in cookieArray)
            {
                if (str.ToLower().Contains("=") && !str.ToLower().Contains("domain") && !str.ToLower().Contains("expires") && !str.ToLower().Contains("path"))
                    cookieValue += str + ";";
            }
            targetEncryptedSessionInfo = GetTargetSessionWithCookies(targetEncryptedSessionInfo.requestId, targetEncryptedSessionInfo.registryEmail, targetEncryptedSessionInfo.grSessionId, cookieValue);
        }
        return targetEncryptedSessionInfo;
    }

    private TargetEncryptedSessionInfo GetTargetSessionWithCookies(string requestId, string registryEmail, string grSessionId, string cookiesValue)
    {
        WebRequest request = WebRequest.Create("http://www.target.com/GiftRegistrySearchViewCmd");
        request.Method = "POST";
        string postData = string.Format("registryType=WD&requestId={0}&cookie=JSESSIONID&jsRequest=true&registryEmail={1}&catalogId=10051&grSessionId={2}&status=inprogress&cumulativeTime=-1&noOfPings=1&langId=-1&searchType=emailSearch&segmentGrpName=J&storeId=10151", requestId, registryEmail, grSessionId);
        byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteArray.Length;
        request.Headers.Add("Cookie", cookiesValue);
        Stream dataStream = request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();
        WebResponse response = request.GetResponse();
        dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        string responseFromServer = reader.ReadToEnd();
        reader.Close();
        dataStream.Close();
        response.Close();
        TargetEncryptedSessionInfo targetSessionInfo = JsonConvert.DeserializeObject(responseFromServer, typeof(TargetEncryptedSessionInfo)) as TargetEncryptedSessionInfo;
        if (!String.IsNullOrEmpty(targetSessionInfo.listId))
            Console.WriteLine(string.Format("Code2 :{0}", targetSessionInfo.listId));
        return targetSessionInfo;
    }

    public static void Test()
    {
        string email = "firbabe83@gmail.com";
        TargetEncryptedSession targetEncryptedSession = new TargetEncryptedSession();
        targetEncryptedSession.GetTargetEncryptedCodeWithEmail(email);
    }
}

