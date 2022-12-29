using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Demo.Avalonia
{
    internal class BindingTestClass
    {
        public class InnerObject
        {
            public string? Name;
            public int Value;
        }

        public static DateTime GetDate()
        {
            return DateTime.Now;
        }

        public static string GetString()
        {
            return "Hello World!";
        }

        public static int GetInt()
        {
            return 10;
        }

        public static double GetDouble()
        {
            return 10.45;
        }

        public static bool GetBool()
        {
            return true;
        }

        public static string[] GetList()
        {
            return new[] { "item 1", "item 2", "item 3" };
        }

        public static IDictionary<string, object> GetDictionary()
        {
            return new Dictionary<string, object>
            {
                { "Name", "This is a dictionary" },
                { "Value", 10.5 }
            };
        }

        public static object GetObject()
        {
            return new InnerObject { Name = "This is an object", Value = 5 };
        }

        public static object GetObjectWithParams()
        {
            return new InnerObject { Name = "This is an object", Value = 5 };
        }

        public async Task<bool> AsyncGetObjectWithParams(string aStringParam)
        {
            Console.WriteLine(DateTime.Now + ": Called " + nameof(AsyncGetObjectWithParams));
            await Task.Delay(5000).ConfigureAwait(false);
            Console.WriteLine(DateTime.Now + ":  Continuing " + nameof(AsyncGetObjectWithParams));
            return true;
        }

        public static string[] GetObjectWithParamArray(params string[] paramWithParamArray)
        {
            return paramWithParamArray;
        }
    }
}