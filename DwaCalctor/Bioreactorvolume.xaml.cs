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
    /// Bioreactorvolume.xaml 的交互逻辑
    /// </summary>
    public partial class Bioreactorvolume : UserControl
    {
        //构造 json 文件路径
        public string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

        public Bioreactorvolume()
        {
            InitializeComponent();
            Get_FlowData();
            Loaded += (s, e) => 
            {
                EnableUpdateDatabaseEvent(this);
            };
        }

        // 解析文本框内容为 double 类型的方法
        private double ParseDouble(string text)
        {
            double result = 0.0;
            double.TryParse(text, out result);
            return result;
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
                    textBox.TextChanged += Tbx_TS_BB_TextChanged;
                }
                EnableUpdateDatabaseEvent(child); // 递归遍历子控件

            }
        }

        /// <summary>
        /// 基础数据窗口初始化时即读取现有数据，以便用户查看；
        /// </summary>
        private void Get_FlowData()
        {
            try
            {
                // 从文件中读取 JSON 字符串
                string jsonString = File.ReadAllText(filePath);

                // 将 JSON 字符串反序列化为对象
                var data = JsonSerializer.Deserialize<JsonNode>(jsonString);

                // 从data.json中获取Groupbioreactor对象
                var bioreactordata = data["Groupbioreactor"];

                if (bioreactordata != null)
                {
                    Tbx_TS_BB.Text = bioreactordata["TS_BB"].ToString();
                    Tbx_V_BB.Text = bioreactordata["V_BB"].ToString();
                    Tbx_V_an.Text = bioreactordata["V_an"].ToString();
                    Tbx_V_D.Text = bioreactordata["V_D"].ToString();
                    Tbx_V_aero.Text = bioreactordata["V_aero"].ToString();
                    Tbx_V_bioT.Text = bioreactordata["V_bioT"].ToString();
                    Tbx_HRT_an.Text = bioreactordata["HRT_an"].ToString();
                    Tbx_HRT_D.Text = bioreactordata["HRT_D"].ToString();
                    Tbx_HRT_aero.Text = bioreactordata["HRT_aero"].ToString();
                    Tbx_HRT_bioT.Text = bioreactordata["HRT_bioT"].ToString();
                    Tbx_RF.Text = bioreactordata["RF"].ToString();
                    Tbx_RZ.Text = bioreactordata["RZ"].ToString();
                    Tbx_eta_0.Text = bioreactordata["eta_0"].ToString();
                }
                else
                {
                    Initializing_Json();
                }
            }
            catch (Exception ex)
            {
                // throw none
                MessageBox.Show(ex.Message);
            }
        }

        // 初始化窗口，读取json数据进行计算
        private void Initializing_Json()
        {
            if (!File.Exists(filePath))
            {
                // 未找到 json 文件，先去完善基础数据
                MessageBox.Show("注意，先前往完善基础数据！！", "数据缺失", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

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

                // 从data.json中获取Grouphydrobalance对象
                var hydrobalancedata = data["Grouphydrobalance"];

                // 从data.json中获取Groupnitribacter
                var nitribacterdata = data["Groupnitribacter"];

                // 从data.json中获取Groupdenitrial
                var denitrialdata = data["Groupdenitrial"];

                // 从data.json中获取Groupphosremoval
                var phosremovaldata = data["Groupphosremoval"];


                if (database != null && inflowdata != null &&
                    outflowdata != null && flowdata != null &&
                    nitribacterdata != null && hydrobalancedata != null &&
                    denitrialdata != null && phosremovaldata != null)
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

                    // 读取设计流量及系数值
                    double Q_d_Knoz = ParseDouble(flowdata["Q_d_Knoz"].ToString());
                    double Q_h_Knoz = ParseDouble(flowdata["Q_h_Knoz"].ToString());
                    double Kz = ParseDouble(flowdata["Kz"].ToString());
                    double Q_d_max = ParseDouble(flowdata["Q_d_max"].ToString());
                    double Q_h_max = ParseDouble(flowdata["Q_h_max"].ToString());

                    // 读取进出水碳平衡数值
                    double X_TS_ZB = ParseDouble(hydrobalancedata["X_TS_ZB"].ToString());
                    double X_COD_ZB = ParseDouble(hydrobalancedata["X_COD_ZB"].ToString());
                    double S_COD_ZB = ParseDouble(hydrobalancedata["S_COD_ZB"].ToString());
                    double S_COD_inert_ZB = ParseDouble(hydrobalancedata["S_COD_inert_ZB"].ToString());
                    double X_COD_inert_ZB = ParseDouble(hydrobalancedata["X_COD_inert_ZB"].ToString());
                    double C_COD_abb_ZB = ParseDouble(hydrobalancedata["C_COD_abb_ZB"].ToString());
                    double C_COD_la_ZB = ParseDouble(hydrobalancedata["C_COD_la_ZB"].ToString());
                    double X_anorg_TS_ZB = ParseDouble(hydrobalancedata["X_anorg_TS_ZB"].ToString());

                    // 读取进出水氮平衡数值
                    double S_TKN_AN = ParseDouble(hydrobalancedata["S_TKN_AN"].ToString());
                    double S_anorgN_UW = ParseDouble(hydrobalancedata["S_anorgN_UW"].ToString());


                    // 读取硝化反应污泥泥龄
                    double t_TS_aerob_Bem = ParseDouble(nitribacterdata["t_TS_aerob_Bem"].ToString());

                    // 读取反硝化参数值
                    double US_d_C = ParseDouble(denitrialdata["US_d_C"].ToString());
                    double t_TS_Bem = ParseDouble(denitrialdata["t_TS_Bem"].ToString());
                    double V_D_over_V_BB = ParseDouble(denitrialdata["V_D_over_V_BB"].ToString());
                    double S_NO3_AN = ParseDouble(denitrialdata["S_NO3_AN"].ToString());
                    double S_NO3_D = ParseDouble(denitrialdata["S_NO3_D"].ToString());

                    // 读取除磷反应参数值
                    double M_TS_BB = ParseDouble(phosremovaldata["M_TS_BB"].ToString());

                    //六、污泥浓度
                    double TS_BB = ParseDouble(Tbx_TS_BB.Text.ToString());   //生物池污泥浓度

                    //七、生物池容积
                    //7.1生物池容积

                    double V_BB = M_TS_BB / TS_BB;  //m3,曝气池的容积
                    Tbx_V_BB.Text = V_BB.ToString("F3");

                    double V_an = 1 * Q_h_Knoz;  //m3,厌氧池容积
                    Tbx_V_an.Text = V_an.ToString("F3");

                    double V_D = V_BB * V_D_over_V_BB;  //m3,缺氧池容积
                    Tbx_V_D.Text = V_D.ToString("F3");

                    double V_aero = V_BB - V_an;   //m3,好氧池容积
                    Tbx_V_aero.Text = V_aero.ToString("F3");

                    double V_bioT = V_BB + V_an;   //m3,总容积
                    Tbx_V_bioT.Text = V_bioT.ToString("F3");

                    double HRT_an = V_an / Q_h_Knoz;   //h,厌氧池水力停留时间
                    Tbx_HRT_an.Text = HRT_an.ToString("F3");

                    double HRT_D = V_D / Q_h_Knoz;  //h,缺氧池水力停留时间
                    Tbx_HRT_D.Text = HRT_D.ToString("F3");

                    double HRT_aero = V_aero / Q_h_Knoz;   //h,好氧池水力停留时间
                    Tbx_HRT_aero.Text = HRT_aero.ToString("F3");

                    double HRT_bioT = V_bioT / Q_h_Knoz;   //h,总水力停留时间
                    Tbx_HRT_bioT.Text = HRT_bioT.ToString("F3");

                    //7.2 回流比
                    double RF = S_NO3_D / S_NO3_AN;   //反硝化所需的回流比
                    Tbx_RF.Text = RF.ToString("F3");

                    double RZ = RF - 1;  //反硝化所需的内回流比
                    Tbx_RZ.Text = RZ.ToString("F3");

                    double eta_0 = 1 - 1 / (1 + RF);    //反硝化最大效率
                    Tbx_eta_0.Text = eta_0.ToString("F3");

                    // 定义生物池系统
                    var bioreactordata = new
                    {
                        TS_BB = Tbx_TS_BB.Text.ToString(),
                        V_BB = Tbx_V_BB.Text.ToString(),
                        V_an = Tbx_V_an.Text.ToString(),
                        V_D = Tbx_V_D.Text.ToString(),
                        V_aero = Tbx_V_aero.Text.ToString(),
                        V_bioT = Tbx_V_bioT.Text.ToString(),
                        HRT_an = Tbx_HRT_an.Text.ToString(),
                        HRT_D = Tbx_HRT_D.Text.ToString(),
                        HRT_aero = Tbx_HRT_aero.Text.ToString(),
                        HRT_bioT = Tbx_HRT_bioT.Text.ToString(),
                        RF = Tbx_RF.Text.ToString(),
                        RZ = Tbx_RZ.Text.ToString(),
                        eta_0 = Tbx_eta_0.Text.ToString(),
                };

                    // 将数据存储到 JSON 对象中
                    var newjsonData = new
                    {
                        Groupbioreactor = bioreactordata,
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
            catch (Exception ex)
            {
                // throw none;
                MessageBox.Show(ex.ToString());
            }
        }

        private void Tbx_TS_BB_TextChanged(object sender, TextChangedEventArgs e)
        {
            Initializing_Json();
        }
    }
}
