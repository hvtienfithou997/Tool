using System.Collections.Generic;
using System.Linq;
using UngVienJobModel;

namespace UngVienJobUI.Utils
{
    public static class Config
    {
        public static readonly string mo_ta_start = "[MOTA]";
        public static readonly string mo_ta_end = "[/MOTA]";

        public static readonly string chuc_danh_start = "[CHUCDANH]";
        public static readonly string chuc_danh_end = "[/CHUCDANH]";

        public static readonly string yeu_cau_start = "[YEUCAU]";
        public static readonly string yeu_cau_end = "[/YEUCAU]";

        public static readonly string thoi_gian_start = "[THOIGIAN]";
        public static readonly string thoi_gian_end = "[/THOIGIAN]";

        public static readonly string luong_start = "[LUONG]";
        public static readonly string luong_end = "[/LUONG]";

        public static readonly string quyen_loi_start = "[QUYENLOI]";
        public static readonly string quyen_loi_end = "[/QUYENLOI]";

        public static readonly string lien_he_start = "[LIENHE]";
        public static readonly string lien_he_end = "[/LIENHE]";

        public static readonly string dia_chi_lam_viec_start = "[DIACHILAMVIEC]";
        public static readonly string dia_chi_lam_viec_end = "[/DIACHILAMVIEC]";

        public static List<string> all_tinh_thanh = new List<string>()
        {
            "Đà Nẵng", "Hồ Chí Minh", "Hà Nội", "Phú Yên", "Yên Bái", "Vĩnh Phúc", "Vĩnh Long", "Tuyên Quang",
            "Trà Vinh",
            "Tiền Giang", "Thừa Thiên Huế", "Thanh Hóa", "Thái Nguyên", "Thái Bình", "Tây Ninh", "Sơn La", "Sóc Trăng",
            "Quảng Trị", "Quảng Ninh", "Quảng Ngãi",
            "Quảng Nam", "Quảng Bình", "Phú Thọ", "Ninh Thuận", "Ninh Bình", "Nghệ An", "Nam Định", "Long An",
            "Lào Cai", "Lạng Sơn", "Lâm Đồng", "Lai Châu", "Kon Tum",
            "Kiên Giang", "Khánh Hòa", "Hưng Yên", "Hòa Bình", "Hậu Giang", "Hải Dương", "Hà Tĩnh", "Hà Nam",
            "Hà Giang", "Gia Lai", "Đồng Tháp", "Đồng Nai",
            "Điện Biên", "Đắk Nông", "Đắk Lắk", "Cao Bằng", "Cà Mau", "Bình Thuận", "Bình Phước", "Bình Dương",
            "Bình Định", "Bến Tre", "Bắc Ninh", "Bạc Liêu",
            "Bắc Kạn", "Bắc Giang", "Bà Rịa - Vũng Tàu", "An Giang", "Hải Phòng", "Cần Thơ"
        };

