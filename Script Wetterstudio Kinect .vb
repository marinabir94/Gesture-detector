'DEFINITION EACH CREATED ELEMENT

'Continuous gestures
dim ZoomIn as Double
dim ZoomOut as Double 
dim ScrollUp as Double
dim ScrollDown as Double
dim ScrollRight as Double
dim ScrollLeft as Double


'Windows
dim Starting as Director
dim audioFeedback1 as Director
dim audioFeedback2 as Director

'Buttons
dim animCube1 as Director
dim animCube2 as Director
dim animCube3 as Director
dim menu as Director

'Displays
dim Display1 as Container
dim Display2 as Container
dim Display3 as Container
dim Display4 as Container

structure display
	showing as Director
	hidding as Director
	visible as Boolean
end structure

dim displays as Array[display]
dim tmpDisplay as display

Sub OnInit()

	'Start (0)
	tmpDisplay.showing = stage.findDirector("Display_1_raus")
	tmpDisplay.hidding = stage.findDirector("Display_1")
	tmpDisplay.visible = true
	displays.push(tmpDisplay)

	'Temperature (1)
	tmpDisplay.showing = stage.findDirector("Display_2_raus")
	tmpDisplay.hidding = stage.findDirector("Display_2")
	tmpDisplay.visible = true
	displays.push(tmpDisplay)
	
	
	'Morgen (2)
	tmpDisplay.showing = stage.findDirector("Display_3_raus")
	tmpDisplay.hidding = stage.findDirector("Display_3")
	tmpDisplay.visible = true
	displays.push(tmpDisplay)
	
	
	'Video (3)
	tmpDisplay.showing = stage.findDirector("Display_4_raus")
	tmpDisplay.hidding = stage.findDirector("Display_4")
	tmpDisplay.visible = true
	displays.push(tmpDisplay)
	resetDisplays()

end sub

'sendOSCplugin = this.GetFunctionPluginInstance("SendOSC")

	'video initial state:
	Display4.texture.mapscaling.x = 3.65
	Display4.texture.mapscaling.y = 3.65
	Display4.texture.mapposition.x = 6.5
	Display4.texture.mapposition.y = 9.5
	
	


'Showing displays
dim showingDisplay_1 as Director
dim showingDisplay_2 as Director
dim showingDisplay_3 as Director
dim showingDisplay_4 as Director

'Hidding displays
dim hiddingDisplay_1 as Director
dim hiddingDisplay_2 as Director
dim hiddingDisplay_3 as Director
dim hiddingDisplay_4 as Director

'Continuous Gestures
ZoomIn = 1
ZoomOut = 1
ScrollUp = 1
ScrollDown = 1
ScrollRight = 1
ScrollLeft = 1

'DESCRIPTION OF EACH CREATED ELEMENT

'Windows 
Starting= stage.findDirector ("Fensteranimation")
audioFeedback1 = stage.findDirector ("AudioClip1")
audioFeedback2 = stage.findDirector ("AudioClip2")

'Buttons
animCube1 = stage.findDirector ("Cube1")
animCube2 = stage.findDirector ("Cube2")
animCube3 = stage.findDirector ("Cube3")
menu = stage.findDirector ("Menue")

'Displays
Display1 = scene.FindContainer ("Display1")
Display2 = scene.FindContainer ("Display2")
Display3 = scene.FindContainer ("Display3")
Display4 = scene.FindContainer ("Display4")



'Showing displays
showingDisplay_1 = stage.findDirector ("Display_1_raus") 
showingDisplay_2 = stage.findDirector ("Display_2_raus") 
showingDisplay_3 = stage.findDirector ("Display_3_raus")
showingDisplay_4 = stage.findDirector ("Display_4_raus")  

'Hidding Displays 
hiddingDisplay_1 = stage.findDirector ("Display_1") 
hiddingDisplay_2 = stage.findDirector ("Display_2") 
hiddingDisplay_3 = stage.findDirector ("Display_3") 
hiddingDisplay_4 = stage.findDirector ("Display_4") 

sub resetDisplays()
	for i=0 to displays.size-1
		if (displays[i].visible and (displays[i].hidding.IsAnimationRunning() == false))then
			displays[i].hidding.startAnimation
			displays[i].visible=false
		end if
	next
end sub

sub showDisplay(index as Integer)
	if (displays[index].showing.IsAnimationRunning() == false) then
		println index
		resetDisplays()
		displays[index].showing.startAnimation
		displays[index].visible=true
	end if
end sub


	

'IMPLEMENTATION OF ECAH OSCMeESSAGE: 

'Start gesture activates the FensterAnimation and Menue Directories 
sub OnOSCMessage_Start (startApplication as Double)

	if (Starting.IsAnimationRunning() == false) then
	
		'sendOSCplugin.PushButton("send")
		audioFeedback2.startAnimation
		Starting.startAnimation
		showDisplay(0)
		menu.startAnimation
		
		println ("Let's start")
		
				
	end if
	
