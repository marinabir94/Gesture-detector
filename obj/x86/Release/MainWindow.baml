   M S B A M L   `   `   `  ���� V  RContinuousGestureBasics-WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulls>clr-namespace:Microsoft.Samples.Kinect.ContinuousGestureBasics0Microsoft.Samples.Kinect.ContinuousGestureBasics  A    ;Microsoft.Samples.Kinect.ContinuousGestureBasics.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Plocal>clr-namespace:Microsoft.Samples.Kinect.ContinuousGestureBasics     -     � Title$  Continuous Gesture Basics��$	��680��$	��790�� � 	Resources    MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush��%]   &	        &	      &	 *     �� $ 	#ff6e6e6e���� $ 	#ff52318f���� $ 	#ff00BCF2��.��� $��	10 0 10 0q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��Images\Logo.png��$
��Left=�$
��Fill=�$��32��$��81��$��0 10 0 5q��� $��0��$��Right=�$��Continuous Gesture Basics��$��Bottom=�$��0 0 -1 0q�#��[  $��Segoe UI)�$��18(��� $��0��$��Images\Status.png��$
��None=�$��Center=�$��0 0 0 5q�� -    �Name$ contentGrid��$��1��$��Center=�$��Center=� � ColumnDefinitions �� $��1* ��� $��2* � �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��0��$��5q�.+��� $��Kinect Viewbox��$��20(�$��Segoe UI)�#��[ $��SemiBold"�$��0��$��5q�(� -    (�Name$ kinectBodyViewbox��.���� $��UniformToFill=����� *ImageSource+� -   $ gestureResultGrid��$��0��$��1��$
��Auto��$��5q�$	��Top=�����  �� 
TargetType" � �� Triggers �� 	 �� Binding	 �� *	IsTracked+
 �� Value
 True.a���  �� Property � �� �� Value
 ��d��� 	 �� *	IsTracked+	
 False.a���  � ��
 ����� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��18(�$��Segoe UI)�$��SemiBold"�$��1��$��5q����� *
TurnLeft+ �� StringFormat$ Turn Left: {0}���� $��18(�$��Segoe UI)�$��SemiBold"�$��2��$��5q����� *	TurnRight+$ Turn Right: {0}���� $��18(�$��Segoe UI)�$��SemiBold"�$��3��$��5q����� *KeepStraight+$ Keep Straight: {0}���� $��18(�$��Segoe UI)�$��SemiBold"�$��4��$��5q����� *SteerProgress+$ Steer Progress: {0}��� -   $ collisionResultGrid��$��0��$��2��$��5q�
���   �$	��Top=� �� $
��Auto ��� $
��Auto �.+��� $��18(�$��Segoe UI)�
�������$��0��$��5q����� *CollisionCount+$ Collisions: {0}���� $��18(�$��Segoe UI)�
�������$��1��$��5q����� *TimeSinceCollision+$# Last Collision: {0:mm\:ss\.f}���� $��2��$��3��$����Instructions: When your body appears in the Kinect Viewbox, you can control the ship. Pretend to hold a fake steering wheel in your hands. Turn the wheel left or right to rotate the ship. Open your hands to stop the ship.��$
��Wrap=�$��10q�#��[  $��Segoe UI)�$��14(�� -   $ 	spaceGrid��$��1��$��3��$��5q�}�. .+��� -    ��Name$ 
spaceImage��$��Images\space.png��$��UniformToFill=�}�. �� -   $��	statusBar��$��2��$��Stretch=�$��Bottom=�
�������#��[ .@��� ���� *
StatusText+