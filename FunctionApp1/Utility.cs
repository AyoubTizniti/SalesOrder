using System;
using Newtonsoft.Json;

namespace FunctionApp1
{
    internal class Utility
    {
        // ConvertJsonToObject
        public static T ConvertJsonToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
