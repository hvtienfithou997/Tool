using System.Collections.Generic;

namespace UngVienJobUI.Utils
{
    public interface ImportJd
    {
        void SetMapper(Dictionary<string, string> value);

        string MappingInput(Dictionary<string, string> keyMapper, string json_data);

        bool Import(string JsonData, string Url, string User, string Pass);
    }

    public class Jobstreet : ImportJd
    {
        public string MappingInput(Dictionary<string, string> keyMapper, string json_data)
        {
            return string.Empty;
        }

        public void SetMapper(Dictionary<string, string> keyMapper)
        {

        }

        public bool Import(string json_data, string url, string user, string pass)
        {
            return true;
        }
    }

    public class MyWork : ImportJd
    {
        public string MappingInput(Dictionary<string, string> keyMapper, string json_data)
        {
            return string.Empty;
        }

        public void SetMapper(Dictionary<string, string> keyMapper)
        {
        }

        public bool Import(string json_data, string url, string user, string pass)
        {
            return true;
        }
    }
}