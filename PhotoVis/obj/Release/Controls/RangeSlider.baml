   M S B A M L   `   `   `  ����   PhotoVis#    PhotoVis.Controls.RangeSlider   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -     >�Name$
  root�� � 	Resources    BackgroundBrush �� Color  HighlightedBrush  NormalBrush  SliderThumbTemplate �� 
TargetType F�Name �� Stretch �� 
StartPoint �� EndPoint � Offset	 � Color
 �� Triggers P� Property P� Value �� 
TargetName �� Value �� Property  simpleSlider J� SnapsToDevicePixels � RowDefinitions ��Name ]�Name ]� Thumb��%�  &	        &	      &	 "     &	 3     7  7  &	 c    7  �� $ #C222���� $ #FFFF���� $ #7FFF���� " n�.R�F� $
 path��$��9��$��18��4����        �     	= �@B @KL р�  	= р�     	8��  $
 Fill=����� $	 0,0&�$	 0,1&�.��� $
 0.05Q�$
	 #666��� $
 0.25Q�$	 #404040��� $
 0.95Q�$	 	#FF222222��
 P�  �@J�IsMouseOver . . ��� $
 path��8   � ���� " ��.R���  . #��z��#��z��._��  �� $
��Auto ��� $
��Auto �#��z���� $
��Auto �.+��� $ PART_SelectionRange��]� $ 
PART_Track��$��1�� n� 8��  �� $��2��$��3������ $	 1,0&�$	 0,0&�.��� $		 Red��$	 0.0Q�� $	 Yellow��$	 0.5Q�� $	 	LimeGreen��$	 1.0Q�.��� $��Center=�$��20��.+��� $��0,1,0,0q�
���   �$��Center=�$��1��$��5,0,5,0q��� -    ��Name$ LowerSlider��#��[ $��0,0,10,0q� � Minimum ��  �� ElementName$
 root�� �� Path$ Minimum� � Maximum �� $
 root��$ Maximum� � Value �� $
 root��$ 
LowerValue� �� Mode$ TwoWay=��� -   $ UpperSlider��#��[ $��10,0,0,0q� �� $
 root��$ Minimum� �� $
 root��$ Maximum� �� $
 root��$ 
UpperValue�$ TwoWay=