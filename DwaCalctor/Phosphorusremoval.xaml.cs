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
    /// Phosphorusremoval.xaml 的交互逻辑
    /// </summary>
    public partial class Phosphorusremoval : UserControl
    {
        //构造 json 文件路径
        public string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

        public Phosphorusremoval()
        {
            InitializeComponent();
            Get_Original_Data();
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


                if (database != null && inflowdata != null &&
                    outflowdata != null && flowdata != null &&
                    nitribacterdata != null && hydrobalancedata != null &&
                    denitrialdata != null)
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
                    double S_NO3_D = ParseDouble(denitrialdata["S_NO3_D"].ToString());

                    // 五、除磷
                    //5.1 生物处理与化学除磷量
                    double C_P_AN = 0.7 * S_TP_AN;  //mg/L,出水浓度
                    Tbx_C_P_AN.Text = C_P_AN.ToString("F3");

                    double X_P_BM = 0.005 * C_COD_ZB;   //形mg/L,成活性污泥的氮
                    Tbx_X_P_BM.Text = X_P_BM.ToString("F3");

                    double X_P_BioP = 0.006 * C_COD_ZB;    //mg/L,生物法除磷量
                    Tbx_X_P_BioP.Text = X_P_BioP.ToString("F3");

                    double X_P_Fall = C_P_ZB - C_P_AN - X_P_BM - X_P_BioP;  //mg/L,需要沉析的磷酸盐
                    Tbx_X_P_Fall.Text = X_P_Fall.ToString("F3");

                    double Me_3plus = 1.5 * X_P_Fall / 31;   //mol/L,化学除磷药剂投加量
                    Tbx_Me_3plus.Text = Me_3plus.ToString("F3");

                    //5.2 除磷污泥产量
                    // 定义絮凝剂投加量
                    double X_P_Fall_Fe = 0;  //折合铁盐投加量
                    double X_P_Fall_Al = 0;  //折合铝盐投加量
                    if (Cbx_P_dos_name.SelectedIndex != -1)
                    {
                        switch (Cbx_P_dos_name.SelectedIndex)
                        {
                            case 0: // 投加铝盐
                                X_P_Fall_Fe = 0;  //折合铁盐投加量
                                X_P_Fall_Al = 27 * Me_3plus;     //mg/L,折合铝盐投加量
                                break;
                            case 1: // 投加铁盐
                                X_P_Fall_Fe = 55.8 * Me_3plus;   //mg/L,折合铁盐投加量
                                X_P_Fall_Al = 0;   //mg/L,折合铝盐投加量
                                break;
                        }
                    }

                    Tbx_X_P_Fall_Fe.Text = X_P_Fall_Fe.ToString("F3");
                    Tbx_X_P_Fall_Al.Text = X_P_Fall_Al.ToString("F3");

                    double US_d_P = Q_d_Knoz * (3 * X_P_BioP + 6.8 * X_P_Fall_Fe + 5.3 * X_P_Fall_Al) / 1000;   //化学除磷产泥量
                    Tbx_US_d_P.Text = US_d_P.ToString("F3");

                    //5.3 污泥产量
                    double US_d_r = US_d_C + US_d_P;    //kg/d,剩余污泥量
                    Tbx_US_d_r.Text = US_d_r.ToString("F3");

                    double M_TS_BB = t_TS_Bem * US_d_r;   //kg,生物段保持的污泥质量
                    Tbx_M_TS_BB.Text = M_TS_BB.ToString("F3");

                    double M_TS_D = V_D_over_V_BB * M_TS_BB;    //kg,缺氧池污泥量
                    Tbx_M_TS_D.Text = M_TS_D.ToString("F3");

                    double M_TS_aero = M_TS_BB - M_TS_D;    //kg,好氧池污泥量
                    Tbx_M_TS_aero.Text = M_TS_aero.ToString("F3");

                    double K_de = Q_d_Knoz * S_NO3_D / M_TS_D / 1000;   //kgN/kgSS·d,反硝化速率
                    Tbx_K_de.Text = K_de.ToString("F3");

                    double L_C = (C_COD_ZB - S_COD_AN) * Q_d_Knoz / 1000 / M_TS_aero;  //kgCOD/kgSS·d,好氧池COD负荷
                    Tbx_L_C.Text = L_C.ToString("F3");

                    double L_B = (C_BOD5_ZB - S_BOD5_AN) * Q_d_Knoz / 1000 / M_TS_aero;  //kgBOD/kgSS·d,好氧池BOD负荷
                    Tbx_L_B.Text = L_B.ToString("F3");

                    // 定义除磷及污泥产量系统
                    var phosremovaldata = new
                    {
                        selected_num = Cbx_P_dos_name.SelectedIndex,
                        M_TS_BB = Tbx_M_TS_BB.Text.ToString(),
                    };

                    // 将数据存储到 JSON 对象中
                    var newjsonData = new
                    {
                        Groupphosremoval = phosremovaldata,
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

        private void Cbx_P_dos_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Initializing_Json();
        }

        // 初始化窗口时，读取原有计算数值（如果有的话），否则默认 “甲醇”
        private void Get_Original_Data()
        {
            if (!File.Exists(filePath))
            {
                // 未找到 json 文件，先去完善基础数据
                MessageBox.Show("注意，先前往完善基础数据！！", "数据缺失", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                // 从文件中读取 JSON 字符串
                string jsonString = File.ReadAllText(filePath);

                // 将 JSON 字符串反序列化为对象
                var data = JsonSerializer.Deserialize<JsonNode>(jsonString);

                // 从data.json中获取Groupdatabase对象
                var phosremovaldata = data["Groupphosremoval"];
                if (phosremovaldata != null)
                {
                    double selected_num = ParseDouble(phosremovaldata["selected_num"].ToString());
                    switch (selected_num)
                    {
                        case 0:
                            Cbx_P_dos_name.SelectedIndex = 0;
                            break;
                        case 1:
                            Cbx_P_dos_name.SelectedIndex = 1;
                            break;
                        case 2:
                            Cbx_P_dos_name.SelectedIndex = 2;
                            break;
                    }
                }
                else
                {
                    Cbx_P_dos_name.SelectedIndex = 0;
                }
                Initializing_Json();
            }
        }
    }
}
