   M S B A M L   `   `   `  ���� d  `Microsoft.Maps.MapControl.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tRclr-namespace:Microsoft.Maps.MapControl.WPF;assembly=Microsoft.Maps.MapControl.WPFMicrosoft.Maps.MapControl.WPF  C ?PhotoVis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:PhotoVis.ControlsPhotoVis.Controls    PhotoVis.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     `mRclr-namespace:Microsoft.Maps.MapControl.WPF;assembly=Microsoft.Maps.MapControl.WPF     .txclr-namespace:PhotoVis.Controls       � Title$  PhotoVis��$	��600��$
��1040�� � 	Resources    GreenPin �� CreateOptions �� CacheOption �� 	UriSource  	GreenPin2  ToBeDeletedPushpinTemplate �� 
TargetType+   %Microsoft.Maps.MapControl.WPF.Pushpin,   &Microsoft.Maps.MapControl.WPF.MapLayer   PositionOriginB   <Microsoft.Maps.MapControl.WPF.Design.PositionOriginConverter �� ImageSource  PushpinControlTemplate �� Stretch	 J� SnapsToDevicePixels
 � ToolTip J� IsHitTestVisible J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   
BlurRadius   Color   ShadowDepth��%�  &	        &	 u     &	 �     &	     �� $ IgnoreImageCache=�$ OnLoad=�$K EC:\dev\git\PhotoVis\C#\BingMapsWPF_Clustering\Resources\green_pin.png?��� $ IgnoreImageCache=�$ OnLoad=�$1 +/PhotoVis;component/Resources/green_pin.png?��� "  .R�� .+��� $��48��$��48��$ BottomCenter ���� # �   �� "  .R�� $��45��$��30��$��0,0,0,0q�.+�F� ���	#E40E9700���#C222$��2��$ Uniform=�	 . �����@KL ��[ � 	= ��[ ���- @T� р� ��[ �@B ��[ �    ��[ @KL �    @B �        �@KL     Ҁ�[     ��[ р�[ @KL Ҁ�[ ��[ @KL ��[ 	
 �� *Title+�� ���#FFF$��Consolas)�$��18(�$��0,0,0,15q�$��Center=�$��Center=� .  $��X��   $ 3Q�$ #C222��$ 2Q�.��]� ��. .+�o� $	��Top=�$��20��.@�m� $��_File��.@�m� $��_Index assignments��m� -   $��_Debug tools���� m� $��_Exit��� .+�� -    �Name$ Overlay�� K� ZIndex$
 1000��$y�	Collapsed=� � RowDefinitions �� $��20 ��� �� $��20 ��� $��20 � � ColumnDefinitions �� $	��60* ��� $	��40* �����  �� Color$ Black�� �� Opacity$	 .85Q�.+��� -    ��Name$ currentImage��$��1��$��0��$��Center=�$��Center=�$	��5,5q�$��Uniform=��� -    ��Name$ imageCounter��$��2��$��0��$��Center=�$��Bottom=�$	��0/0��$��14(�
�������� $��1��$��1�� �� $	��50* ��� $	��50* �.+�'   !Microsoft.Maps.MapControl.WPF.Map  -     Name$ InnerMap��$��0��$��0��+   %Microsoft.Maps.MapControl.WPF.MapCore   Mode;   5Microsoft.Maps.MapControl.WPF.Design.MapModeConverter$ Aerial    CredentialsProviderT   NMicrosoft.Maps.MapControl.WPF.Design.ApplicationIdCredentialsProviderConverter$F @AtDFmbUgIwetT7gjLyhAWl6KDFCy4C5V3fgmchMfozGt1qS1wfiu3N67j-Hm6GyT $	��5,5q��� $
��Left=�$��	10,10,0,0q�$��1��$	��Top=�.+��� -   $ FileName��$
��Wrap=�$�� ��
��������� -   $ ImageSaveDate��$
��Wrap=�$�� ��
��������� -   $ 
ImageTaken��$
��Wrap=�$�� ��
��������� -	   $��Show on disk��$
��0,10q��  �� �� $	��100 �.+�  -
   $��MyMap��$
 Road $F @AtDFmbUgIwetT7gjLyhAWl6KDFCy4C5V3fgmchMfozGt1qS1wfiu3N67j-Hm6GyT    Children.   -     Name$ NewPolygonLayer��  -   $ PushpinLayer���� $	��150��$��Right=�$��10q�.+��� -   $��Aerial mode���� -   $��	Road mode��$
��0,10q��� -   $��Hybrid mode���� -   $ ��Use Point Based Clustering��$
��0,10q��� -   $��Use Grid Based Clustering����  �� Padding$ 10q�$��10q�$
��Wrap=�$+��%Shift+Click and drag to select images��$	��220��$	��Top=�$
��Left=�$��40��
���  ��
������� .  �� -    ��Name$ debugContainer��$y�	Collapsed=�
���   �$	��Top=�$
��Left=� J� Opacity$	 0.8Q�
�������$��2q�  �� CornerRadius$  5��._��� -   ! ��Name$! eventsPanel�� .  " �� Left$" 0��
# �� Top$# 0���� $��1��.+��� $��Status:��(	  "PhotoVis.Controls.ScrollingTextBox	  -   $ 	 Name$$ 	StatusTbx��% �� 
IsReadOnly% . $��80��