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
using System.Diagnostics.Eventing.Reader;


namespace DwaCalctor
{
    /// <summary>
    /// Denitrivolratio.xaml 的交互逻辑
    /// </summary>
    public partial class Denitrivolratio : UserControl
    {
        //构造 json 文件路径
        public string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

        public double C_COD_dos_f = 0;   //外加碳源化学需氧量
        public double V_D_over_V_BB_f = 0.2;  //缺氧池/曝气池体积比例
        public double x_f = 0;  //耗氧量和供氧量平衡，一般为0.88，尽量接近1
        public double t_TS_Bem_f = 0;  //设计污泥泥龄
        public double X_COD_BM_f = 0;    //生物体中的COD
        public double X_COD_inert_BM_f = 0;   //剩余惰性固体 
        public double US_d_C_f = 0;  //污泥产量
        public double S_NO3_AN_f = 0;  //出水硝态氮
        public double X_orngN_BM_f = 0;  //形成活性污泥的氮
        public double X_orgN_inert_f = 0;  //与惰性颗粒结合的氮
        public double S_NO3_D_f = 0;   //每日平均反硝化的硝态氮浓度
        public double OV_C_f = 0;  //碳降解的总需氧量
        public double OV_C_la_vorg_f = 0;  //反硝化区易降解及外加碳源需氧量
        public double OV_C_D_f = 0;  //反硝化总需氧量

        public double[] result = new double[] { };

        public Denitrivolratio()
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

        // 使用多线程对运算过程多次迭代，加快运算速度


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


                if (database != null && inflowdata != null && 
                    outflowdata != null && flowdata != null &&
                    nitribacterdata != null && hydrobalancedata != null)
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

                    //四、反硝化体积比例VD/VBB
                    double Y_COD_dos = 0; 

                    // 投加碳源类型
                    if (Cbx_COD_dos_name.SelectedIndex != -1)
                    {
                        switch (Cbx_COD_dos_name.SelectedIndex)
                        {
                            case 0: // 选择甲醇
                                Y_COD_dos = 0.45;
                                break;
                            case 1: // 选择乙醇
                                Y_COD_dos = 0.42;
                                break;
                            case 2: // 选择醋酸
                                Y_COD_dos = 0.42;
                                break;
                        }
                    }

                    double F_T = Math.Pow(1.072, (T_C - 15));  //内源呼吸的衰减系数
                    Tbx_F_T.Text = F_T.ToString("F3");  //内源呼吸的衰减系数