        public static List<string> list_nganh_nghe_my_work = new List<string>()
        {
            "Bán hàng", "Biên tập/ Báo chí/ Truyền hình", "Bảo hiểm/ Tư vấn bảo hiểm", "Bảo vệ/ An ninh/ Vệ sỹ",
            "Phiên dịch/ Ngoại ngữ", "Bưu chính", "Chứng khoán - Vàng", "Cơ khí - Chế tạo", "Công chức - Viên chức",
            "Công nghệ cao", "Công nghiệp", "Dầu khí - Hóa chất",
            "Dệt may - Da giày", "Dịch vụ", "Du lịch", "Đầu tư", "Điện - Điện tử - Điện lạnh", "Điện tử viễn thông",
            "Freelance", "Games", "Giáo dục - Đào tạo", "Giao nhận/ Vận chuyển/ Kho bãi",
            "Hàng gia dụng", "Hàng hải", "Hàng không", "Hành chính - Văn phòng", "Hóa học - Sinh học",
            "Hoạch định - Dự án", "IT phần cứng/mạng", "IT phần mềm", "In ấn - Xuất bản",
            "Kế toán - Kiểm toán", "Khách sạn - Nhà hàng", "Kiến trúc - Thiết kế nội thất", "Bất động sản", "Kỹ thuật",
            "Kỹ thuật ứng dụng", "Làm bán thời gian", "Làm đẹp/ Thể lực/ Spa",
            "Lao động phổ thông", "Lương cao", "Marketing - PR", "Môi trường", "Mỹ phẩm - Trang sức",
            "Phi chính phủ/ Phi lợi nhuận", "Ngân hàng/ Tài Chính", "Ngành nghề khác",
            "Nghệ thuật - Điện ảnh", "Người giúp việc/ Phục vụ/ Tạp vụ", "Nhân sự", "Nhân viên kinh doanh",
            "Nông - Lâm - Ngư nghiệp", "Ô tô - Xe máy", "Pháp luật/ Pháp lý",
            "Phát triển thị trường", "Promotion Girl/ Boy (PG-PB)", "QA-QC/ Thẩm định/ Giám định", "Quan hệ đối ngoại",
            "Quản trị kinh doanh", "Sinh viên làm thêm", "Startup",
            "Thể dục/ Thể thao", "Thiết kế - Mỹ thuật", "Thiết kế đồ họa - Web", "Thời trang", "Thủ công mỹ nghệ",
            "Thư ký - Trợ lý", "Thực phẩm - Đồ uống", "Thực tập", "Thương mại điện tử",
            "Tiếp thị - Quảng cáo", "Tổ chức sự kiện - Quà tặng", "Tư vấn/ Chăm sóc khách hàng",
            "Vận tải - Lái xe/ Tài xế", "Nhân viên trông quán internet", "Vật tư/Thiết bị/Mua hàng",
            "Việc làm cấp cao", "Việc làm Tết", "Xây dựng", "Xuất - Nhập khẩu", "Xuất khẩu lao động", "Y tế - Dược"
        };

        public static List<string> list_nganh_nghe_top_cv = new List<string>()
        {
            "Kinh doanh / Bán hàng", "Biên / Phiên dịch", "Báo chí / Truyền hình", "Bưu chính - Viễn thông",
            "Bảo hiểm", "Bất động sản", "Chứng khoán / Vàng / Ngoại tệ", "Công nghệ cao",
            "Cơ khí / Chế tạo / Tự động hóa", "Du lịch", "Dầu khí/Hóa chất", "Dệt may / Da giày",
            "Dịch vụ khách hàng", "Điện tử viễn thông", "Điện / Điện tử / Điện lạnh", "Giáo dục / Đào tạo",
            "Hoá học / Sinh học", "Hoạch định/Dự án", "Hàng gia dụng", "Hàng hải",
            "Hàng không", "Hành chính / Văn phòng", "In ấn / Xuất bản", "IT Phần cứng / Mạng", "IT phần mềm",
            "Khách sạn / Nhà hàng", "Kế toán / Kiểm toán", "Marketing / Truyền thông / Quảng cáo",
            "Môi trường / Xử lý chất thải", "Mỹ phẩm / Trang sức", "Mỹ thuật / Nghệ thuật / Điện ảnh",
            "Ngân hàng / Tài chính", "Nhân sự", "Nông / Lâm / Ngư nghiệp", "Luật/Pháp lý",
            "Quản lý chất lượng (QA/QC)", "Quản lý điều hành", "Thiết kế đồ họa", "Thời trang", "Thực phẩm / Đồ uống",
            "Tư vấn", "Tổ chức sự kiện / Quà tặng", "Vận tải / Kho vận", "Logistics",
            "Xuất nhập khẩu", "Xây dựng", "Y tế / Dược", "Công nghệ Ô tô", "An toàn lao động", "Bán hàng kỹ thuật",
            "Bán lẻ / bán sỉ", "Bảo trì / Sữa chữa", "Dược phẩm / Công nghệ sinh học",
            "Địa chất / Khoáng sản", "Hàng cao cấp", "Hàng tiêu dùng", "Kiến trúc", "Phi chính phủ / Phi lợi nhuận",
            "Sản phẩm công nghiệp", "Sản xuất", "Tài chính / Đầu tư", "Ngành nghề khác"
        };

