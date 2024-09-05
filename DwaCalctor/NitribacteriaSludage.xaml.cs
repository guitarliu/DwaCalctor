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
using Path=System.IO.Path;

namespace DwaCalctor
{
    /// <summary>
    /// NitribacteriaSludage.xaml 的交互逻辑
    /// </summary>
    public partial class NitribacteriaSludage : UserControl
    {

        //构造 json 文件路径
        public string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

        public NitribacteriaSludage()
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

                    // 从data.json中获取Groupflowdata对象
                    var flowdata = data["Groupflowdata"];

                    if (database != null && inflowdata != null && outflowdata != null && flowdata != null)
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

                        // 读取设计流量及系统值
                        double Q_d_Knoz = ParseDouble(flowdata["Q_d_Knoz"].ToString());
                        double Q_h_Knoz = ParseDouble(flowdata["Q_h_Knoz"].ToString());
                        double Kz = ParseDouble(flowdata["Kz"].ToString());
                        double Q_d_max = ParseDouble(flowdata["Q_d_max"].ToString());
                        double Q_h_max = ParseDouble(flowdata["Q_h_max"].ToString());

                        // 三、硝化菌泥龄
                        double B_D_COD_Z = Q_d_Knoz * C_COD_ZB / 1000;
                        Tbx_B_d_COD_Z.Text = B_D_COD_Z.ToString("F3");  //kg/d,COD日负荷
                        if (B_D_COD_Z <= 2400) { Tbx_PF.Text = "2.1"; }
                        else if (B_D_COD_Z > 12000) { Tbx_PF.Text = "1.5"; }
                        else { Tbx_PF.Text = (2.1 - (B_D_COD_Z - 2400) * 0.6 / 9600).ToString("F3"); } //硝化反应系数
                        double PF = double.Parse(Tbx_PF.Text);
                        Tbx_t_TS_aerob_Bem.Text = (PF * 1.6 / miu_A_max * Math.Pow(1.103, (15 - T_C))).ToString("F3"); //d,硝化菌污泥龄

                        // 定义硝化菌参数
                        var nitribacterdata = new
                        {
                            B_D_COD_Z = ParseDouble(Tbx_B_d_COD_Z.Text.Trim()),
                            PF = ParseDouble(Tbx_PF.Text.Trim()),
                            t_TS_aerob_Bem = ParseDouble(Tbx_t_TS_aerob_Bem.Text.Trim())
                        };

                        // 将数据存储到 JSON 对象中
                        var newjsonData = new
                        {
                            Groupnitribacter = nitribacterdata,
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
