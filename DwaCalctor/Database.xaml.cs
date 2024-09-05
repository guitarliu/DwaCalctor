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
using System.Text.Json.Nodes;

namespace DwaCalctor
{
    /// <summary>
    /// Database.xaml 的交互逻辑
    /// </summary>
    public partial class Database : UserControl
    {
        //构造 json 文件路径
        public string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

        public Database()
        {
            InitializeComponent();

            if (!File.Exists(filePath))
            {
                Initializing_Json();
            }

            // 初始化基础数据窗口，首先读取data.json的基础数据
            Get_Database();
            Loaded += (s, e) =>
            {
                EnableUpdateDatabaseEvent(this);
            };

        }


        // 启用所有 TextBox 的 TextChanged 事件
        public void EnableUpdateDatabaseEvent(DependencyObject parent)
        {
            if (parent == null)
                return;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is TextBox textBox)
                {
                    textBox.TextChanged += Update_Database;
                }
                EnableUpdateDatabaseEvent(child); // 递归遍历子控件

            }
        }

        private void Initializing_Json()
        {
            // 新数据
            // 定义基础数据
            var database = new
            {
                f_s = ParseDouble(Tbx_F_s.Text.Trim()),   //溶解性的惰性COD占总COD比例,0.05~0.1,市政污水建议去0.05
                f_A = ParseDouble(Tbx_F_A.Text.Trim()),   //颗粒性惰性组分比例,0.2~0.35,对于城市污水取0.3
                f_COD = ParseDouble(Tbx_F_COD.Text.Trim()),   //易降解COD比例,0.15~0.25
                f_B = ParseDouble(Tbx_F_B.Text.Trim()),   //进水可过滤无机物质,进厂污水取0.3,初沉池污水取0.2
                S_orgN_AN = ParseDouble(Tbx_S_orgN_AN.Text.Trim()),   //mg/L,出水有机氮
                Y_COD_abb = ParseDouble(Tbx_Y_COD_abb.Text.Trim()),   //可降解COD产泥系数,降解每gCOD形成的生物量COD
                b = ParseDouble(Tbx_B.Text.Trim()),   //d^-1,15℃衰减系数
                SVI = ParseDouble(Tbx_SVI.Text.Trim()),   //120L/kg,100-150,污泥体积指数
                t_E = ParseDouble(Tbx_T_E.Text.Trim()),   //h,设计浓缩时间不能太长，防止污泥溶解和二沉池反硝化使沉淀污泥悬浮
                R_m2a = ParseDouble(Tbx_R_m2a.Text.Trim()),   //膜池2好氧池回流比
                R_D3an = ParseDouble(Tbx_R_D3an.Text.Trim()),   //缺氧池2厌氧池回流比
                C_0 = ParseDouble(Tbx_C_O.Text.Trim()),   //mg/L,混合液剩余DO值
                C_S = ParseDouble(Tbx_C_S.Text.Trim()),   //mg/L,标准条件下清水中饱和溶解氧
                T_TS = ParseDouble(Tbx_T_TS.Text.Trim()),   //摄氏度，计算混合液温度(标况)
                T_summer = ParseDouble(Tbx_T_summer.Text.Trim()),   //摄氏度，夏季温度
                h_p = ParseDouble(Tbx_h_p.Text.Trim()),   //m,管道阻力
                h_A = ParseDouble(Tbx_h_A.Text.Trim()),   //m,曝气器水头损失
                h_delta = ParseDouble(Tbx_h_delta.Text.Trim()),   //m,每升高1℃需补偿压力值
                S_NO3_ZB = ParseDouble(Tbx_S_NO3_ZB.Text.Trim()),   //mg/L，设定进水硝酸盐氮为0
                miu_A_max = ParseDouble(Tbx_miu_A_max.Text.Trim()),   //d^-1,最大比生长速率
            };


            // 二、进出水水质及平衡
            // 定义进水水质
            var inflowdata = new
            {
                // 2.1 输入参数
                C_COD_ZB = ParseDouble(Tbx_C_COD_ZB.Text.Trim()),  //进水化学需氧量
                C_BOD5_ZB = ParseDouble(Tbx_C_BOD5_ZB.Text.Trim()), //进水生物需氧量
                C_P_ZB = ParseDouble(Tbx_C_P_ZB.Text.Trim()),  //进水总磷
                C_TN_ZB = ParseDouble(Tbx_C_TN_ZB.Text.Trim()),  //进水总氮
                C_SS_ZB = ParseDouble(Tbx_C_SS_ZB.Text.Trim()),  //进水悬浮固体
                T_C = ParseDouble(Tbx_T_C.Text.Trim()),  //设计温度
            };

            // 定义出水水质
            var outflowdata = new
            {
                S_COD_AN = ParseDouble(Tbx_S_COD_AN.Text.Trim()),  //出水化学需要氧量
                S_BOD5_AN = ParseDouble(Tbx_S_BOD5_AN.Text.Trim()),  //出水生物需氧量
                S_TP_AN = ParseDouble(Tbx_S_TP_AN.Text.Trim()),  //出水总磷
                S_TN_AN = ParseDouble(Tbx_S_TN_AN.Text.Trim()),  //出水总氮
                S_NH4_AN = ParseDouble(Tbx_S_NH4_AN.Text.Trim()),  //出水氨氮
                S_SS_AN = ParseDouble(Tbx_S_SS_AN.Text.Trim()),  //出水悬浮固体
            };

            // 将数据存储到 JSON 对象中
            var newjsonData = new
            {
                Groupdatabase = database,
                Groupinflow = inflowdata,
                Groupoutflow = outflowdata,
            };

            // 将新数据序列化为 JSON 对象
            JsonObject newData = JsonSerializer.SerializeToNode(newjsonData) as JsonObject;

            if (!File.Exists(filePath))
            {
                // 将 JSON 字符串写入到文件
                File.WriteAllText(filePath, newData.ToString());
            }

            else 
            {
                // 先读取现有json文件内容，再将新内容加进去

                // 从文件中读取 JSON 字符串
                string oldjsonString = File.ReadAllText(filePath);

                // 将 JSON 字符串反序列化为对象
                JsonObject oldData = JsonSerializer.Deserialize<JsonObject>(oldjsonString) ?? new JsonObject();

                // 合并新数据到现有数据中
                foreach (var property in newData)
                {
                    oldData[property.Key] = property.Value.DeepClone();
                }

                // 将更新后的数据序列化为 JSON 字符串
                string updatedJsonString = oldData.ToString();

                // 将 JSON 字符串写入到文件
                File.WriteAllText(filePath, updatedJsonString);
            }
        }
        /// <summary>
        /// 基础数据窗口初始化时即读取现有数据，以便用户查看；
        /// </summary>
        private void Get_Database()
        {
            try
            {
                // 从文件中读取 JSON 字符串
                string jsonString = File.ReadAllText(filePath);

                // 将 JSON 字符串反序列化为对象
                var data = JsonSerializer.Deserialize<JsonNode>(jsonString);

                // 从data.json中获取Groupdatabase对象
                var database = data["Groupdatabase"];

                // 从data.json中获取Groupinflow
                var inflowdata = data["Groupinflow"];

                //从data.json中获取Groupoutflow
                var outflowdata = data["Groupoutflow"];

                if (database != null && inflowdata != null && outflowdata != null)
                {

                    //读取常数或建议数值
                    Tbx_F_s.Text = database["f_s"].ToString();   //溶解性的惰性COD占总COD比例,0.05~0.1,市政污水建议去0.05
                    Tbx_F_A.Text = database["f_A"].ToString();   //颗粒性惰性组分比例,0.2~0.35,对于城市污水取0.3
                    Tbx_F_COD.Text = database["f_COD"].ToString();   //易降解COD比例,0.15~0.25
                    Tbx_F_B.Text = database["f_B"].ToString();   //进水可过滤无机物质,进厂污水取0.3,初沉池污水取0.2
                    Tbx_S_orgN_AN.Text = database["S_orgN_AN"].ToString();   //mg/L,出水有机氮
                    Tbx_Y_COD_abb.Text = database["Y_COD_abb"].ToString();   //可降解COD产泥系数,降解每gCOD形成的生物量COD
                    Tbx_B.Text = database["b"].ToString();   //d^-1,15℃衰减系数
                    Tbx_SVI.Text = database["SVI"].ToString();   //120L/kg,100-150,污泥体积指数
                    Tbx_T_E.Text = database["t_E"].ToString();   //h,设计浓缩时间不能太长，防止污泥溶解和二沉池反硝化使沉淀污泥悬浮
                    Tbx_R_m2a.Text = database["R_m2a"].ToString();   //膜池2好氧池回流比
                    Tbx_R_D3an.Text = database["R_D3an"].ToString();   //缺氧池2厌氧池回流比
                    Tbx_C_O.Text = database["C_0"].ToString();   //mg/L,混合液剩余DO值
                    Tbx_C_S.Text = database["C_S"].ToString();   //mg/L,标准条件下清水中饱和溶解氧
                    Tbx_T_TS.Text = database["T_TS"].ToString();   //摄氏度，计算混合液温度(标况)
                    Tbx_T_summer.Text = database["T_summer"].ToString();   //摄氏度，夏季温度
                    Tbx_h_p.Text = database["h_p"].ToString();   //m,管道阻力
                    Tbx_h_A.Text = database["h_A"].ToString();   //m,曝气器水头损失
                    Tbx_h_delta.Text = database["h_delta"].ToString();   //m,每升高1℃需补偿压力值
                    Tbx_S_NO3_ZB.Text = database["S_NO3_ZB"].ToString();   //mg/L，设定进水硝酸盐氮为0
                    Tbx_miu_A_max.Text = database["miu_A_max"].ToString();   //d^-1,最大比生长速率


                    // 读取进出水水质数值
                    // 2.1 输入参数
                    Tbx_C_COD_ZB.Text = inflowdata["C_COD_ZB"].ToString();  //进水化学需氧量
                    Tbx_C_BOD5_ZB.Text = inflowdata["C_BOD5_ZB"].ToString(); //进水生物需氧量
                    Tbx_C_P_ZB.Text = inflowdata["C_P_ZB"].ToString();  //进水总磷
                    Tbx_C_TN_ZB.Text = inflowdata["C_TN_ZB"].ToString();  //进水总氮
                    Tbx_C_SS_ZB.Text = inflowdata["C_SS_ZB"].ToString();  //进水悬浮固体
                    Tbx_T_C.Text = inflowdata["T_C"].ToString();  //设计温度
                    Tbx_S_COD_AN.Text = outflowdata["S_COD_AN"].ToString();  //出水化学需要氧量
                    Tbx_S_BOD5_AN.Text = outflowdata["S_BOD5_AN"].ToString();  //出水生物需氧量
                    Tbx_S_TP_AN.Text = outflowdata["S_TP_AN"].ToString();  //出水总磷
                    Tbx_S_TN_AN.Text = outflowdata["S_TN_AN"].ToString();  //出水总氮
                    Tbx_S_NH4_AN.Text = outflowdata["S_NH4_AN"].ToString();  //出水氨氮
                    Tbx_S_SS_AN.Text = outflowdata["S_SS_AN"].ToString();  //出水悬浮固体
                }
                else 
                {
                    Initializing_Json();
                }
            }
            catch
            {
                
            }
        }

        // 解析文本框内容为 double 类型的方法
        private double ParseDouble(string text)
        {
            double result = 0.0;
            double.TryParse(text, out result);
            return result;
        }



        /// <summary>
        /// 用于实时写入基础数据，同时存储用户输入
        /// </summary>
        private void Update_Database(object sender, TextChangedEventArgs e)
        {
            Initializing_Json();
        }
    }
}
