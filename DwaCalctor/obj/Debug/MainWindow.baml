   M S B A M L   `   `   `  ����E  ADwaCalctor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:DwaCalctor
DwaCalctor  5   	       DwaCalctor.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:DwaCalctor     5   	   b ^SharpVectors.Converters.Wpf, Version=1.8.4.2, Culture=neutral, PublicKeyToken=b532964b8548be771svgc&http://sharpvectors.codeplex.com/svgc/  5   	   $	��600��5	   	   $	��800��5
   	     � WindowState$  Normal=�5   	    � WindowStartupLocation$ CenterScreen=�5   	    � Icon$ /Icons/Logo.png��5   	    �� BitmapScalingMode$ HighQuality=�6     � Title$# DWA-A一段式工艺计算器��5   	   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��135 �6   6-   �� 5      6!   .+�5   
   � $��0��6   
������ .+�5      �� -   5         ��Name$ 	StpColumn��5      ���	#FF42424E.+�5      �� $��Icons/Logo.png��6   $��60��60   $��60��6;   $
��Left=�6G   $��
40,25,0,35q�6b   $ HighQuality=�6v   �� 5      -   5         ��Name$ BtnDatabase��5      
������ $��35��6G   $��0q�6S   $��0,0,0,5q�6g   .��5      � .+�5      �� $��Icons/Database.png��6    $��20��6<   $��20��6H   $ HighQuality=�6S   �� 5      $��   基  础  数  据��6$   
�������$��Center=�6Q   $
��12.5(�6l   5      5      �� 5      -   5        $ BtnDesignFlow��5      
������ $��35��6I   $��0q�6U   $��0,0,0,5q�6i   .��5      � .+�5      �� $��Icons/DesignFlow.png��6    $��20��6>   $��20��6J   $ HighQuality=�6U   �� 5       $��   设  计  流  量��6$   
�������$��Center=�6Q   $
��12.5(�6l   5!      5"      �� 5#      -   5        $ BtnHydrobalance��5#      
������ $��35��6K   $��0q�6W   $��0,0,0,5q�6k   .��5$      � .+�5%      �� $��Icons/Hydrobalance.png��6    $��20��6@   $��20��6L   $ HighQuality=�6W   �� 5&      $��  进 出 水 平 衡��6$   
�������$��Center=�6O   $
��12.5(�6j   5'      5(      �� 5)      -   5        $ BtnNitribacteriaSludage��5)      
������ $��35��6S   $��0q�6_   $��0,0,0,5q�6s   .��5*      � .+�5+      �� $$��Icons/NitribacteriaSludage.png��6    $��20��6H   $��20��6T   $ HighQuality=�6_   �� 5,      $��  硝 化 菌 泥 龄��6$   
�������$��Center=�6O   $
��12.5(�6j   5-      5.      �� 5/      -   5        $ BtnDenitrivolratio��5/      
������ $��35��6N   $��0q�6Z   $��0,0,0,5q�6n   .��50      � .+�51      �� $��Icons/Denitrivolratio.png��6    $��20��6C   $��20��6O   $ HighQuality=�6Z   �� 52      $��  反硝化体积比��6$   
�������$��Center=�6L   $
��12.5(�6g   53      54      �� 55      -   5        $ BtnPhosphorusremoval��55      
������ $��35��6P   $��0q�6\   $��0,0,0,5q�6p   .��56      � .+�57      �� $!��Icons/Phosphorusremoval.png��6    $��20��6E   $��20��6Q   $ HighQuality=�6\   �� 58      $��  除             磷��6$   
�������$��Center=�6U   $
��12.5(�6p   59      5:      �� 5;      -   5        $ BtnBioreactorvolume��5;      
������ $��35��6O   $��0q�6[   $��0,0,0,5q�6o   .��5<      � .+�5=      �� $ ��Icons/Bioreactorvolume.png��6    $��20��6D   $��20��6P   $ HighQuality=�6[   �� 5>      $��  生 物 池 容 积��6$   
�������$��Center=�6O   $
��12.5(�6j   5?      5@      �� 5A      -	   5        $ BtnOxygendemand��5A      
������ $��35��6K   $��0q�6W   $��0,0,0,5q�6k   .��5B      � .+�5C      �� $��Icons/Oxygendemand.png��6    $��20��6@   $��20��6L   $ HighQuality=�6W   �� 5D      $��  需     氧     量��6$   
�������$��Center=�6S   $
��12.5(�6n   5E      5F      �� 5G      -
   5        $ BtnAboutinfo��5G      
������ $��35��6H   $��0q�6T   $��0,0,0,5q�6h   .��5H      � .+�5I      �� $��Icons/AboutInfo.png��6    $��20��6=   $��20��6I   $ HighQuality=�6T   �� 5J      $��  关              于��6$   
�������$��Center=�6V   $
��12.5(�6q   5K      5L      5M      5N      � 5O   
   -   5         �Name$ GridTopPanel��5O      $��1��6%   ���	#FF009688$��80��6L   $	��Top=�6X   .+�5P      �� -   5        	 ��Name$	 LblHomeTile��5P      
�������$
��Home��6A   $��25(�6P   $��Center=�6^   $��Center=�6y   $��Arial Black)�6�   5Q      � 5R   
   -   5        $ DesktopGrid��5R      $��1��6$   $��0,80,0,0q�64   .+�5S      �� $��UniformToFill=�6=   
 �� StretchDirection$

 Both=�5T      $ HighQuality=�6-   ��5S      /  )SharpVectors.Converters.SvgImageExtension  *Icons/cover.svg+5T   -   5U      5V      5W      