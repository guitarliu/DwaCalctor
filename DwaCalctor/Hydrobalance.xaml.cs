using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
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
using Excel = Microsoft.Office.Interop.Excel;
using Path = System.IO.Path;


namespace DwaCalctor
{
    /// <summary>
    /// Hydrobalance.xaml 的交互逻辑
    /// </summary>
    public partial class Hydrobalance : UserControl
    {
        //构造 json 文件路径
        public string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        public Hydrobalance()
        {
            InitializeComponent();
            Initializing_Json();
        }

        // 解析文本框内容为 double 类型的方法
        private double ParseDouble(string text)
        {
            double result = 0.0;
            double.TryParse(text, out result);
            return result;
        }

        // 初始化窗口，读取json数据进行计算
        private void Initializing_Json()
        {
            if (!File.Exists(filePath))
            {
                // 未找到 json 文件，先去完善基础数据
                MessageBox.Show("注意，先前往完善基础数据！！", "数据缺失", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
            else
            {
                // 二、进出水水质及平衡
                // 2.2 碳平衡
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
                        double f_s = ParseDouble(database["f_s"].ToString());   //溶解性的惰性COD占总COD比例,0.05~0.1,市政污水建议去0.05
                        double f_A = ParseDouble(database["f_A"].ToString());   //颗粒性惰性组分比例,0.2~0.35,对于城市污水取0.3
                        double f_COD = ParseDouble(database["f_COD"].ToString());   //易降解COD比例,0.15~0.25
                        double f_B = ParseDouble(database["f_B"].ToString());   //进水可过滤无机物质,进厂污水取0.3,初沉池污水取0.2
                        double S_orgN_AN = ParseDouble(database["S_orgN_AN"].ToString());   //mg/L,出水有机氮
                        double Y_COD_abb = ParseDouble(database["Y_COD_abb"].ToString());   //可降解COD产泥系数,降解每gCOD形成的生物量COD
                        double b = ParseDouble(database["b"].ToString());   //d^-1,15℃衰减系数
                        double SVI = ParseDouble(database["SVI"].ToString());   //120L/kg,100-150,污泥体积指数
                        double t_E = ParseDouble(database["t_E"].ToString());   //h,设计浓缩时间不能太长，防止污泥溶解和二沉池反硝化使沉淀污泥悬浮
                        double R_m2a = ParseDouble(database["R_m2a"].ToString());   //膜池2好氧池回流比
                        double R_D3an = ParseDouble(database["R_D3an"].ToString());   //缺氧池2厌氧池回流比
                        double C_0 = ParseDouble(database["C_0"].ToString());   //mg/L,混合液剩余DO值
                        double C_S = ParseDouble(database["C_S"].ToString());   //mg/L,标准条件下清水中饱和溶解氧
                        double T_TS = ParseDouble(database["T_TS"].ToString());   //摄氏度，计算混合液温度(标况)
                        double T_summer = ParseDouble(database["T_summer"].ToString());   //摄氏度，夏季温度
                        double h_p = ParseDouble(database["h_p"].ToString());   //m,管道阻力
                        double h_A = ParseDouble(database["h_A"].ToString());   //m,曝气器水头损失
                        double h_delta = ParseDouble(database["h_delta"].ToString());   //m,每升高1℃需补偿压力值
                        double S_NO3_ZB = ParseDouble(database["S_NO3_ZB"].ToString());   //mg/L，设定进水硝酸盐氮为0
                        double miu_A_max = ParseDouble(database["miu_A_max"].ToString());   //d^-1,最大比生长速率


                        // 读取进出水水质数值
                        // 2.1 输入参数
                        double C_COD_ZB = ParseDouble(inflowdata["C_COD_ZB"].ToString());  //进水化学需氧量
                        double C_BOD5_ZB = ParseDouble(inflowdata["C_BOD5_ZB"].ToString()); //进水生物需氧量
                        double C_P_ZB = ParseDouble(inflowdata["C_P_ZB"].ToString());  //进水总磷
                        double C_TN_ZB = ParseDouble(inflowdata["C_TN_ZB"].ToString());  //进水总氮
                        double C_SS_ZB = ParseDouble(inflowdata["C_SS_ZB"].ToString());  //进水悬浮固体
                        double T_C = ParseDouble(inflowdata["T_C"].ToString());  //设计温度
                        double S_COD_AN = ParseDouble(outflowdata["S_COD_AN"].ToString());  //出水化学需要氧量
                        double S_BOD5_AN = ParseDouble(outflowdata["S_BOD5_AN"].ToString());  //出水生物需氧量
                        double S_TP_AN = ParseDouble(outflowdata["S_TP_AN"].ToString());  //出水总磷
                        double S_TN_AN = ParseDouble(outflowdata["S_TN_AN"].ToString());  //出水总氮
                        double S_NH4_AN = ParseDouble(outflowdata["S_NH4_AN"].ToString());  //出水氨氮
                        double S_SS_AN = ParseDouble(outflowdata["S_SS_AN"].ToString());  //出水悬浮固体

                        // 2.2 碳平衡
                        double X_TS_ZB = C_SS_ZB;  //进水可过滤物质
                        Tbx_X_TS_ZB.Text = X_TS_ZB.ToString(); //进水可过滤物质

                        double X_COD_ZB = X_TS_ZB * 1.6 * (1 - f_B);
                        Tbx_X_COD_ZB.Text = X_COD_ZB.ToString("F3");  //颗粒性COD(可过滤物质COD),有机干物质颗粒按1.6gCOD/oTS计

                        double S_COD_ZB = C_COD_ZB - X_COD_ZB;
                        Tbx_S_COD_ZB.Text = S_COD_ZB.ToString("F3");  //可溶解性COD

                        double S_COD_inert_ZB = f_s * C_COD_ZB;
                        Tbx_S_COD_inert_ZB.Text = S_COD_inert_ZB.ToString("F3");  //溶解性惰性组分

                        double X_COD_inert_ZB = f_A * X_COD_ZB;
                        Tbx_X_COD_inert_ZB.Text = X_COD_inert_ZB.ToString("F3");  //颗粒性惰性组分

                        double C_COD_abb_ZB = C_COD_ZB - S_COD_inert_ZB - X_COD_inert_ZB;
                        Tbx_C_COD_abb_ZB.Text = C_COD_abb_ZB.ToString("F3");  //可降解COD

                        double C_COD_la_ZB = f_COD * C_COD_abb_ZB;
                        Tbx_C_COD_la_ZB.Text = C_COD_la_ZB.ToString("F3");  //易降解COD

                        double X_anorg_TS_ZB = f_B * X_TS_ZB;
                        Tbx_X_anorg_TS_ZB.Text = X_anorg_TS_ZB.ToString("F3");  //进水可过滤无机物质(仅算数,进水颗粒性COD没有直接用)

                        //2.3出水氮平衡
                        double S_TKN_AN = S_NH4_AN + S_orgN_AN;  //mg/L,出水凯氏氮
                        double S_anorgN_UW = S_TN_AN - S_TKN_AN;  //mg/L,出水硝酸盐氮

                        Tbx_S_TKN_AN.Text = S_TKN_AN.ToString("F3");  //mg/L,出水凯氏氮
                        Tbx_S_anorgN_UW.Text = S_anorgN_UW.ToString("F3");  //mg/L,出水硝酸盐氮


                        // 定义进出水碳氮平衡参数
                        var hydrobalancedata = new 
                        {
                            X_TS_ZB = ParseDouble(Tbx_X_TS_ZB.Text.Trim()),
                            X_COD_ZB = ParseDouble(Tbx_X_COD_ZB.Text.Trim()),
                            S_COD_ZB = ParseDouble(Tbx_S_COD_ZB.Text.Trim()),
                            S_COD_inert_ZB = ParseDouble(Tbx_S_COD_inert_ZB.Text.Trim()),
                            X_COD_inert_ZB = ParseDouble(Tbx_X_COD_inert_ZB.Text.Trim()),
                            C_COD_abb_ZB = ParseDouble(Tbx_C_COD_abb_ZB.Text.Trim()),
                            C_COD_la_ZB = ParseDouble(Tbx_C_COD_la_ZB.Text.Trim()),
                            X_anorg_TS_ZB = ParseDouble(Tbx_X_anorg_TS_ZB.Text.Trim()),
                            S_TKN_AN = ParseDouble(Tbx_S_TKN_AN.Text.Trim()),
                            S_anorgN_UW = ParseDouble(Tbx_S_anorgN_UW.Text.Trim())
                        };

                        // 将数据存储到 JSON 对象中
                        var newjsonData = new
                        {
                            Grouphydrobalance = hydrobalancedata,
                        };

                        // 将数据序列化为 JSON 对象
                        JsonObject newData = JsonSerializer.SerializeToNode(newjsonData) as JsonObject;


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
                    else 
                    {
                        // 找到 json 文件，但基础数据缺失，先去完善基础数据
                        MessageBox.Show("注意，先前往完善基础数据！！", "数据缺失", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch
                {
                    // throw none;
                }
            }
         
        }
    }
}