                    while (x_f < 1)
                    {
                        if (x_f < 0)
                        {
                            // 基础数据有问题时，计算结果不符合逻辑，提示返回修改基础数据
                            MessageBox.Show("耗氧供氧量平衡系数为负值，不符合逻辑（一般为0.88, 尽量接近1），请返回修改基础数据！！", "数据错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                        }

                        //4.1污泥产量的计算
                        t_TS_Bem_f = t_TS_aerob_Bem / (1 - V_D_over_V_BB_f); //设计污泥泥龄
                        X_COD_BM_f = (C_COD_abb_ZB * Y_COD_abb + C_COD_dos_f * Y_COD_dos) / (1 + b * t_TS_Bem_f * F_T);    //生物体中的COD
                        X_COD_inert_BM_f = 0.2 * X_COD_BM_f * t_TS_Bem_f * b * F_T;  //剩余惰性固体
                        US_d_C_f = Q_d_Knoz * (X_COD_inert_ZB / 1.33 + (X_COD_BM_f + X_COD_inert_ZB) / (0.93 * 1.42) + f_B * X_TS_ZB) / 1000;  //污泥产量

                        //4.2反硝化硝态氮浓度计算
                        S_NO3_AN_f = 0.7 * S_anorgN_UW; //出水硝态氮
                        X_orngN_BM_f = 0.07 * X_COD_BM_f; //形成活性污泥的氮
                        X_orgN_inert_f = 0.03 * (X_COD_inert_BM_f + X_COD_inert_ZB);  //与惰性颗粒结合的氮
                        S_NO3_D_f = C_TN_ZB - S_NO3_AN_f - S_orgN_AN - S_NH4_AN - X_orngN_BM_f - X_orgN_inert_f;  //每日平均反硝化的硝态氮浓度

                        //4.3碳降解的需氧量
                        OV_C_f = C_COD_abb_ZB + C_COD_dos_f - X_COD_BM_f - X_COD_inert_BM_f;  //碳降解的总需氧量
                        OV_C_la_vorg_f = f_COD * C_COD_abb_ZB * (1 - Y_COD_abb) + C_COD_dos_f * (1 - Y_COD_dos);  //反硝化区易降解及外加碳源需氧量
                        OV_C_D_f = 0.75 * (OV_C_la_vorg_f + (OV_C_f - OV_C_la_vorg_f) * Math.Pow(V_D_over_V_BB_f, 0.68));  //反硝化区总需氧量

                        //4.4耗氧量和供氧量平衡
                        x_f = OV_C_D_f / 2.86 / S_NO3_D_f;

                        if (V_D_over_V_BB_f < 0.6 && x_f < 1)
                        {
                            V_D_over_V_BB_f += 0.01;
                        }
                        if (V_D_over_V_BB_f >= 0.6 && x_f < 1)
                        {
                            V_D_over_V_BB_f = 0.6;
                            C_COD_dos_f += 0.01;
                        }
                        result = new double[] {
                            C_COD_dos_f, V_D_over_V_BB_f, x_f,
                            t_TS_Bem_f, X_COD_BM_f,
                            X_COD_inert_BM_f, US_d_C_f,
                            S_NO3_AN_f,X_orngN_BM_f,
                            X_orgN_inert_f, S_NO3_D_f, OV_C_f,
                            OV_C_la_vorg_f, OV_C_D_f
                        };
                    }

                    double C_COD_dos = result[0];   //mg/L，外加碳源化学需氧量
                    Tbx_C_COD_dos.Text = C_COD_dos.ToString("F3");

                    double V_D_over_V_BB = result[1];   //缺氧池/曝气池体积比例
                    Tbx_V_D_over_V_BB.Text = V_D_over_V_BB.ToString("F3");

                    double x = result[2];   //耗氧量和供氧量平衡
                    Tbx_x.Text = x.ToString("F3");

                    double t_TS_Bem = result[3];    //设计污泥泥龄
                    Tbx_t_TS_Bem.Text = t_TS_Bem.ToString("F3");

                    double X_COD_BM = result[4];    //生物体中的COD
                    Tbx_X_COD_BM.Text = X_COD_BM.ToString("F3");

                    double X_COD_inert_BM = result[5];  //剩余惰性固体
                    Tbx_X_COD_inert_BM.Text = X_COD_inert_BM.ToString("F3");

                    double US_d_C = result[6];  //污泥产量
                    Tbx_US_d_C.Text = US_d_C.ToString("F3");

                    double S_NO3_AN = result[7];    //出水硝态氮
                    Tbx_S_NO3_AN.Text = S_NO3_AN.ToString("F3");

                    double X_orngN_BM = result[8];  //形成活性污泥的氮
                    Tbx_X_orngN_BM.Text = X_orngN_BM.ToString("F3");

                    double X_orgN_inert = result[9];    //与惰性颗粒结合的氮
                    Tbx_X_orgN_inert.Text = X_orgN_inert.ToString("F3");

                    double S_NO3_D = result[10];    //每日平均反硝化的硝态氮浓度
                    Tbx_S_NO3_D.Text = S_NO3_D.ToString("F3");

                    double OV_C = result[11];   //碳降解的总需氧量
                    Tbx_OV_C.Text = OV_C.ToString("F3");

                    double OV_C_la_vorg = result[12];   //反硝化区易降解及外加碳源需氧量
                    Tbx_OV_C_la_vorg.Text = OV_C_la_vorg.ToString("F3");

                    double OV_C_D = result[13];     //反硝化总需氧量
                    Tbx_OV_C_D.Text = OV_C_D.ToString("F3");

                    double T_TS_D_Bem = t_TS_Bem - t_TS_aerob_Bem;  //d,反硝化菌泥龄
                    Tbx_T_TS_D_Bem.Text = T_TS_D_Bem.ToString("F3");

                    // 定义反硝化参数值
                    var denitrialdata = new
                    {
                        selected_num = Cbx_COD_dos_name.SelectedIndex,
                        US_d_C = Tbx_US_d_C.Text.ToString(),
                        t_TS_Bem = Tbx_t_TS_Bem.Text.ToString(),
                        V_D_over_V_BB = Tbx_V_D_over_V_BB.Text.ToString(),
                        S_NO3_AN = Tbx_S_NO3_AN.Text.ToString(),
                        S_NO3_D = Tbx_S_NO3_D.Text.ToString(),
                        OV_C = Tbx_OV_C.Text.ToString(),
                    };

                    // 将数据存储到 JSON 对象中
                    var newjsonData = new
                    {
                        Groupdenitrial = denitrialdata,
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
                var denitrialdata = data["Groupdenitrial"];
                if (denitrialdata != null)
                {
                    double selected_num = ParseDouble(denitrialdata["selected_num"].ToString());
                    switch (selected_num)
                    {
                        case 0:
                            Cbx_COD_dos_name.SelectedIndex = 0;
                            break;
                        case 1:
                            Cbx_COD_dos_name.SelectedIndex = 1;
                            break;
                        case 2:
                            Cbx_COD_dos_name.SelectedIndex = 2;
                            break;
                    }
                }
                else
                {
                    Cbx_COD_dos_name.SelectedIndex = 0;
                }
                Initializing_Json();
                
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Initializing_Json();
        }
    }
}
