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
    /// Oxygendemand.xaml 的交互逻辑
    /// </summary>
    public partial class Oxygendemand : UserControl
    {
        //构造 json 文件路径
        public string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        public Oxygendemand()
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

                // 从data.json中获取Groupoxygendemand对象
                var oxygendemanddata = data["Groupoxygendemand"];

                if (oxygendemanddata != null)
                {
                    Tbx_OV_d_C.Text= oxygendemanddata["OV_d_C"].ToString();
                    Tbx_OV_d_N.Text= oxygendemanddata["OV_d_N"].ToString();
                    Tbx_OV_d_D.Text= oxygendemanddata["OV_d_D"].ToString();
                    Tbx_OV_h_aM.Text= oxygendemanddata["OV_h_aM"].ToString();
                    Tbx_OV_h_max.Text= oxygendemanddata["OV_h_max"].ToString();
                    Tbx_alfa.Text= oxygendemanddata["alfa"].ToString();
                    Tbx_beta.Text= oxygendemanddata["beta"].ToString();
                    Tbx_h_TB2A.Text= oxygendemanddata["h_TB2A"].ToString();
                    Tbx_h_tk.Text= oxygendemanddata["h_tk"].ToString();
                    Tbx_h_El.Text= oxygendemanddata["h_El"].ToString();
                    Tbx_E_A.Text= oxygendemanddata["E_A"].ToString();
                    Tbx_O_t.Text= oxygendemanddata["O_t"].ToString();
                    Tbx_P_a.Text= oxygendemanddata["P_a"].ToString();
                    Tbx_P_b.Text= oxygendemanddata["P_b"].ToString();
                    Tbx_C_SW.Text= oxygendemanddata["C_SW"].ToString();
                    Tbx_C_SM.Text= oxygendemanddata["C_SM"].ToString();
                    Tbx_FCF.Text= oxygendemanddata["FCF"].ToString();
                    Tbx_SOR.Text= oxygendemanddata["SOR"].ToString();
                    Tbx_G_S.Text= oxygendemanddata["G_S"].ToString();
                    Tbx_V_GS_over_V_knoz.Text = oxygendemanddata["V_GS_over_V_knoz"].ToString();
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

        private void Tbx_TS_BB_TextChanged(object sender, TextChangedEventArgs e)
        {
            Initializing_Json();
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
                    double OV_C = ParseDouble(denitrialdata["OV_C"].ToJsonString());

                    // 读取除磷反应参数值
                    double M_TS_BB = ParseDouble(phosremovaldata["M_TS_BB"].ToString());

                    //八、需氧量
                    //8.1 耗氧量物料平衡
                    double OV_d_C = Q_d_Knoz * OV_C / 1000;   //kgO2/d, 碳去除的耗氧量
                    Tbx_OV_d_C.Text = OV_d_C.ToString("F3");

                    double OV_d_N = Q_d_Knoz * 4.3 * (S_NO3_D - S_NO3_ZB + S_NO3_AN) / 1000;   //kgO2/d, 反硝化回收供氧量
                    Tbx_OV_d_N.Text = OV_d_N.ToString("F3");

                    double OV_d_D = Q_d_Knoz * 2.86 * S_NO3_D / 1000;   //kgO2/d, 反硝化回收供氧量
                    Tbx_OV_d_D.Text = OV_d_D.ToString("F3");

                    double OV_h_aM = ((OV_d_C - OV_d_D) + OV_d_N) / 24;   //kgO2/h，平均耗氧量
                    Tbx_OV_h_aM.Text = OV_h_aM.ToString("F3");

                    double OV_h_max = Kz * OV_h_aM;   //kgO2/h，最高耗氧量
                    Tbx_OV_h_max.Text = OV_h_max.ToString("F3");

                    // 8.2 标准传氧速率
                    double alfa = ParseDouble(Tbx_alfa.Text.ToString());  //0.8-0.85,混合液KLa/清水KLa

                    double beta = ParseDouble(Tbx_beta.Text.ToString());  //0.9~0.97，混合液饱和溶解氧/清水饱和溶解氧

                    double h_TB2A = ParseDouble(Tbx_h_TB2A.Text.ToString());   //曝气装置与池底距离(m)

                    double h_tk = ParseDouble(Tbx_h_tk.Text.ToString());   //设计水深(m)

                    double h_El = ParseDouble(Tbx_h_El.Text.ToString());   //当地海拔高度(m)

                    double E_A = ParseDouble(Tbx_E_A.Text.ToString());   //氧利用率

                    double O_t = 21 * (1 - E_A) / (79 + 21 * (1 - E_A));   //曝气池逸出气体中含氧率
                    Tbx_O_t.Text = O_t.ToString("F3");

                    double P_a = (101325 - h_El / 12 / 133) / 1000000;   //Mpa,当地大气压力
                    Tbx_P_a.Text = P_a.ToString("F3");

                    double P_b = P_a + (h_tk - h_TB2A) * 9.81 / 1000;   //Mpa曝气装置处绝对压力
                    Tbx_P_b.Text = P_b.ToString("F3");

                    double C_SW = 8.24 * P_a / 0.101325;   //mg/L,清水表面饱和溶解氧
                    Tbx_C_SW.Text = C_SW.ToString("F3");

                    double C_SM = C_SW * (O_t / 42 + P_b / (2 * P_a));   //mg/L,水下深度到池面清水平均溶氧值
                    Tbx_C_SM.Text = C_SM.ToString("F3");

                    double FCF = alfa * (beta * C_SM - C_0) / C_S;  //AOR与SOR转换系数
                    Tbx_FCF.Text = FCF.ToString("F3");

                    double SOR = OV_h_aM / FCF;  //kgO2/h,标准传氧速率SOR
                    Tbx_SOR.Text = SOR.ToString("F3");

                    double G_S = SOR / (0.28 * E_A);  //标准状况供空气体积
                    Tbx_G_S.Text = G_S.ToString("F3");

                    double V_GS_over_V_knoz = G_S / Q_h_Knoz;  //气水比
                    Tbx_V_GS_over_V_knoz.Text = V_GS_over_V_knoz.ToString("F3");

                    // 定义生物池系统
                    var oxygendemanddata = new
                    {
                        OV_d_C = Tbx_OV_d_C.Text.ToString(),
                        OV_d_N = Tbx_OV_d_N.Text.ToString(),
                        OV_d_D = Tbx_OV_d_D.Text.ToString(),
                        OV_h_aM = Tbx_OV_h_aM.Text.ToString(),
                        OV_h_max = Tbx_OV_h_max.Text.ToString(),
                        alfa = Tbx_alfa.Text.ToString(),
                        beta = Tbx_beta.Text.ToString(),
                        h_TB2A = Tbx_h_TB2A.Text.ToString(),
                        h_tk = Tbx_h_tk.Text.ToString(),
                        h_El = Tbx_h_El.Text.ToString(),
                        E_A = Tbx_E_A.Text.ToString(),
                        O_t = Tbx_O_t.Text.ToString(),
                        P_a = Tbx_P_a.Text.ToString(),
                        P_b = Tbx_P_b.Text.ToString(),
                        C_SW = Tbx_C_SW.Text.ToString(),
                        C_SM = Tbx_C_SM.Text.ToString(),
                        FCF = Tbx_FCF.Text.ToString(),
                        SOR = Tbx_SOR.Text.ToString(),
                        G_S = Tbx_G_S.Text.ToString(),
                        V_GS_over_V_knoz = Tbx_V_GS_over_V_knoz.Text.ToString(),                
                    };

                    // 将数据存储到 JSON 对象中
                    var newjsonData = new
                    {
                        Groupoxygendemand = oxygendemanddata,
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
    }
}
