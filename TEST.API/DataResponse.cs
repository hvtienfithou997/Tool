using Newtonsoft.Json;
using System;
using TEST.API.Models;
namespace TEST.API
{
    public class DataResponse
    {
        private object _data;
        public bool success { get; set; }

        public object data
        {
            get { return _data; }
            set
            {
                if (value != null && value.GetType() == typeof(string))
                {
                    try
                    {
                        _data = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(value));
                    }
                    catch (Exception)
                    {
                        _data = value;
                    }
                }
                else
                {
                    var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };

                    var json = JsonConvert.SerializeObject(value, settings);

                    _data = JsonConvert.DeserializeObject<dynamic>(json);
                }
            }
        }
        public string msg { get; set; }
        public long total { get; set; }
    }

    public class NewResponse : DataResponse
    {
        public string id { get; set; }
        public string chuc_danh { get; set; }
        public string trang_thai { get; set; }
        public string url_tin { get; set; }
    }
}