using Nest;
using System;
using System.Threading.Tasks;
using UngVienJobModel;

namespace ES
{
    public class ThuocTinhRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static ThuocTinhRepository instance;

        public ThuocTinhRepository(string modify_index)
        {
            _default_index = !string.IsNullOrEmpty(modify_index) ? modify_index : _default_index;
            ConnectionSettings settings = new ConnectionSettings(connectionPool, sourceSerializer: Nest.JsonNetSerializer.JsonNetSerializer.Default).DefaultIndex(_default_index).DisableDirectStreaming(true);
            settings.MaximumRetries(10);
            client = new ElasticClient(settings);
            var ping = client.Ping(p => p.Pretty(true));
            if (ping.ServerError != null && ping.ServerError.Error != null)
            {
                throw new Exception("START ES FIRST");
            }
        }

        public static ThuocTinhRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "ha_thuoc_tinh_link";
                    instance = new ThuocTinhRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(1)).Map<ThuocTinh>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public bool Index(ThuocTinh data)
        {
            //var gia_tri_random = Nanoid.Nanoid.Generate(alphabet: "1234567890", size: 8);

            //var max_gia_tri = Convert.ToInt32(gia_tri_random);
            bool need_retry = true;

            //data.gia_tri = max_gia_tri;
            int retry = 0; int max_retry = 5;

            while (retry++ < max_retry && need_retry)
            {
                need_retry = !Index(_default_index, data, "");
                if (need_retry)
                    Task.Delay(1000).Wait();
            }

            return !need_retry;
        }
    }
}