end sub 


'PointingUp gesture activates the Cube1 and Display_Raus_2 Director 

sub OnOSCMessage_PointUp (TemperatureButton as Double)
 
	if (animCube1.IsAnimationRunning() == false) then 
		'sendOSCplugin.PushButton("send")
		animCube1.startAnimation 
		audioFeedback1.startAnimation
		println ("PointUp")

		'Temperature --> Display_2
		showDisplay(1)
	end if 
	
end sub 



'PointingMiddle gesture activates the Cube2 and Display1_Raus Director 

sub OnOSCMessage_PointMiddle (VideoButton as Double) 

	if (animCube2.IsAnimationRunning () == false) 	then
		'sendOSCplugin.PushButton("send")	
		animCube2.startAnimation 
		audioFeedback1.startAnimation
		println ("PointMiddle")
'			
'		Video ---> Display_4
        showDisplay(3) 

		'video initial state:
		Display4.texture.mapscaling.x = 3.65
		Display4.texture.mapscaling.y = 3.65
		Display4.texture.mapposition.x = 6.5
		Display4.texture.mapposition.y = 9.5
	end if 
	
end sub 

'PointingDown gesture activates the Cube3 and Display_3 Director 

sub OnOSCMessage_PointDown (MorgenButton as Double) 

	if (animCube3.IsAnimationRunning() == false) then 
		'sendOSCplugin.PushButton("send")
		animCube3.startAnimation 
		audioFeedback1.startAnimation 
		println ("PointDown")
		
'		Morgen ---> Display_3
        showDisplay(2)
		

	end if  

end sub 



sub OnOSCMessage_ZoomProgress (ZoomValue as Double)

	if (displays[3].visible=true) then
	Display4.texture.mapScaling.x = Display4.texture.mapScaling.y
	println ("zoom")

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' 		
		if ((Display4.texture.mapScaling.x <= 8) and (Display4.texture.mapScaling.y <= 8) and (ZoomIn==1) ) then
		
				println ("I'm in the Zoom In")
				Display4.texture.mapScaling.x =Display4.texture.mapScaling.x + 1.5*ZoomValue
				Display4.texture.mapScaling.y =Display4.texture.mapScaling.y + 1.5*ZoomValue	
						
		end if
	
		if ((Display4.texture.mapScaling.x >= 8 ) or (Display4.texture.mapScaling.y >= 8)) then 
					
			Display4.texture.mapScaling.x = 8
			Display4.texture.mapScaling.y = 8
					
		end if
	
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' 	

		if ((Display4.texture.mapScaling.x >=3.65 ) and (Display4.texture.mapScaling.y >=3.65) and (ZoomOut==1) ) then
		
				println ("I'm in the Zoom Out")
				

			 
			if ((Display4.texture.mapPosition.x <= 28) and (Display4.texture.mapPosition.x >= 1.5) and (Display4.texture.mapPosition.y <= 20)and (Display4.texture.mapPosition.y >=5) and (ScrollRight==1)) then 
				Display4.texture.mapScaling.x =Display4.texture.mapScaling.x - (1-1.5*ZoomValue)
				Display4.texture.mapScaling.y =Display4.texture.mapScaling.y - (1-1.5*ZoomValue)
				
			end if
			
					
			
			
			if (Display4.texture.mapPosition.y <= 5.0) then    
						
				Display4.texture.mapPosition.y = 5.0  
						
			end if 
			
			if (Display4.texture.mapPosition.y >= 20.0) then   
						
				Display4.texture.mapPosition.y = 20.0  
						 
			end if 		 
			 
			if (Display4.texture.mapPosition.x <= 1.5) then   
						
				Display4.texture.mapPosition.x = 1.5 
						
			end if  
			
				
			if (Display4.texture.mapPosition.x >= 28) then  
			
				Display4.texture.mapPosition.x = 28 
			
			end if
				 
		end if
		
				
		if ((Display4.texture.mapScaling.x <= 3.65 ) or (Display4.texture.mapScaling.y <= 3.65)) then 
					
			Display4.texture.mapScaling.x = 3.65 
			Display4.texture.mapScaling.y = 3.65
					
		end if
		
		if (Display4.texture.mapScaling.x = 3.65) then 
				
			if (Display4.texture.mapPosition.x <= 6.5) then 
				Display4.texture.mapPosition.x = 6.5
			end if 
			
			if (Display4.texture.mapPosition.y <= 9.5) then 
				Display4.texture.mapPosition.y = 9.5
			end if 
					
		end if 
		
	end if	
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' 	
end sub 


sub OnOSCMessage_ScrollProgress (ScrollValue as Double) 
		