        public static List<string> list_nganh_nghe_jobstreet = new List<string>()
        {
            "Nhà hàng, Khách sạn & Du lịch", "Bán lẻ & Bán sỉ", "Truyền thông, Quảng cáo & Dịch vụ Trực tuyến",
            "Vận tải và Hậu cần",
            "Sản xuất", "Xây dựng, Thiết kế & Kỹ thuật", "Công nghệ thông tin & Viễn thông", "Tài chính và Bảo hiểm",
            "Bất động sản", "Luật, Kiểm toán & Kế Toán", "Y tế và Phúc lợi xã hội", "Giáo dục, Đào tạo & Nghiên cứu",
            "Dịch vụ cá nhân", "Tuyển dụng và Tư vấn nhân sự", "Công nghiệp, Nông nghiệp & Dầu khí",
            "Dịch vụ điện, Khí đốt, Nước và Chất thải", "Chính phủ, Quốc phòng và An ninh", "Ngành khác"
        };

        public static List<string> list_nganh_nghe_careerlink = new List<string>()
        {
            "An Ninh / Bảo Vệ", "An Toàn Lao Động", "Bán hàng", "Bán lẻ / Bán sỉ", "Báo chí / Biên tập viên / Xuất bản",
            "Bảo hiểm",
            "Bảo trì / Sửa chữa", "Bất động sản", "Biên phiên dịch / Thông dịch viên", "Biên phiên dịch (tiếng Nhật)",
            "Chăm sóc sức khỏe / Y tế", "CNTT - Phần cứng / Mạng", "CNTT - Phần mềm", "Dầu khí / Khoáng sản",
            "Dệt may / Da giày", "Dịch vụ khách hàng", "Điện lạnh / Nhiệt lạnh", "Du lịch", "Dược / Sinh học",
            "Điện / Điện tử", "Đồ Gỗ", "Giáo dục / Đào tạo / Thư viện", "Hàng gia dụng",
            "Hóa chất / Sinh hóa / Thực phẩm",
            "Kế toán / Kiểm toán", "Khách sạn", "Kiến trúc", "Kỹ thuật ứng dụng / Cơ khí", "Lao động phổ thông",
            "Môi trường / Xử lý chất thải", "Mới tốt nghiệp / Thực tập", "Ngân hàng / Chứng khoán",
            "Nghệ thuật / Thiết kế / Giải trí", "Người nước ngoài", "Nhà hàng / Dịch vụ ăn uống", "Nhân sự",
            "Nội thất / Ngoại thất", "Nông nghiệp / Lâm nghiệp", "Ô tô", "Pháp lý / Luật",
            "Phi chính phủ / Phi lợi nhuận",
            "Quản lý chất lượng (QA / QC)", "Quản lý điều hành", "Quảng cáo / Khuyến mãi / Đối ngoại",
            "Sản xuất / Vận hành sản xuất", "Tài chính / Đầu tư", "Thời trang", "Thuỷ Hải Sản", "Thư ký / Hành chánh",
            "Tiếp thị",
            "Tư vấn", "Vận chuyển / Giao thông / Kho bãi", "Vật tư / Mua hàng", "Viễn Thông", "Xây dựng",
            "Xuất nhập khẩu / Ngoại thương", "Khác"
        };

