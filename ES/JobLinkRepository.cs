using Nest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UngVienJobModel;

namespace ES
{
    public class JobLinkRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static JobLinkRepository instance;

        public JobLinkRepository(string modify_index)
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

        public static JobLinkRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "ha_job_link";
                    instance = new JobLinkRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(int no_shard, int no_rep = 1, bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(no_rep).NumberOfShards(no_shard)).Map<JobLink>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public async Task<IEnumerable<JobLink>> GetAll(string app_id, string[] fields)
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new TermQuery() { Field = "app_id", Value = app_id });
            must.Add(new MatchAllQuery());
            var hits = await GetScroll<JobLink>(_default_index);

            return hits.Select(x => x.Source);
        }

        public int IndexMany(IEnumerable<JobLink> data)
        {
            return IndexMany<JobLink>(data);
        }

        //public string Index(JobLink data)
        //{
        //    //data.link = "careerbuilder.vn";
        //    //data.trang_thai = TrangThai.DANG_SU_DUNG;
        //    //data.trang_thai_xu_ly = TrangThaiXuLy.DANG_XU_LY;
        //    //data.loai = LoaiLink.CONFIG;
        //    //data.app_id = "xmedia.vn";
        //    //data.json = new CauHinh()
        //    //{
        //    //    url_login = "https://careerbuilder.vn/vi/employers/login",
        //    //    xpath_username = "//form[@name='frmLogin']//input[@name='username']",
        //    //    xpath_password = "//form[@name='frmLogin']//input[@name='password']"
        //    //};

        //    return Index(_default_index, data);
        //}

        public bool Index(JobLink data)
        {
            //int retry = 0; int max_retry = 5;
            var req = new IndexRequest<JobLink>
            {
                Document = data
            };
            var re = client.Index<JobLink>(req);
            if (re.Result == Result.Created)
            {
                return true;
            }
            return false;
        }

        public bool Update(string id, JobLink data)
        {
            return Update<JobLink>(_default_index, data, id);
        }

        public bool Insert()
        {
            return false;
        }

        /// <summary>
        /// Lấy các link đang ở trạng thái chưa xử lý
        /// </summary>
        /// <param name="page_size">Số lượng link lấy ra</param>
        /// <param name="doi_trang_thai_xu_ly">True: Đối trạng thái xử lý của các link thành DANG_XU_LY, False: Không thay đổi trạng thái xử lý</param>
        /// <returns></returns>
        public List<JobLink> GetJobChuaXuLy(int page_size, bool doi_trang_thai_xu_ly = false)
        {
            var lst = new List<JobLink>();
            var re = client.Search<JobLink>(s => s.Query(q => q.Term(t => t.Field("loai").Value(LoaiLink.JOB_LINK)) &&
                q.Term(t => t.Field("trang_thai_xu_ly").Value(TrangThaiXuLy.DANG_XU_LY))).Size(page_size));
            lst = re.Hits.Select(x => ConvertDoc(x)).ToList();
            if (doi_trang_thai_xu_ly)
            {
                var bulk = new BulkDescriptor();
                foreach (var item in lst)
                {
                    bulk.Update<JobLink, object>(u => u.Id(item.id).Doc(new { trang_thai_xu_ly = TrangThaiXuLy.DANG_XU_LY }));
                }
                client.Bulk(bulk);
            }
            return lst;
        }

        public List<JobLink> GetJobChuaXuLy(string domain, int page_size, bool doi_trang_thai_xu_ly = false)
        {
            var lst = new List<JobLink>();
            var re = client.Search<JobLink>(s => s.Query(q => q.Wildcard(t => t.Field("link").Value($"*{domain}*")) && q.Term(t => t.Field("loai").Value(LoaiLink.JOB_LINK))
                && q.Term(t => t.Field("trang_thai_xu_ly").Value(TrangThaiXuLy.DANG_XU_LY))).Size(page_size));

            lst = re.Hits.Select(x => ConvertDoc(x)).ToList();
            if (doi_trang_thai_xu_ly)
            {
                var bulk = new BulkDescriptor();

                foreach (var item in lst)
                {
                    bulk.Update<JobLink, object>(u => u.Id(item.id).Doc(new { trang_thai_xu_ly = TrangThaiXuLy.DA_XU_LY }));
                }

                client.Bulk(bulk);
            }

            return lst;
        }

        public CauHinh GetCauHinh(string domain)
        {
            CauHinh config = new CauHinh();
            try
            {
                var re = client.Search<JobLink>(s => s.Query(q => q.Term(t => t.Field("link").Value(domain)) && q.Term(t => t.Field("loai").Value(LoaiLink.CONFIG))));

                if (re.Total > 0)
                {
                    var jso = (JObject)re.Documents.First().json;
                    if (jso != null)
                    {
                        config = jso.ToObject<CauHinh>();
                        config.password = "";
                        config.username = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return config;
        }

        public JobLink ConvertDoc(IHit<JobLink> hit)
        {
            var jo = hit.Source;
            jo.id = hit.Id;
            return jo;
        }

        public List<JobLink> GetAllCauHinh()
        {
            var re = client.Search<JobLink>(s => s.Query(q => q.Term(t => t.Field(f => f.loai == LoaiLink.CONFIG))).Size(9999));
            return re.Hits.Select(ConvertDoc).ToList();
        }

        private JobLink GetJobLinkByChucDanh(string app_id, string chuc_danh)
        {
            var re = client.Search<JobLink>(s => s.Query(q =>
                q.Term(t => t.Field("app_id").Value(app_id))
                && q.Term(t => t.Field("ten_job.keyword").Value(chuc_danh))
            ));
            if (re.Total > 0)
            {
                var job_link = re.Hits.Select(ConvertDoc).First();
                return job_link;
            }
            return null;
        }
        public bool IsExistJobLinkByChucDanh(string app_id, string chuc_danh)
        {
            var job_link = GetJobLinkByChucDanh(app_id, chuc_danh);
            return job_link != null;
        }

        private JobLink GetJobLinkByLink(string app_id, string link)
        {
            var re = client.Search<JobLink>(s => s.Query(q =>
                q.Term(t => t.Field("app_id").Value(app_id))
                && q.Term(t => t.Field("link").Value(link))
            ));
            if (re.Total > 0)
            {
                var job_link = re.Hits.Select(x => ConvertDoc(x)).First();
                return job_link;
            }
            return null;
        }

        public bool IsExistJobLink(string app_id, string link)
        {
            var job_link = GetJobLinkByLink(app_id, link);
            return job_link != null;
        }

        public List<JobLink> Search(string app_id, string term, IEnumerable<string> lst_account, long tao_tu, long tao_den, long ngay_xu_ly_tu, long ngay_xu_ly_den,
            string trang_thai_xu_ly, string site_name, int page, out long total_recs, int page_size, Dictionary<string, string> sort_order)
        {
            total_recs = 0;
            List<JobLink> lst = new List<JobLink>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();

                must.Add(new TermQuery() { Field = "app_id", Value = app_id });
                must.Add(new TermsQuery() { Field = "nguoi_tao", Terms = lst_account });

                if (!string.IsNullOrEmpty(term) && ValidateQuery(term))
                {
                    must.Add(new QueryStringQuery() { Fields = new string[] { "nguoi_tao", "ten_job", "link" }, Query = term });
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

                if (!string.IsNullOrEmpty(trang_thai_xu_ly))
                {
                    must.Add(new TermQuery()
                    {
                        Field = "trang_thai_xu_ly",
                        Value = trang_thai_xu_ly
                    });
                }

                if (ngay_xu_ly_tu > 0)
                {
                    must.Add(new LongRangeQuery()
                    {
                        Field = "ngay_xu_ly",
                        GreaterThanOrEqualTo = ngay_xu_ly_tu
                    });
                }
                if (ngay_xu_ly_den > 0)
                {
                    must.Add(new LongRangeQuery()
                    {
                        Field = "ngay_xu_ly",
                        LessThanOrEqualTo = ngay_xu_ly_den
                    });
                }

                if (!string.IsNullOrEmpty(site_name))
                {
                    must.Add(new WildcardQuery() { Field = "link", Value = $"*{site_name}*" });
                }

                List<QueryContainer> must_not = new List<QueryContainer>();

                List<ISort> sort = new List<ISort>();
                if (sort_order == null)
                {
                    sort.Add(new FieldSort() { Field = "ngay_xu_ly", Order = SortOrder.Descending });
                    sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                }
                else
                {
                    foreach (var item in sort_order)
                    {
                        bool val = item.Value == "0" ? true : false;
                        sort.Add(val
                            ? new FieldSort() { Field = item.Key, Order = SortOrder.Descending }
                            : new FieldSort() { Field = item.Key, Order = SortOrder.Ascending });
                    }
                }

                must_not.Add(new TermQuery() { Field = "loai", Value = LoaiLink.CONFIG });
                SearchRequest req = new SearchRequest(_default_index)
                {
                    Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not }),
                    From = (page - 1) * page_size,
                    Size = page_size,
                    TrackTotalHits = true,
                    Sort = sort
                };
                var res = client.Search<JobLink>(req);
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
            
        public bool UpdateTrangThaiXuLy(JobLink job)
        {
            var job_link = new JobLink(LoaiLink.JOB_LINK);

            var re_u = client.Search<JobLink>(s => s.Query(q => q.Term(t => t.Field("link").Value(job.link))).Size(1));
            job_link = re_u.Hits.First().Source;
            job_link.id = re_u.Hits.First().Id;
            if (re_u.Total > 0)
            {
                var re = client.Update<JobLink, object>(job_link.id, u => u.Doc(new { trang_thai_xu_ly = job.trang_thai_xu_ly, ngay_xu_ly = job.ngay_xu_ly, tong_so_cv = job.tong_so_cv }));
                return re.Result == Result.Updated || re.Result == Result.Noop;
            }

            return false;
        }
        public bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UpdateThuocTinh<JobLink>(id, thuoc_tinh);
        }
        public bool UpdateThuocTinh<T>(string id, List<int> thuoc_tinh) where T : class
        {
            var re = client.Update<T, object>(id, u => u.Doc(new { thuoc_tinh = thuoc_tinh }));
            return re.Result == Result.Updated || re.Result == Result.Noop;
        }


    }
}