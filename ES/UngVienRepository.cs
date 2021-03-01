using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UngVienJobModel;

namespace ES
{
    public class UngVienRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static UngVienRepository instance;

        public UngVienRepository(string modify_index)
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

        public static UngVienRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "ha_all_ung_vien";
                    instance = new UngVienRepository(_default_index);
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

        public int IndexMany(IEnumerable<UngVien> data)
        {
            //int count_success = 0;
            //for (int i = 0; i <= data.Count() / spl; i++)
            //{
            //    var tmp = data.Skip(i * spl).Take(spl);
            //    if (tmp.Count() > 0)
            //    {
            //        var bulk = new BulkDescriptor();
            //        foreach (var item in tmp)
            //        {
            //            string id = $"{item.app_id}_{item.domain}_{item.custom_id}";
            //            bulk.Index<UngVien>(b => b.Document(item).Id(id));
            //        }
            //        var re = client.Bulk(bulk);
            //        count_success += tmp.Count() - re.ItemsWithErrors.Count();
            //    }
            //}
            //return count_success;
            return IndexMany<UngVien>(data);
        }
        public bool IndexSingle(object data)
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
        private  UngVien ConvertDoc(IHit<UngVien> hit)
        {
            var uv = hit.Source;
            uv.id = hit.Id;
            return uv;
        }

        public List<UngVien> GetByNguoiTao(string nguoi_tao, long tao_tu, long tao_den, int page, int page_size, string[] fields, out long total_recs)
        {
            List<QueryContainer> must = new List<QueryContainer>();

            if (!string.IsNullOrEmpty(nguoi_tao) && ValidateQuery(nguoi_tao))
            {
                must.Add(new QueryStringQuery() { Fields = new string[] { "nguoi_tao", "ho_ten", "email", "so_dien_thoai" }, Query = nguoi_tao });
            }
            else
            {
                must.Add(new TermQuery()
                {
                    Field = "nguoi_tao",
                    Value = nguoi_tao
                });
            }

            if (tao_den > 0)
            {
                must.Add(new LongRangeQuery()
                {
                    Field = "ngay_tao",
                    LessThanOrEqualTo = tao_den
                });
            }
            if (tao_tu > 0)
            {
                must.Add(new LongRangeQuery()
                {
                    Field = "ngay_tao",
                    GreaterThanOrEqualTo = tao_tu
                });
            }
            QueryContainer queryFilterMultikey = new QueryContainer(new BoolQuery()
            {
                Must = must
            });

            var sort = new List<ISort>() { new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending } };

            SearchRequest request = new SearchRequest(Indices.Index(_default_index))
            {
                Query = queryFilterMultikey,
                Size = page_size,
                From = (page - 1) * page_size,
                Sort = sort
            };
            if (fields.Length > 0)
            {
                SourceFilter soF = new SourceFilter() { Includes = fields };
                request.Source = soF;
            }
            var re = client.Search<UngVien>(request);

            total_recs = re.Total;
            return re.Documents.ToList();
        }

        public List<UngVien> Search(string app_id, string term, IEnumerable<string> lst_account, long tao_tu, long tao_den, string site_name, string vi_tri, string hoc_van, string dia_chi, string gioi_tinh, int page, out long total_recs, int page_size, Dictionary<string,string> sort_order)
        {
            total_recs = 0;
            List<UngVien> lst = new List<UngVien>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id", Value = app_id });
                must.Add(new TermsQuery() { Field = "nguoi_tao", Terms = lst_account });

                if (!string.IsNullOrEmpty(term) && ValidateQuery(term))
                {
                    must.Add(new QueryStringQuery()
                    {
                        Fields = new string[] { "nguoi_tao", "ho_ten", "email", "so_dien_thoai", "lien_Lac" },
                        Query = term
                    });
                }

                if (!string.IsNullOrEmpty(vi_tri))
                {
                    must.Add(new QueryStringQuery()
                    {
                        DefaultField = "vi_tri",
                        Query = vi_tri
                    });
                }

                if (!string.IsNullOrEmpty(hoc_van))
                {
                    must.Add(new QueryStringQuery()
                    {
                        DefaultField = "hoc_van",
                        Query = hoc_van
                    });
                }

                if (!string.IsNullOrEmpty(dia_chi))
                {
                    must.Add(new MatchQuery()
                    {
                        Field = "dia_chi",
                        Query = dia_chi
                    });
                }

                if (!string.IsNullOrEmpty(gioi_tinh))
                {
                    must.Add(new TermQuery()
                    {
                        Field = "gioi_tinh",
                        Value = gioi_tinh
                    });
                }

                if (tao_den > 0)
                {
                    must.Add(new LongRangeQuery()
                    {
                        Field = "ngay_tao",
                        LessThanOrEqualTo = tao_den
                    });
                }
                if (tao_tu > 0)
                {
                    must.Add(new LongRangeQuery()
                    {
                        Field = "ngay_tao",
                        GreaterThanOrEqualTo = tao_tu
                    });
                }
                if (!string.IsNullOrEmpty(site_name))
                {
                    must.Add(new WildcardQuery() { Field = "thong_tin_chung.domain", Value = $"*{site_name}*" });
                }

                List<QueryContainer> must_not = new List<QueryContainer>();
                List<ISort> sort = new List<ISort>();
                if (sort_order == null)
                {
                    sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Ascending });
                    
                }
                else
                {
                    
                    foreach (var item in sort_order)
                    {
                        bool val = item.Value == "0" ? true : false;
                        sort.Add(val
                           ? new FieldSort() {Field = item.Key, Order = SortOrder.Descending}
                            : new FieldSort() {Field = item.Key, Order = SortOrder.Ascending});
                    }
                }
                
                SearchRequest req = new SearchRequest(_default_index)
                {
                    Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not }),
                    From = (page - 1) * page_size,
                    Size = page_size,
                    Sort = sort,
                    TrackTotalHits = true
                };
                var res = client.Search<UngVien>(req);
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

        public UngVien GetUngVienBySdt(string app_id, string sdt)
        {
            var re = client.Search<UngVien>(s => s.Query(q =>
                q.Term(t => t.Field("app_id").Value(app_id))
                && q.Term(t => t.Field("so_dien_thoai").Value(sdt))
            ));
            if (re.Total > 0)
            {
                var ung_vien = re.Hits.Select(ConvertDoc).First();
                return ung_vien;
            }
            return null;
        }

        public bool IsExistUngVien(string app_id, string sdt)
        {
            var ung_vien = GetUngVienBySdt(app_id, sdt);
            return ung_vien != null;
        }
    }
}