        public static List<string> list_nganh_nghe_jobsgo = new List<string>
        {
            "--- Bán Lẻ/Cửa Hàng (Kinh Doanh/Bán Hàng)", "--- Tư Vấn Doanh Nghiệp (B2B) (Kinh Doanh/Bán Hàng)",
            "--- Tư Vấn Bán Hàng (Kinh Doanh/Bán Hàng)", "--- Bán Hàng Qua Điện Thoại (Telesale) (Kinh Doanh/Bán Hàng)",
            "--- Đào Tạo Bán Hàng (Kinh Doanh/Bán Hàng)", "--- Nghiên Cứu Thị Trường (Kinh Doanh/Bán Hàng)",
            "--- Phát Triển Thị Trường (Kinh Doanh/Bán Hàng)",
            "--- Kỹ Sư Bán Hàng (Sale Engineer) (Kinh Doanh/Bán Hàng)", "--- Tổng Đài Viên (Kinh Doanh/Bán Hàng)",
            "--- Giao Dịch Viên (Tại Quầy) (Kinh Doanh/Bán Hàng)", "--- IT Helpdesk (Kinh Doanh/Bán Hàng)",
            "--- Chăm Sóc Khách Hàng (Kinh Doanh/Bán Hàng)", "--- Giám Sát Bán Hàng (Kinh Doanh/Bán Hàng)",
            "--- Thư Ký (Nhân Sự/Hành Chính)", "--- Hành chính Văn phòng (Nhân Sự/Hành Chính)",
            "--- Quản Trị Nhân Sự (HR) (Nhân Sự/Hành Chính)","--- Tuyển Dụng (HR) (Nhân Sự/Hành Chính)",
            "--- Lễ Tân (Receptionist) (Nhân Sự/Hành Chính)","--- Nhập Liệu (Nhân Sự/Hành Chính)",
            "--- Đào Tạo (Nhân Sự/Hành Chính)","--- Bảo Hiểm Và Tiền Lương (CB) (Nhân Sự/Hành Chính)",
            "--- HRBP (Nhân Sự/Hành Chính)", "--- Văn Thư/Lưu Trữ (Nhân Sự/Hành Chính)",
            "--- Luật (Luật/Pháp Chế)","--- Pháp Chế (Luật/Pháp Chế)",
            "--- Kiểm Soát Nội Bộ (Luật/Pháp Chế)","--- Kế Toán Tổng Hợp (Kế Toán/Kiểm Toán)","--- Thủ Quỹ (Kế Toán/Kiểm Toán)",
            "--- AccNet (Kế Toán/Kiểm Toán)","--- FAST (Kế Toán/Kiểm Toán)","--- LinkQ (Kế Toán/Kiểm Toán)",
            "--- CeAC (Kế Toán/Kiểm Toán)","--- Misa (Kế Toán/Kiểm Toán)","--- CPA (Kế Toán/Kiểm Toán)",
            "--- ACCA (Kế Toán/Kiểm Toán)","--- Kế Toán Xây Dựng (Kế Toán/Kiểm Toán)","--- Kế Toán Thuế (Kế Toán/Kiểm Toán)",
            "--- Thu Ngân (Kế Toán/Kiểm Toán)","--- Kiểm Toán (Kế Toán/Kiểm Toán)","--- Kế Toán Quản Trị (Kế Toán/Kiểm Toán)",
            "--- Kế Toán Bán Hàng (Kế Toán/Kiểm Toán)","--- Kế Toán Sản Xuất (Kế Toán/Kiểm Toán)","--- Kế Toán Kho (Kế Toán/Kiểm Toán)",
            "--- Triển Khai Phần Mềm (Công Nghệ Thông Tin)","--- Big Data/Hadoop (Công Nghệ Thông Tin)","--- Data Analytics (Công Nghệ Thông Tin)",
            "--- An Ninh Mạng (Công Nghệ Thông Tin)","--- IT Support (Công Nghệ Thông Tin)","--- SAP (Công Nghệ Thông Tin)","--- Quản Trị Hệ Thống (Công Nghệ Thông Tin)",
            "--- Quản Trị Mạng (Công Nghệ Thông Tin)","--- Javascript (Công Nghệ Thông Tin)","--- Frontend (HTML/CSS) (Công Nghệ Thông Tin)",
            "--- Ruby (Công Nghệ Thông Tin)","--- Kỹ Sư Cầu Nối (Công Nghệ Thông Tin)","--- Data Warehouse (Công Nghệ Thông Tin)","--- Cloud (AWS/Azure) (Công Nghệ Thông Tin)",
            "--- Machine Learning (Công Nghệ Thông Tin)","--- Thiết Kế Vi Mạch (Công Nghệ Thông Tin)","--- Firmware (Công Nghệ Thông Tin)","--- CCNA (Công Nghệ Thông Tin)",
            "--- CCNP (Công Nghệ Thông Tin)","--- ASP.NET (Công Nghệ Thông Tin)","--- Quản Lý Dự Án (IT) (Công Nghệ Thông Tin)",
            "--- iOS (Công Nghệ Thông Tin)",
            "--- Android (Công Nghệ Thông Tin)","--- Backend (Công Nghệ Thông Tin)","--- Java (Công Nghệ Thông Tin)",
            "--- C/C++ (Công Nghệ Thông Tin)",
            "--- SQL (Công Nghệ Thông Tin)","--- Oracle (Công Nghệ Thông Tin)","--- Quản Lý Sản Phẩm (Công Nghệ Thông Tin)",
            "--- .NET/C# (Công Nghệ Thông Tin)",
            "--- Hệ Nhúng (Công Nghệ Thông Tin)","--- Full-stack (Công Nghệ Thông Tin)","--- Phần Cứng Máy Tính/Điện Thoại (Công Nghệ Thông Tin)",
            "--- NodeJS (Công Nghệ Thông Tin)","--- PHP (Công Nghệ Thông Tin)","--- Python (Công Nghệ Thông Tin)","--- QA/Test (Công Nghệ Thông Tin)",
            "--- Nội Dung (Quảng cáo/Marketing/PR)","--- Seeding (Forum/Facebook) (Quảng cáo/Marketing/PR)","--- Media Buying (Quảng cáo/Marketing/PR)",
            "--- Media Planning (Quảng cáo/Marketing/PR)","--- SEO (Quảng cáo/Marketing/PR)","--- Facebook Ads (Quảng cáo/Marketing/PR)",
            "--- Google Ads(AdWords) (Quảng cáo/Marketing/PR)","--- Quản Trị Thương Hiệu (Branding) (Quảng cáo/Marketing/PR)",
            "--- Offline Marketing (Báo/Đài/Truyền Hình) (Quảng cáo/Marketing/PR)","--- Sự Kiện (Quảng cáo/Marketing/PR)",
            "--- Social Marketing (Quảng cáo/Marketing/PR)","--- Digital Marketing (Quảng cáo/Marketing/PR)","--- Truyền Thông/Báo Chí (Quảng cáo/Marketing/PR)",
            "--- An Toàn Lao Động (Sản Xuất )","--- Mộc/Đồ Gỗ (Sản Xuất )","--- In Ấn/Xuất Bản (Sản Xuất )","--- Vận Hành Sản Xuất (Sản Xuất )",
            "--- Chất Lượng (QA/QC) (Sản Xuất )","--- Vật Tư Sản Xuất (Sản Xuất )","--- Quản Lý Điều Hành (Sản Xuất )","--- Chế Biến Thực Phẩm (Sản Xuất )",
            "--- Dệt May/Da Giày (Sản Xuất )","--- Giám Sát Sản Xuất (Sản Xuất )","--- Kỹ Thuật Công Nghiệp (Sản Xuất )","--- Nghiên Cứu Và Phát Triển (RD) (Sản Xuất )",
            "--- Trang Sức (Sản Xuất )","--- Môi Giới Chứng Khoán (Tài Chính/Ngân Hàng)","--- Phân Tích Tài Chính (Tài Chính/Ngân Hàng)","--- Xử Lý Nợ (Tài Chính/Ngân Hàng)--- Giao Dịch"
        };

