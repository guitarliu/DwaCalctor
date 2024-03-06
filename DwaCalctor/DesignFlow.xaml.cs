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
using static DwaCalctor.Database;
using Word = Microsoft.Office.Interop.Word;
using Table = Microsoft.Office.Interop.Word.Table;
using System.Text.Json;
using Path = System.IO.Path;
using System.Text.Json.Nodes;

namespace DwaCalctor
{
    /// <summary>
    /// DesignFlow.xaml 的交互逻辑
    /// </summary>
    public partial class DesignFlow : UserControl
    {
        public DesignFlow()
        {
            InitializeComponent();
            Get_FlowData();
        }

        // 解析文本框内容为 double 类型的方法
        private double ParseDouble(string text)
        {
            double result = 0.0;
            double.TryParse(text, out result);
            return result;
        }
        private void Tbx_Q_d_Knoz_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 一、设计流量
            try
            {
                double Q_d_Knoz = ParseDouble(Tbx_Q_d_Knoz.Text.Trim());
                double Q_h_Knoz = Q_d_Knoz / 24;
                Tbx_Q_h_Knoz.Text = Q_h_Knoz.ToString("F3");
                if (Q_h_Knoz <= 13)
                {
                    Tbx_Kz.Text = "2.7";
                }
                else if (Q_h_Knoz >= 2600)
                {
                    Tbx_Kz.Text = "1.5";
                }
                else
                {
                    Tbx_Kz.Text = (3.5778 * Math.Pow(Q_h_Knoz, -0.112)).ToString("F3");
                }

                double Q_d_max = Q_d_Knoz * ParseDouble(Tbx_Kz.Text);
                Tbx_Q_d_max.Text = Q_d_max.ToString("F3");
                Tbx_Q_h_max.Text = (Q_d_max / 24).ToString("F3");
            }
            catch
            {
                Tbx_Q_h_Knoz.Text = string.Empty;
                Tbx_Kz.Text = string.Empty;
                Tbx_Q_d_max.Text = string.Empty;
                Tbx_Q_h_max.Text = string.Empty;
            }
            finally 
            {
                // 定义设计流量数据
                var flowdata = new
                {
                    Q_d_Knoz = ParseDouble(Tbx_Q_d_Knoz.Text),
                    Q_h_Knoz = ParseDouble(Tbx_Q_h_Knoz.Text),
                    Kz = ParseDouble(Tbx_Kz.Text),
                    Q_d_max = ParseDouble(Tbx_Q_d_max.Text),
                    Q_h_max = ParseDouble(Tbx_Q_h_max.Text)
                };

                // 将数据存储到 JSON 对象中
                var jsonData = new
                {
                    Groupflowdata = flowdata,
                };

                // 将 JSON 对象序列化为 JSON 字符串
                string jsonString = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions
                {
                    WriteIndented = true // 设置为true，使输出的JSON格式化
                });

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
                // 将 JSON 字符串写入到文件
                File.WriteAllText(filePath, jsonString);
            }
        }

        /// <summary>
        /// 基础数据窗口初始化时即读取现有数据，以便用户查看；
        /// </summary>
        private void Get_FlowData()
        {
            try
            {
                // 构造文件路径
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

                // 从文件中读取 JSON 字符串
                string jsonString = File.ReadAllText(filePath);

                // 将 JSON 字符串反序列化为对象
                var data = JsonSerializer.Deserialize<JsonNode>(jsonString);

                // 从data.json中获取Groupflowdata对象
                var flowdata = JsonNode.Parse(data.ToString());

                Tbx_Q_d_Knoz.Text = flowdata["Groupflowdata"]["Q_d_Knoz"].ToString();
                Tbx_Q_h_Knoz.Text = flowdata["Groupflowdata"]["Q_h_Knoz"].ToString();
                Tbx_Kz.Text = flowdata["Groupflowdata"]["Kz"].ToString();
                Tbx_Q_d_max.Text = flowdata["Groupflowdata"]["Q_d_max"].ToString();
                Tbx_Q_h_max.Text = flowdata["Groupflowdata"]["Q_h_max"].ToString();
            }
            catch (Exception ex)
            {
                // throw none
                MessageBox.Show(ex.Message);
            }
        }
    }
}
