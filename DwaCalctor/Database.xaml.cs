using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;
using Table = Microsoft.Office.Interop.Word.Table;
using System.Xml;
using System.Text.Json;
using Path = System.IO.Path;

namespace DwaCalctor
{
    /// <summary>
    /// Database.xaml 的交互逻辑
    /// </summary>
    public partial class Database : UserControl
    {
        public Database()
        {
            InitializeComponent();

            // 初始化基础数据窗口，首先读取data.json的基础数据
            Get_Database();
        }

        /// <summary>
        /// 用于实时写入基础数据，同时存储用户输入
        /// </summary>
        private void Update_Database(object sender, TextChangedEventArgs e)
        {
            // 定义基础数据
            var database = new
            {
                f_s = double.Parse(Tbx_F_s.Text),   //溶解性的惰性COD占总COD比例,0.05~0.1,市政污水建议去0.05
                f_A = double.Parse(Tbx_F_A.Text),   //颗粒性惰性组分比例,0.2~0.35,对于城市污水取0.3
                f_COD = double.Parse(Tbx_F_COD.Text),   //易降解COD比例,0.15~0.25
                f_B = double.Parse(Tbx_F_B.Text),   //进水可过滤无机物质,进厂污水取0.3,初沉池污水取0.2
                S_orgN_AN = double.Parse(Tbx_S_orgN_AN.Text),   //mg/L,出水有机氮
                Y_COD_abb = double.Parse(Tbx_Y_COD_abb.Text),   //可降解COD产泥系数,降解每gCOD形成的生物量COD
                b = double.Parse(Tbx_B.Text),   //d^-1,15℃衰减系数
                SVI = double.Parse(Tbx_SVI.Text),   //120L/kg,100-150,污泥体积指数
                t_E = double.Parse(Tbx_T_E.Text),   //h,设计浓缩时间不能太长，防止污泥溶解和二沉池反硝化使沉淀污泥悬浮
                R_m2a = double.Parse(Tbx_R_m2a.Text),   //膜池2好氧池回流比
                R_D3an = double.Parse(Tbx_R_D3an.Text),   //缺氧池2厌氧池回流比
                C_0 = double.Parse(Tbx_C_O.Text),   //mg/L,混合液剩余DO值
                C_S = double.Parse(Tbx_C_S.Text),   //mg/L,标准条件下清水中饱和溶解氧
                T_TS = double.Parse(Tbx_T_TS.Text),   //摄氏度，计算混合液温度(标况)
                T_summer = double.Parse(Tbx_T_summer.Text),   //摄氏度，夏季温度
                h_p = double.Parse(Tbx_h_p.Text),   //m,管道阻力
                h_A = double.Parse(Tbx_h_A.Text),   //m,曝气器水头损失
                h_delta = double.Parse(Tbx_h_delta.Text),   //m,每升高1℃需补偿压力值
                S_NO3_ZB = double.Parse(Tbx_S_NO3_ZB.Text),   //mg/L，设定进水硝酸盐氮为0
                miu_A_max = double.Parse(Tbx_miu_A_max.Text),   //d^-1,最大比生长速率
            };


            // 二、进出水水质及平衡
            // 定义进水水质
            var inflowdata = new
            {
                // 2.1 输入参数
                C_COD_ZB = double.Parse(Tbx_C_COD_ZB.Text),  //进水化学需氧量
                C_BOD5_ZB = double.Parse(Tbx_C_BOD5_ZB.Text), //进水生物需氧量
                C_P_ZB = double.Parse(Tbx_C_P_ZB.Text),  //进水总磷
                C_TN_ZB = double.Parse(Tbx_C_TN_ZB.Text),  //进水总氮
                C_SS_ZB = double.Parse(Tbx_C_SS_ZB.Text),  //进水悬浮固体
                T_C = double.Parse(Tbx_T_C.Text),  //设计温度
            };

            // 定义出水水质
            var outflowdata = new
            {
                S_COD_AN = double.Parse(Tbx_S_COD_AN.Text),  //出水化学需要氧量
                S_BOD5_AN = double.Parse(Tbx_S_BOD5_AN.Text),  //出水生物需氧量
                S_TP_AN = double.Parse(Tbx_S_TP_AN.Text),  //出水总磷
                S_TN_AN = double.Parse(Tbx_S_TN_AN.Text),  //出水总氮
                S_NH4_AN = double.Parse(Tbx_S_NH4_AN.Text),  //出水氨氮
                S_SS_AN = double.Parse(Tbx_S_SS_AN.Text),  //出水悬浮固体
            };

            // 将数据存储到 JSON 对象中
            var jsonData = new
            {
                Groupdatabase = database,
                Groupinflow = inflowdata,
                Groupoutflow = outflowdata,
            };

            // 将 JSON 对象序列化为 JSON 字符串
            string jsonString = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions
            {
                WriteIndented = true // 设置为true，使输出的JSON格式化
            });

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
            // 将 JSON 字符串写入到文件
            File.WriteAllText("data.json", jsonString);
        }
        /// <summary>
        /// 基础数据窗口初始化时即读取现有数据，以便用户查看；
        /// </summary>
        private void Get_Database()
        {
            try {
                // 构造文件路径
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

                // 从文件中读取 JSON 字符串
                string jsonString = File.ReadAllText(filePath);

                // 将 JSON 字符串反序列化为对象
                var data = JsonSerializer.Deserialize<dynamic>(jsonString);

                //读取常数或建议数值
                Tbx_F_s.Text = data.Groupdatabase.f_s.ToSring();   //溶解性的惰性COD占总COD比例,0.05~0.1,市政污水建议去0.05
                Tbx_F_A.Text = data.Groupdatabase.f_A.ToString();   //颗粒性惰性组分比例,0.2~0.35,对于城市污水取0.3
                Tbx_F_COD.Text = data.Groupdatabase.f_COD.ToString();   //易降解COD比例,0.15~0.25
                Tbx_F_B.Text = data.Groupdatabase.f_B.ToString();   //进水可过滤无机物质,进厂污水取0.3,初沉池污水取0.2
                Tbx_S_orgN_AN.Text = data.Groupdatabase.S_orgN_AN.ToString();   //mg/L,出水有机氮
                Tbx_Y_COD_abb.Text = data.Groupdatabase.Y_COD_abb.ToString();   //可降解COD产泥系数,降解每gCOD形成的生物量COD
                Tbx_B.Text = data.Groupdatabase.b.ToString();   //d^-1,15℃衰减系数
                Tbx_SVI.Text = data.Groupdatabase.SVI.ToString();   //120L/kg,100-150,污泥体积指数
                Tbx_T_E.Text = data.Groupdatabase.t_E.ToString();   //h,设计浓缩时间不能太长，防止污泥溶解和二沉池反硝化使沉淀污泥悬浮
                Tbx_R_m2a.Text = data.Groupdatabase.R_m2a.ToString();   //膜池2好氧池回流比
                Tbx_R_D3an.Text = data.Groupdatabase.R_D3an.ToString();   //缺氧池2厌氧池回流比
                Tbx_C_O.Text = data.Groupdatabase.C_0.ToString();   //mg/L,混合液剩余DO值
                Tbx_C_S.Text = data.Groupdatabase.C_S.ToString();   //mg/L,标准条件下清水中饱和溶解氧
                Tbx_T_TS.Text = data.Groupdatabase.T_TS.ToString();   //摄氏度，计算混合液温度(标况)
                Tbx_T_summer.Text = data.Groupdatabase.T_summer.ToString();   //摄氏度，夏季温度
                Tbx_h_p.Text = data.Groupdatabase.h_p.ToString();   //m,管道阻力
                Tbx_h_A.Text = data.Groupdatabase.h_A.ToString();   //m,曝气器水头损失
                Tbx_h_delta.Text = data.Groupdatabase.h_delta.ToString();   //m,每升高1℃需补偿压力值
                Tbx_S_NO3_ZB.Text = data.Groupdatabase.S_NO3_ZB.ToString();   //mg/L，设定进水硝酸盐氮为0
                Tbx_miu_A_max.Text = data.Groupdatabase.miu_A_max.ToString();   //d^-1,最大比生长速率


                // 读取进出水水质数值
                // 2.1 输入参数
                Tbx_C_COD_ZB.Text = data.Groupinflow.C_COD_ZB.ToString();  //进水化学需氧量
                Tbx_C_BOD5_ZB.Text = data.Groupinflow.C_BOD5_ZB.ToString(); //进水生物需氧量
                Tbx_C_P_ZB.Text = data.Groupinflow.C_P_ZB.ToString();  //进水总磷
                Tbx_C_TN_ZB.Text = data.Groupinflow.C_TN_ZB.ToString();  //进水总氮
                Tbx_C_SS_ZB.Text = data.Groupinflow.C_SS_ZB.ToString();  //进水悬浮固体
                Tbx_T_C.Text = data.Groupinflow.T_C.ToString();  //设计温度
                Tbx_S_COD_AN.Text = data.Groupoutflow.S_COD_AN.ToString();  //出水化学需要氧量
                Tbx_S_BOD5_AN.Text = data.Groupoutflow.S_BOD5_AN.ToString();  //出水生物需氧量
                Tbx_S_TP_AN.Text = data.Groupoutflow.S_TP_AN.ToString();  //出水总磷
                Tbx_S_TN_AN.Text = data.Groupoutflow.S_TN_AN.ToString();  //出水总氮
                Tbx_S_NH4_AN.Text = data.Groupoutflow.S_NH4_AN.ToString();  //出水氨氮
                Tbx_S_SS_AN.Text = data.Groupoutflow.S_SS_AN.ToString();  //出水悬浮固体
            }
            catch 
            {
                // throw none
            }
        }
    }
}
