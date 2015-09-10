using System;
using System.Threading;
using System.Web.Script.Serialization;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using PYS.Lib;
using System.Text;
using PYS.Utility;
using System.Net;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using PYS.Entity;
using System.Text.RegularExpressions;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Linq.Expressions;
using Autofac;
using URLINTERFACELib;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class PYSConsole
{
    public static void Main()
    {
        string input = "<a href=\"http://www.target.com/RegistryGiftGiverCmd?isPreview=false&amp;status=completePageLink®istryType=BB&amp;isAjax=false&amp;listId=qr5aAYL6-UxlDyY8DhBlTA&amp;clkid=wT20fuXaEX-dyTA0opSPP2wlUkQ0k6VR2QUVWU0&amp;lnm=Online Tracking Link&amp;afid=The Knot%2C Inc. and Subsidiaries&amp;ref=tgt_adv_xasd0002\">http://www.target.com/RegistryGiftGiverCmd?isPreview=false&amp;status=completePageLink&reg;istryType=BB&amp;isAjax=false&amp;listId=qr5aAYL6-UxlDyY8DhBlTA&amp;clkid=wT20fuXaEX-dyTA0opSPP2wlUkQ0k6VR2QUVWU0&amp;lnm=Online Tracking Link&amp;afid=Thehttp://uw.thebump.com/admin/edit/34393925 Knot%2C Inc. and Subsidiaries&amp;ref=tgt_adv_</a>";
        string str1 = "http://www.target.com/RegistryGiftGiverCmd?isPreview=false&amp;status=completePageLink®istryType=BB&amp;isAjax=false&amp;listId=qr5aAYL6-UxlDyY8DhBlTA&amp;clkid=wT20fuXaEX-dyTA0opSPP2wlUkQ0k6VR2QUVWU0&amp;lnm=Online Tracking Link&amp;afid=The Knot%2C Inc. and Subsidiaries&amp;ref=tgt_adv_xasd0002";

        Regex regex = new Regex(str1.Trim());
        string result = input.Replace(str1, "RRRRR");

        // string result = regex.Replace(input, "RRRRR", 1);


        // TestExcel.Run();
        // string filePath = @"C:\Users\cli\Desktop\assembles.txt";
        // GlobalAssemblyCache.Test(filePath);
        //LandingType L = LandingType.Filter_Price0_49;
        //string a = L.ToDescription();
        //LandingType L2 = LandingType.Manage_URLSettings;
        //string b = L2.ToDescription();
        //Console.ReadKey();
    }




    static void TimeZoneTest()
    {
        //DateTime easternTime = new DateTime(2013, 11, 06, 15, 08, 00);
        ////DateTime easternTime = DateTime.Now;
        ////string easternZoneId = "AUS Central Standard Time";
        //string easternZoneId = "Australia Central Standard Time";
        //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
        //DateTime convertedTime = TimeZoneInfo.ConvertTime(easternTime, easternZone);

        ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
        TimeZone zone = TimeZone.CurrentTimeZone;
        // Demonstrate ToLocalTime and ToUniversalTime.
        DateTime local = zone.ToLocalTime(DateTime.Now);
        DateTime universal = zone.ToUniversalTime(DateTime.Now);
        Console.WriteLine(local);
        Console.WriteLine(universal);
    }

    static string FetchHtml(string url)
    {
        using (WebClient client = new WebClient())
        {
            // client.DownloadFile("http://yoursite.com/page.html", @"C:\localfile.html");
            string htmlCode = client.DownloadString("http://www.bedbathandbeyond.com/regGiftRegistry.asp?wrn=-79396643&=");
            return htmlCode;
        }
    }

}



public static class EnumCommon
{
    public static string ToDescription(this LandingType value)
    {
        var field = typeof(LandingType).GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        if (attribute != null) return attribute.Description;
        else return value.ToString();
    }
}

public enum LandingType : byte
{
    [Description("Filter_Price0-49")]
    Filter_Price0_49,
    Manage_URLSettings,
}

#region Autofac
public interface IOutput
{
    void Write(string content);
}

public class ConsoleOutput : IOutput
{
    public void Write(string content)
    {
        Console.WriteLine(content);
    }
}

public interface IDateWriter
{
    void WriteDate();
}

public class TodayWrite : IDateWriter
{
    private IOutput _output;
    public TodayWrite(IOutput output)
    {
        this._output = output;
    }

    public void WriteDate()
    {
        this._output.Write(DateTime.Today.ToShortDateString());
    }
}
#endregion

#region Thread&Delegate
public class PYSThread
{
    public static void RunThread1()
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("Thread1 {0}", i);
            Thread.Sleep(1);
        }
    }

    public static void RunThread2()
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("Thread2 {0}", i);
            Thread.Sleep(1);
        }
    }

    public static void Test()
    {
        Console.WriteLine("Before start thread");
        Thread tid1 = new Thread(new ThreadStart(PYSThread.RunThread1));
        Thread tid2 = new Thread(new ThreadStart(PYSThread.RunThread2));
        tid1.Start();
        tid2.Start();
    }
}

public class PYSDelegateBasic
{
    public delegate void Del(string message);

    public static void DeleteMethod(string message)
    {
        Console.WriteLine(message);
    }

    public PYSDelegateBasic()
    {
        Del handler = DeleteMethod;
        handler("THIS IS MY NATIVE LAND,I WILL DEFEND IT WITH MY LIFE...");
    }
}

#endregion