        public static Dictionary<string, string> lst_jobsgo()
        {
            Dictionary<string, string> lst_nganh = new Dictionary<string, string>();

            foreach (var lst in list_nganh_nghe_jobsgo)
            {
                lst_nganh.Add(lst, lst.Replace("--- ", ""));
            }

            return lst_nganh;
        }

        public static List<string> list_nganh_nghe_careerbuilder = new List<string>
        {
            "Tiếp thị / Marketing", "Bán lẻ / Bán sỉ", "Bán hàng / Kinh doanh", "Tiếp thị trực tuyến", "Dược phẩm",
            "Y tế / Chăm sóc sức khỏe", "Tư vấn","Dịch vụ khách hàng","Phi chính phủ / Phi lợi nhuận","Luật / Pháp lý",
            "Bưu chính viễn thông","Vận chuyển / Giao nhận /  Kho vận","Lao động phổ thông","An Ninh / Bảo Vệ","Giáo dục / Đào tạo",
            "Thư viện","Hàng gia dụng / Chăm sóc cá nhân","Thực phẩm & Đồ uống","Hành chính / Thư ký","Quản lý điều hành","Nhân sự",
            "Biên phiên dịch","Kế toán / Kiểm toán","Ngân hàng","Bảo hiểm","Chứng khoán","Tài chính / Đầu tư","Nhà hàng / Khách sạn",
            "Du lịch","Hàng không","Nông nghiệp","Thống kê","Thủy sản / Hải sản","Lâm Nghiệp","Chăn nuôi / Thú y","Thủy lợi","Trắc địa / Địa Chất",
            "Hàng hải","Công nghệ sinh học","Công nghệ thực phẩm / Dinh dưỡng","Cơ khí / Ô tô / Tự động hóa","Môi trường","Dầu khí",
            "Hóa học","Điện / Điện tử / Điện lạnh","Khoáng sản","Bảo trì / Sửa chữa","CNTT - Phần mềm","CNTT - Phần cứng / Mạng",
            "Mỹ thuật / Nghệ thuật / Thiết kế","Giải trí","Truyền hình / Báo chí / Biên tập","Quảng cáo / Đối ngoại / Truyền Thông","Tổ chức sự kiện",
            "Xuất nhập khẩu","Sản xuất / Vận hành sản xuất","Đồ gỗ","Dệt may / Da giày / Thời trang","Quản lý chất lượng (QA/QC)","Thu mua / Vật tư",
            "An toàn lao động","In ấn / Xuất bản","Kiến trúc","Xây dựng","Bất động sản","Nội ngoại thất","Ngành khác","Mới tốt nghiệp / Thực tập"
        };

