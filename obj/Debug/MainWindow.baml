   M S B A M L   `   `   `  ����G  CTextComparer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TextComparerTextComparer  5   	       TextComparer.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:TextComparer     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $
��145* �6   �� 5      $	��53* �6   .+�5   
   �� $��Сравнить��6   $��10,273,127,104q�6G   $��1��6_   ��6$   �� *CompareCommand+6_   �� 5   
   $��10,10,10,151q�5      ��5      �� *	TextsList+ �� SelectedItem 5      �� *SelectedText+��5      �� 5      .R�5      �� $��5q�6!   .+�5      �� $��18(�6$   ��62   ��  �� Path$ Title�5      5      5      �� 5   
   $��23��6   $��
10,31,10,0q�6    �� TextWrapping$
 Wrap=�62   $	��Top=�6�   $��1��6�    � DataContext 6F   �� *SelectedText+��6k   �� *Title+ �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5   
   $��
10,10,10,0q�6   $
��Wrap=�6(   $#��Выбранный Текст��6<   $	��Top=�6S   $��1��6k   �� 5   
   $��+��6   $��10,273,495,0q�6=   $	��Top=�6S   $��42��6k   ��6   �� *OpenCommand+6k   �� 5   
   $��-��6   $��90,273,415,0q�6?   $	��Top=�6U   $��42��6m   ��6   �� *RemoveCommand+6m   5       5!      