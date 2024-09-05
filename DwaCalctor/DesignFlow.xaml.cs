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
        // 构造 json 文件路径
        public string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        public DesignFlow()
        {
            InitializeComponent();
            if (!File.Exists(filePath))
            {
                Initializing_Json();
            }
            Get_FlowData();

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
                    textBox.TextChanged += Tbx_Q_d_Knoz_TextChanged;
                }
                EnableUpdateDatabaseEvent(child); // 递归遍历子控件

            }
        }

        private void Initializing_Json()
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

            // 定义设计流量数据
            var flowdata = new
            {
                Q_d_Knoz = ParseDouble(Tbx_Q_d_Knoz.Text.Trim()),
                Q_h_Knoz = ParseDouble(Tbx_Q_h_Knoz.Text.Trim()),
                Kz = ParseDouble(Tbx_Kz.Text.Trim()),
                Q_d_max = ParseDouble(Tbx_Q_d_max.Text.Trim()),
                Q_h_max = ParseDouble(Tbx_Q_h_max.Text.Trim())
            };

            // 将数据存储到 JSON 对象中
            var newjsonData = new
            {
                Groupflowdata = flowdata,
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
                JsonNode oldData = JsonSerializer.Deserialize<JsonNode>(oldjsonString);

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

        // 解析文本框内容为 double 类型的方法
        private double ParseDouble(string text)
        {
            double result = 0.0;
            double.TryParse(text, out result);
            return result;
        }
        private void Tbx_Q_d_Knoz_TextChanged(object sender, TextChangedEventArgs e)
        {
            Initializing_Json();
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

                // 从data.json中获取Groupflowdata对象
                var flowdata = data["Groupflowdata"];

                if (flowdata != null)
                {
                    Tbx_Q_d_Knoz.Text = flowdata["Q_d_Knoz"].ToString();
                    Tbx_Q_h_Knoz.Text = flowdata["Q_h_Knoz"].ToString();
                    Tbx_Kz.Text = flowdata["Kz"].ToString();
                    Tbx_Q_d_max.Text = flowdata["Q_d_max"].ToString();
                    Tbx_Q_h_max.Text = flowdata["Q_h_max"].ToString();
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
    }
}