        public static List<string> list_luong = new List<string>()
        {
            "1-3 triệu", "3-5 triệu", "5-7 triệu", "7-10 triệu", "10-12 triệu", "12-15 triệu", "15-20 triệu",
            "20-25 triệu", "25-30 triệu", "30 triệu trở lên"
        };
    }

    public static class Es
    {
        public static bool IndexLinkPosted(List<LinkSaved> lst)
        {
            bool is_success = false;
            if (lst.Count() > 0)
            {
                var count = ES.LinkSavedRepository.Instance.IndexMany(lst);
                is_success = count > 0;
            }
            return is_success;
        }
    }

    public class CauHinhSite
    {
        public string user_name { get; set; }
        public string password { get; set; }
        public string xpath_user { get; set; }
        public string xpath_pass { get; set; }
    }

    public static class CauHinhAccount
    {
        public static void CauHinhJobStreet(CauHinh ch)
        {
            ch.username = "thunt@xmedia.vn";
            ch.password = "XM25122788";
            ch.url_login = "https://employer.jobstreet.vn/vn/login";
            ch.xpath_password = "//input[@id='password']";
            ch.xpath_username = "//input[@id='email']";
        }

        public static void CauHinhMyWork(CauHinh ch)
        {
            ch.username = "tuyendung.xmedia@gmail.com";
            ch.password = XMedia.XUtil.Encode("xm123456");
            ch.url_login = "https://mywork.com.vn/nha-tuyen-dung/dang-nhap";
            ch.xpath_password = "//input[@id='formLoginPassword']";
            ch.xpath_username = "//input[@id='formLoginEmail']";
        }

        public static void CauHinhTopCv(CauHinh ch)
        {
            ch.username = "thunt@xmedia.vn";
            ch.password = "phuongthu25122788";
            ch.url_login = "https://tuyendung.topcv.vn/login";
            ch.xpath_password = ".//input[@name='password']";
            ch.xpath_username = ".//input[@name='email']";
        }

        public static void CauHinhCareer(CauHinh ch)
        {
            ch.username = "tuyendung@xmedia.vn";
            ch.password = "phuongthu25122788";
            ch.url_login = "https://www.careerlink.vn/nha-tuyen-dung/login";
            ch.xpath_password = ".//input[@id='_password']";
            ch.xpath_username = ".//input[@id='_username']";
        }

        public static void CauHinhJobsGo(CauHinh ch)
        {
            ch.username = "tuyendung@xmedia.vn";
            ch.password = "@xmedia123";
            ch.url_login = "https://employer.jobsgo.vn/site/login";
            ch.xpath_password = ".//input[@id='loginform-password']";
            ch.xpath_username = ".//input[@id='loginform-user_name']";
        }
    }
}