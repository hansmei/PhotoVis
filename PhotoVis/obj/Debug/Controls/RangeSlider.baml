   M S B A M L   `   `   `  ����  PhotoVis#    PhotoVis.Controls.RangeSlider   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5          >�Name$
  root��5       � 	Resources 5�         BackgroundBrush �� Color  HighlightedBrush  NormalBrush  SliderThumbTemplate �� 
TargetType F�Name �� Stretch �� 
StartPoint �� EndPoint � Offset	 � Color
 �� Triggers P� Property P� Value �� 
TargetName �� Value �� Property  simpleSlider J� SnapsToDevicePixels � RowDefinitions ��Name ]�Name ]� Thumb��%  &	        &	      &	 >     &	 ]     7  7  &	 V    7  �� 5�   
   $ #C222��62   �� 5�   
   $ #FFFF��63   �� 5�   
   $ #7FFF��6.   �� 5�   
   " n�66   .R�5�      F� $
 path��6   $��9��6!   $��18��6+   4����        �     	= �@B @KL р�  	= р�     	8��  $
 Fill=�6|   ��5�      �� 5�      $	 0,0&�6*   $	 0,1&�6;   .��5�      � $
 0.05Q�6'   $
	 #666��65   � 5�      $
 0.25Q�6'   $	 #404040��65   � 5�      $
 0.95Q�6'   $	 	#FF222222��65   5�      5�      
 5�      P� 5�       �@J�IsMouseOver . . �5�      �� $
 path��6/   8   � ��6A   5�      5�      �� 5�   
   " ��6/   .R�5�      ��  . #��z��60   #��z��6\   ._�5�      �  5�      �� 5�      $
��Auto �6(   �� 5�      $
��Auto �6(   #��z��66   �� 5�      $
��Auto �6(   .+�5�      �� $ PART_SelectionRange��6    ]� 5�      $ 
PART_Track��6   $��1��60    5�      n� 5�      8��  6$   5�      �� 5�      $��2��6   $��3��6*   ��5      �� 5     $	 1,0&�62   $	 0,0&�6A   .��5  "   � $		 Red��6/   $	 0.0Q�6;   � 5  "   $	 Yellow��6/   $	 0.5Q�6>   � 5  "   $	 	LimeGreen��6/   $	 1.0Q�6A   5     5     5     5	     5
     5     .��5     � $��Center=�6   $��20��6&   .+�5  
   �� $��0,1,0,0q�6   
���   �$��Center=�6?   $��1��6Z   $��5,0,5,0q�5     �� 5  
   -   5         ��Name$ LowerSlider��5     #��[ 5     $��0,0,10,0q�5      � Minimum 5     ��  �� ElementName$
 root�� �� Path$ Minimum� � Maximum 5     �� $
 root��$ Maximum� � Value 5     �� $
 root��$ 
LowerValue� �� Mode$ TwoWay=�5     �� 5  
   -   5        $ UpperSlider��5     #��[ 5     $��10,0,0,0q�5      5     �� $
 root��$ Minimum� 5     �� $
 root��$ Maximum� 5     �� $
 root��$ 
UpperValue�$ TwoWay=�5     5      5!     