if (displays[3].visible=true) then 
Display4.texture.mapScaling.x	= Display4.texture.mapScaling.y	
	println ("scroll") 
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' 
'(Display4.texture.mapPosition.x <= 28) and (Display4.texture.mapPosition.x >= 1.5) and  (Display4.texture.mapPosition.y > 5)
		if ((Display4.texture.mapPosition.y >= 5) and   (Display4.texture.mapPosition.y <= 20) and (ScrollUp==1)) then
		
			
			println ("I'm in the Scroll Up") 
			Display4.texture.mapPosition.y =  Display4.texture.mapPosition.y + ScrollValue 
									
		end if 
		
		if (Display4.texture.mapPosition.y <= 5.0) then   
					
			Display4.texture.mapPosition.y = 5.0 
					
		end if 
		 	
		if (Display4.texture.mapPosition.y >= 20.0) then  
					
			Display4.texture.mapPosition.y = 20.0 
					
		end if
	
		
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''  		
'(Display4.texture.mapPosition.x < 28) and (Display4.texture.mapPosition.x > 1.5) and 	 (Display4.texture.mapPosition.y >5)
		if ((Display4.texture.mapPosition.y <= 20)  and (Display4.texture.mapPosition.y >= 5) and (ScrollDown==1)) then 
		
			println ("I'm in the Scroll Down") 
			Display4.texture.mapPosition.y = Display4.texture.mapPosition.y - (1 - 1.5 * ScrollValue)
								 
		end if 
		
		if (Display4.texture.mapPosition.y <= 5.0) then   
					
			Display4.texture.mapPosition.y = 5.0 
					
		end if 
		
		if (Display4.texture.mapPosition.y >= 20.0) then  
					
			Display4.texture.mapPosition.y = 20.0 
					
		end if 		
		
		if (Display4.texture.mapPosition.x <= 1.5) then  
					
			Display4.texture.mapPosition.x = 1.5
					
		end if 
		
			
		if (Display4.texture.mapPosition.x >= 28) then 
		
			Display4.texture.mapPosition.x = 28
		
		end if
end if 
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' 
end sub 

sub OnOSCMessage_ScrollSideProgress (ScrollSideValue as Double)
		
if (displays[3].visible=true) then  
Display4.texture.mapScaling.x	= Display4.texture.mapScaling.y	 
	println ("scrollSIde") 
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' 
'(Display4.texture.mapPosition.x <= 28) and (Display4.texture.mapPosition.x >= 1.5) and  (Display4.texture.mapPosition.y > 5)
		if ((Display4.texture.mapPosition.x >= 1.5) and   (Display4.texture.mapPosition.x <= 28) and (ScrollRight==1)) then
		
			
			println ("I'm in the Scroll Right") 
			Display4.texture.mapPosition.x = Display4.texture.mapPosition.x + 2*ScrollSideValue 
									
		end if 
		
		if (Display4.texture.mapPosition.x <= 1.5) then   
					
			Display4.texture.mapPosition.x = 1.5
					
		end if 
		 	
		if (Display4.texture.mapPosition.x >= 28.0) then  
					
			Display4.texture.mapPosition.x = 28.0 
					
		end if
	
		
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''  		
'(Display4.texture.mapPosition.x < 28) and (Display4.texture.mapPosition.x > 1.5) and 	 (Display4.texture.mapPosition.y >5)
		if ((Display4.texture.mapPosition.x <= 28)  and (Display4.texture.mapPosition.x >= 1.5) and (ScrollLeft==1)) then 
		
			println ("I'm in the Scroll Left") 
			Display4.texture.mapPosition.x = Display4.texture.mapPosition.x - (1-1.5*ScrollSideValue)
								 
		end if 
		
		if (Display4.texture.mapPosition.x <= 1.5) then   
					
			Display4.texture.mapPosition.x = 1.5 
					
		end if 
		
		if (Display4.texture.mapPosition.x >= 28.0) then  
					
			Display4.texture.mapPosition.x = 28.0 
					
		end if 		
		
		if (Display4.texture.mapPosition.y <= 5) then  
					
			Display4.texture.mapPosition.y = 5
					
		end if 
		
			
		if (Display4.texture.mapPosition.y >= 20) then 
		
			Display4.texture.mapPosition.y = 20
		
		end if
end if 
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' 
end sub 

	

sub OnOSCMessage_ZoomIn (ZoomingIn as Double)
	ZoomingIn=ZoomIn
end sub 

sub OnOSCMessage_ZoomOut (ZoomingOut as Double) 
	ZoomingOut=ZoomOut
end sub 


sub OnOSCMessage_ScrollUp (ScrollingUp as Double)
	ScrollingUp=ScrollUp
end sub 

sub OnOSCMessage_ScrollDown (ScrollingDown as Double)
	ScrollingDown=ScrollDown
end sub 

sub OnOSCMessage_ScrollURight (ScrollingRight as Double)
	ScrollingRight=ScrollRight
end sub  

sub OnOSCMessage_ScrollLeft (ScrollingLeft as Double)
	ScrollingLeft=ScrollLeft
end sub 









