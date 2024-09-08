# DwaCalctor

DwaCalctor 是一个基于 WPF 的应用程序，实现了 DWA-A 131（2016）一段式活性污泥法的设计计算。它为污水处理厂设计人员提供了一个用户友好的界面，可以根据更新的德国标准进行计算。

![软件图标](https://github.com/guitarliu/DwaCalctor/blob/main/DwaCalctor/Icons/Logo.png)
![思维导图](https://github.com/guitarliu/DwaCalctor/blob/main/mindmap.svg)

## 功能特点

- 实现 2016 版 DWA-A 131 设计计算
- 用户友好的 WPF 界面
- 数据存储和管理
- 计算脱氮除磷的关键参数
- 反硝化体积比的迭代计算
- 自动确定外加碳源需求

## 软件截图

![运行截图1](https://github.com/guitarliu/DwaCalctor/blob/main/screen_shot1.png)
![运行截图2](https://github.com/guitarliu/DwaCalctor/blob/main/screen_shot2.png)

## 背景

本软件基于德国水、污水和废弃物处理协会（DWA）发布的 DWA-A 131（2016）指南。它包含了与之前 ATV-A 131 标准相比的重要更新，包括：

- 使用 COD 而不是 BOD5 作为主要设计参数
- 更新的硝化反应系数
- 优化的设计流程和固体物质平衡
- 扩大的反硝化体积比范围（0.2-0.6）
- 改进的外加碳源需求计算

## 使用方法

1. 下载并安装最新版本的 DwaCalctor。
2. 运行程序，先输入污水处理基础数据。
3. 切换不同处理过程，自动完成后续步骤计算。
4. 数据实时存储到json文件中，实时自动加载上一次存储数据。

## 计算流程

软件遵循以下主要步骤：

1. 工艺选择和初始参数估算
2. 确定硝化反应系数
3. 计算好氧污泥泥龄
4. 反硝化体积比的迭代计算
5. 污泥产量计算
6. 氮平衡和反硝化需求计算
7. 需氧量计算
8. 确定外加碳源需求（如果需要）

## 参考资料

- [2016版DWA-A 131设计规范简介](https://mp.weixin.qq.com/s/5PEM41xmiHEKbiKVIAU4fA)

## 致谢

本项目是基于参考文章中提供的原始 Python 脚本的 WPF 实现。在这里对前辈表示感谢！。

## 作者信息

- 博客：[主页](https://spacetools.top)
- 公众号：SpaceTools

## 贡献

我们欢迎并感谢任何形式的贡献。如果您想为 DwaCalctor 做出贡献，请遵循以下步骤：

1. Fork 本仓库
2. 创建您的特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交您的更改 (`git commit -m 'Add some AmazingFeature'`)
4. 将您的更改推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启一个 Pull Request

在提交 Pull Request 之前，请确保您的代码符合编码规范，并且所有测试都已通过。

## 许可证

本项目采用 MIT 许可证。详情请见 [LICENSE](LICENSE) 文件。

## 项目统计

[![Star History Chart](https://api.star-history.com/svg?repos=yourusername/DwaCalctor&type=Date)](https://star-history.com/#yourusername/DwaCalctor&Date)

