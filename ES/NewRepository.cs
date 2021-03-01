using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UngVienJobModel;

namespace ES
{
    public class NewRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static NewRepository instance;

        public NewRepository(string modify_index)
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

        public static NewRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "ha_all_tin";
                    instance = new NewRepository(_default_index);
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

        public bool Index(DangTin data, out string id)
        {
            id = "";
            //int retry = 0; int max_retry = 5;
            var req = new IndexRequest<DangTin>
            {
                Document = data
            };
            var re = client.Index(req);
            if (re.Result == Result.Created)
            {
                id = re.Id;

                return true;
            }
            return false;
        }
        public bool Index(DangTin data)
        {

            //int retry = 0; int max_retry = 5;
            var req = new IndexRequest<DangTin>
            {
                Document = data
            };
            var re = client.Index(req);
            if (re.Result == Result.Created)
            {
                //id = re.Id;

                return true;
            }
            return false;
        }
        public bool IndexData(object data)
        {
            int retry = 0; int max_retry = 5;
            bool need_retry = true;
            while (retry++ < max_retry && need_retry)
            {
                need_retry = !Index(_default_index, data, "");
                if (need_retry)
                    Task.Delay(1000).Wait();
            }
            return !need_retry;
        }
        
        //public List<DangTin> Search(string term, IEnumerable<string> lst_account, long tao_tu, long tao_den, string site_name, string vi_tri, string hoc_van, string dia_chi, string gioi_tinh, int page, out long total_recs, int page_size, Dictionary<string, string> sort_order)
        public List<DangTin> Search(string term,  string nguoi_tao, int page, out long total_recs, int page_size)
        {
            total_recs = 0;
            List<DangTin> lst = new List<DangTin>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>
                {
                    //new TermQuery() {Field = "app_id", Value = app_id},
                    new TermQuery() {Field = "nguoi_tao", Value = nguoi_tao}
                };

                if (!string.IsNullOrEmpty(term) && ValidateQuery(term))
                {
                    must.Add(new QueryStringQuery()
                    {
                        Fields = new string[] { "nguoi_tao", "chuc_danh", "link", "so_dien_thoai", "lien_Lac" },
                        Query = term
                    });
                }

                //if (!string.IsNullOrEmpty(filter))
                //{
                //    must.Add(new TermQuery()
                //    {
                //        Field = "thuoc_tinh",
                //        Value = filter
                //    });
                //}

                //if (!string.IsNullOrEmpty(vi_tri))
                //{
                //    must.Add(new QueryStringQuery()
                //    {
                //        DefaultField = "vi_tri",
                //        Query = vi_tri
                //    });
                //}

                //if (!string.IsNullOrEmpty(hoc_van))
                //{
                //    must.Add(new QueryStringQuery()
                //    {
                //        DefaultField = "hoc_van",
                //        Query = hoc_van
                //    });
                //}

                //if (!string.IsNullOrEmpty(dia_chi))
                //{
                //    must.Add(new MatchQuery()
                //    {
                //        Field = "dia_chi",
                //        Query = dia_chi
                //    });
                //}

                //if (!string.IsNullOrEmpty(gioi_tinh))
                //{
                //    must.Add(new TermQuery()
                //    {
                //        Field = "gioi_tinh",
                //        Value = gioi_tinh
                //    });
                //}

                //if (tao_den > 0)
                //{
                //    must.Add(new LongRangeQuery()
                //    {
                //        Field = "ngay_tao",
                //        LessThanOrEqualTo = tao_den
                //    });
                //}
                //if (tao_tu > 0)
                //{
                //    must.Add(new LongRangeQuery()
                //    {
                //        Field = "ngay_tao",
                //        GreaterThanOrEqualTo = tao_tu
                //    });
                //}
                //if (!string.IsNullOrEmpty(site_name))
                //{
                //    must.Add(new WildcardQuery() { Field = "thong_tin_chung.domain", Value = $"*{site_name}*" });
                //}

                List<QueryContainer> must_not = new List<QueryContainer>();
                List<ISort> sort = new List<ISort>();
                //if (sort_order == null)
                //{
                //    sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Ascending });
                //}
                //else
                //{
                //    foreach (var item in sort_order)
                //    {
                //        bool val = item.Value == "0" ? true : false;
                //        sort.Add(val
                //           ? new FieldSort() { Field = item.Key, Order = SortOrder.Descending }
                //            : new FieldSort() { Field = item.Key, Order = SortOrder.Ascending });
                //    }
                //}

                SearchRequest req = new SearchRequest(_default_index)
                {
                    Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not }),
                    From = (page - 1) * page_size,
                    Size = page_size,
                    Sort = sort,
                    TrackTotalHits = true
                };
                var res = client.Search<DangTin>(req);
                if (res.IsValid)
                {
                    total_recs = res.Total;
                    lst = res.Hits.Select(ConvertDoc).ToList();
                }
            }
            catch
            {
                //msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }

        public DangTin ConvertDoc(IHit<DangTin> hit)
        {
            var tin = hit.Source;
            tin.id = hit.Id;
            return tin;
        }

        public DangTin GetById(string id)
        {
            var obj = GetById<DangTin>(_default_index, id);
            if (obj != null)
            {
                obj.id = id;
                return obj;
            }
            return null;
        }
        public bool UpdateThuocTinh(string id, int thuoc_tinh)
        {
            return UpdateThuocTinh<DangTin>(id, thuoc_tinh);
        }
        public bool UpdateThuocTinh<T>(string id, int thuoc_tinh) where T : class
        {
            var re = client.Update<T, object>(id, u => u.Doc(new { thuoc_tinh = thuoc_tinh }));
            return re.Result == Result.Updated || re.Result == Result.Noop;
        }


        //public DangTin GetCvByIdTin(string id)
        //{
        //    var re = client.Search<DangTin>(s => s.Query(q =>
        //         q.Term(t => t.Field("id.keyword").Value(id))
        //    ));
        //    if (re.Total > 0)
        //    {
        //        return re.Hits.Select(ConvertDoc).First();
        //    }
        //    return null;
        //}
    }
}