using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UngVienJobModel;

namespace TEST.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DangTinController : ControllerBase
    {
        [HttpPost]
        [Route("posttin")]
        public IActionResult Post([FromBody] object value)
        {
            try
            {
                DataResponse res = new DataResponse();
                var tin_tuc = JsonConvert.DeserializeObject<DangTin>(value.ToString());
                if (tin_tuc != null)
                {
                    string json = @"{
  'id_tin': 'abc123124',
  'chuc_danh': 'Lap trinh vien',
  'url_tin_tuc': 'joboko.com',
  'thuoc_tinh': 1,  
  'nguoi_tao': 'tienhv'
}";
                    res.data = JsonConvert.SerializeObject(json);
                    res.success = true;
                    res.msg = "Bạn vui lòng chờ đợi!";
                    return Ok(res);

                    // success = true or false, stt = vua dang tin, success = true thi tra ra url cua tin
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return BadRequest();
        }

        /// <summary>
        /// Hàm Lấy danh sách tin
        /// </summary>
        /// <param name="term">tìm kiếm field</param>
        /// <param name="nguoi_tao">Tìm kiếm bằng người tạo</param>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="page_size">Số bản ghi trên một trang</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getalltin")]
        public IActionResult Search(string term, string nguoi_tao, int page, int page_size)
        {
            DataResponse res = new DataResponse();
            var all_tin = ES.NewRepository.Instance.Search(term, nguoi_tao, page, out long total_recs, page_size).ToList();
            res.data = all_tin;
            res.success = true;
            res.msg = "Lấy danh sách thành công";
            res.total = total_recs;
            return Ok(JsonConvert.SerializeObject(res));
        }

        /// <summary>
        /// Lấy CV ứng viên bằng id tin
        /// </summary>
        /// <param name="id_tin_post"></param>
        /// <param name="page"></param>
        /// <param name="page_size"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcvbyid")]
        public IActionResult GetCvByIdTin(string id, int page, int page_size)
        {
            var list_id_tin = JsonConvert.DeserializeObject<List<string>>(id);
            DataResponse res = new DataResponse();
            string msg = "Lấy Cv";
            int total = 100;
            foreach (var tin in list_id_tin)
            {
                Console.WriteLine(id);
            }
            string json = @"[{
    'app_id': 'xmedia.vn',
    'ho_ten':'viet_tien',
    'ngay_sinh':'1997',
    'dia_chi': 'Ha Noi',
    'vi_tri' : 'Lap trinh vien',
    'email': 'tienhv@xmedia.vn',
    'so_dien_thoai': '0945935480',
    'hoc_van':'dai hoc',
    'link_cv_online': 'https://google.com.vn',
    'nguoi_tao': 'tienhv',
    'ngay_tao': 0
  },
  {
    'app_id': 'xmedia.vn',
    'ho_ten':'viet_tien123123',
    'ngay_sinh':'1995',
    'dia_chi': 'Ha Noi VN',
    'vi_tri' : 'Lap trinh vien java',
    'email': 'tienhv@xmedia.vn',
    'so_dien_thoai': '0945935480',
    'hoc_van':'dai hoc',
    'link_cv_online': 'joboko.com',
    'nguoi_tao': 'tienhv',
    'ngay_tao': 0
  }]";
            // convert sang json khi trả ra
            res.data = json;
            res.msg = msg;
            res.success = true;
            // tong so cv lay duoc
            res.total = total;
            return Ok(JsonConvert.SerializeObject(res));
        }

        /// <summary>
        /// Cập nhật trạng thái của tin
        /// </summary>
        /// <param name="lst_id"></param>
        /// <param name="thuoc_tinh"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatetrangthai")]
        public IActionResult UpdateTrangThaiTinTucById(List<string> id, int thuoc_tinh)
        {
            // update thuoc tinh cua list_id truyen vao, lay nhung id co thuoc tinh bang thuoc tinh truyen vao
            DataResponse res = new DataResponse();
            foreach (var item in id)
            {
                //...
                //...
                //...
            }
            //'trang_thai': 'Mới'
            //'trang_thai': 'Bị từ chối'
            //'trang_thai': 'Đang hoạt động'
            string json = @"[
            {
                'id': 'DJnSCnQBtUkXexiSAZXV',
                'ten_job': 'java',
                'thuoc_tinh':'1',
              },
            {
            'id': 'gPgMtXUBcn0nJN3F34mr',
            'ten_job': 'php',
            'thuoc_tinh':'2',
            },
            {
            'id': '45nECnQBtUkXexiSN5Tn',
            'ten_job': 'net',
            'thuoc_tinh':'3',
            }
            ]";
            res.msg = "";
            res.success = true;
            res.data = JsonConvert.SerializeObject(json);
            return Ok(res);
        }
    }
}