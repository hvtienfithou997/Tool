using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UngVienJobModel;

namespace ES
{
    public class LinkSavedRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static LinkSavedRepository instance;

        public LinkSavedRepository(string modify_index)
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

        public static LinkSavedRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "ha_save_link_post";
                    instance = new LinkSavedRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(int no_shard, int no_rep = 1, bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(no_rep).NumberOfShards(no_shard)).Map<UngVien>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public async Task<IEnumerable<UngVien>> GetAll(string app_id, string[] fields)
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
            must.Add(new MatchAllQuery());
            var hits = await GetScroll<UngVien>(_default_index);

            return hits.Select(x => x.Source);
        }

        public int IndexMany(IEnumerable<LinkSaved> data)
        {
            int count_success = 0;
            for (int i = 0; i <= data.Count() / spl; i++)
            {
                var tmp = data.Skip(i * spl).Take(spl);
                if (tmp.Count() > 0)
                {
                    var bulk = new BulkDescriptor();
                    foreach (var item in tmp)
                    {
                        //string id = $"{item.app_id}_{item.domain}_{item.id}";
                        //string id = 123123;
                        bulk.Index<LinkSaved>(b => b.Id(item.id).Document(item));
                    }
                    var re = client.Bulk(bulk);
                    count_success += tmp.Count() - re.ItemsWithErrors.Count();
                }
            }
            return count_success;
        }
    
        private LinkSaved ConvertDoc(IHit<LinkSaved> hit)
        {
            LinkSaved u = new LinkSaved();

            try
            {
                u = hit.Source;
                u.id = hit.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return u;
        }

        public List<LinkSaved> GetLinksByGiaTri(int gia_tri, string web)
        {
            var lst = new List<LinkSaved>();
            var re = client.Search<LinkSaved>(s => s.Source().Size(1000).Query(q =>
                q.Term(t => t.Field("thuoc_tinh").Value(gia_tri)) &&
                q.Term(o => o.Field("website.keyword").Value(web))));
            if (re.Total > 0)
            {
                lst = re.Hits.Select(x => ConvertDoc(x)).ToList();
            }
            return lst;
        }

        public bool UpdateStatus(string id, int thuoc_tinh)
        {
            //string msg = "";
            var link = new LinkSaved();
            var re_u = client.Get<LinkSaved>(id);
            if (re_u.Found)
            {
                link.id = id;
                var re = client.Update<LinkSaved, object>(link.id, u => u.Doc(new { thuoc_tinh = thuoc_tinh }));
                return re.Result == Result.Updated || re.Result == Result.Noop;
            }

            return false;
        }
    }
}