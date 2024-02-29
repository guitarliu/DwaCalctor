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
        }

        private void Tbx_Q_d_Knoz_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 一、设计流量
            try
            {
                double Q_d_Knoz = double.Parse(Tbx_Q_d_Knoz.Text.Trim());
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

                double Q_d_max = Q_d_Knoz * double.Parse(Tbx_Kz.Text);
                Tbx_Q_d_max.Text = Q_d_max.ToString("F3");
                Tbx_Q_h_max.Text = (Q_d_max / 24).ToString("F3");
            }
            catch { 
                Tbx_Q_h_Knoz.Text = string.Empty;
                Tbx_Kz.Text = string.Empty;
                Tbx_Q_d_max.Text = string.Empty;
                Tbx_Q_h_max.Text = string.Empty;
            }
        }
    }
}
