using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace DwaCalctor
{
    /// <summary>
    /// AboutInfo.xaml 的交互逻辑
    /// </summary>
    public partial class AboutInfo : UserControl
    {
        public AboutInfo()
        {
            InitializeComponent();
            Tbk_BackgroundContent.Text = "德国水、污水和废弃物处理协会（DWA）的前身是德国污水技术协会（ATV），" +
                               "致力于水、污水、废弃物的处理和可持续发展。" +
                               "其重要职能之一是编制和颁布相关技术标准和规范，" +
                               "在世界范围内都有广泛的影响。包括我国及许多亚洲地区的工程项目，" +
                               "都将DWA的规范作为重要的设计依据。" +
                               "我国有许多设计院和学者引用过原德国污水技术协会（ATV）的污水设计规范（ATV - A 131）。" +
                               "该规范有1991年及2000年两个版本，由于其应用便捷，" +
                               "计算方法清晰，至今仍有许多技术人员在使用。";

            Tbk_MainContent.Text = "这款软件将某位大佬的Python 脚本转化为直观易用的 WPF 项目，" +
                                "为用户提供高效的污水处理脱氮除磷相关计算。" +
                                "软件特别针对德国水、污水和废弃物处理协会（DWA）" +
                                "及其前身德国污水技术协会（ATV）的技术规范进行开发。本软件免费开源。";
        }

        // 添加 RequestNavigate 事件处理程序
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // 使用默认浏览器打开链接
            try
            {
                // Process.Start 需要 System.Diagnostics 命名空间
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = e.Uri.ToString(),
                    UseShellExecute = true // 确保使用系统的默认浏览器
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开链接: {ex.Message}");
            }

            e.Handled = true; // 标记事件已处理
        }
    }
}
