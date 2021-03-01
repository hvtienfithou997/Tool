using System;
using System.Collections.Generic;
using System.Text;

namespace UngVienJobModel
{
    public class QuyTrinh
    {
        public QuyTrinh()
        {
            quy_trinh = new List<QuyTrinh>();
        }
        public int thu_tu { get; set; }
        public HanhDong hanh_dong { get; set; }
        public string ten { get; set; }
        public string xpath { get; set; }
        public int time_out { get; set; }
        public List<QuyTrinh> quy_trinh { get; set; }
    }
}
