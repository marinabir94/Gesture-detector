//------------------------------------------------------------------------------
// <copyright file="GestureResultView.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.ContinuousGestureBasics
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;
    using Microsoft.Samples.Kinect.ContinuousGestureBasics.Common;

    /// <summary>
    /// Tracks gesture results coming from the GestureDetector and displays them in the UI.
    /// Updates the SpaceView object with the latest gesture result data from the sensor.
    /// </summary>
    public sealed class GestureResultView : BindableBase
    {
        /// <summary> True, if the user is attempting to go up (either 'Scroll_up' or 'Max_up' is detected) </summary>
        private bool goUp = false;

        /// <summary> True, if the user is attempting to go down (either 'Scroll_down' or 'Max_down' is detected) </summary>
        private bool goDown = false;

        /// <summary> True, if the user is not doing any gesture</summary>
        private bool keepLevel = false;

        /// <summary> Current progress value reported by the continuous 'scrollUpProgress' gesture </summary>
        private float scrollProgress = 0.0f;

        /// <summary> True, if the user is attempting to go up (either 'Zoom_in' or 'Max_In' is detected) </summary>
        private bool goClose = false;

        /// <summary> True, if the user is attempting to go down (either 'Zoom_Out' or 'Max_Out' is detected) </summary>
        private bool goFar = false;

        /// <summary> True, if the user is not doing any gesture</summary>
        private bool keepingLevelZoom = false;

        /// <summary> Current progress value reported by the continuous 'zoomProgress' gesture </summary>
        private float zoomProgress = 0.0f;

        /// <summary> True, if the user is attempting to press up </summary>
        private bool upButton = false;

        /// <summary> True, if the user is attempting to press down. </summary>
        private bool downButton = false;

        /// <summary> True, if the user is attempting to press to the middle. </summary>
        private bool middleButton = false;

        /// <summary> True, if the user is attempting to press to the middle. </summary>
        private bool startApp = false;

        /// <summary> True, if the body is currently being tracked </summary>
        private bool isTracked = false;



        /// <summary>
        /// Initializes a new instance of the GestureResultView class and sets initial property values
        /// </summary>
        /// <param name="isTracked">True, if the body is currently tracked</param>
        /// <param name="up">True, if the 'Scroll_up' gesture is currently detected</param>
        /// <param name="down">True, if the 'Scroll_down' gesture is currently detected</param>
        /// <param name="straightScroll">True, if the 'KeepLevel' gesture is currently detected</param>
        /// <param name="progressScroll">Progress value of the 'SteerProgress' gesture</param>
        /// <param name="close">True, if the 'Scroll_up' gesture is currently detected</param>
        /// <param name="far">True, if the 'Scroll_down' gesture is currently detected</param>
        /// <param name="straightZoom">True, if the 'KeepLevelZoom' gesture is currently detected</param>
        /// <param name="progressZoom">Progress value of the 'zoomProgress' gesture</param>
        /// <param name="upPoint">True, if the 'Steer_Left' gesture is currently detected</param>
        /// <param name="downPoint">True, if the 'Steer_Right' gesture is currently detected</param>
        /// <param name="middlePoint">True, if the 'PointMiddle' gesture is currently detected</param>
        /// <param name="init">True, if the 'PointMiddle' gesture is currently detected</param>

        public GestureResultView(bool isTracked, bool up, bool down, bool straightScroll, bool close, bool far, bool straightZoom, bool upPoint, bool downPoint, bool middlePoint, bool startApp, float progressScroll, float progressZoom)
        {
            

            this.IsTracked = isTracked;
            this.goUp = up;
            this.goDown = down;
            this.keepLevel = straightScroll;
            this.scrollProgress = progressScroll;
            this.goClose = close;
            this.goFar = far;
            this.keepingLevelZoom = straightZoom;
            this.zoomProgress = progressZoom;
            this.UpButton = upPoint;
            this.DownButton = downPoint;
            this.MiddleButton = middlePoint;
            this.StartApp = startApp;
           
        }

        /// <summary> 
        /// Gets a value indicating whether or not the body associated with the gesture detector is currently being tracked 
        /// </summary>
        public bool IsTracked
        {
            get
            {
                return this.isTracked;
            }

            private set
            {
                this.SetProperty(ref this.isTracked, value);
            }
        }

        /// <summary> 
        /// Gets a value indicating whether the user is attempting to go up
        /// </summary>
        public bool GoUp
        {
            get
            {
                return this.goUp;
            }

            private set
            {
                this.SetProperty(ref this.goUp, value);
            }
        }

        /// <summary> 
        /// Gets a value indicating whether the user is attempting to go down 
        /// </summary>
        public bool GoDown
        {
            get
            {
                return this.goDown;
            }

            private set
            {
                this.SetProperty(ref this.goDown, value);
            }
        }

        /// <summary> 
        /// Gets a value indicating whether the user is trying to keep straight the map
        /// </summary>
        public bool KeepLevel
        {
            get
            {
                return this.keepLevel;
            }

            private set
            {
                this.SetProperty(ref this.keepLevel, value);
            }
        }

        /// <summary> 
        /// Gets a value indicating the progress associated with the 'scrollUpProgress' gesture for the tracked body 
        /// </summary>
        public float ScrollProgress
        {
            get
            {
                return this.scrollProgress;
            }

            private set
            {
                this.SetProperty(ref this.scrollProgress, value);
            }
        }
        /// <summary> 
        /// Gets a value indicating whether the user is attempting to go up
        /// </summary>
        public bool KeepingLevelZoom
        {
            get
            {
                return this.keepingLevelZoom;
            }

            private set
            {
                this.SetProperty(ref this.keepingLevelZoom, value);
            }
        }

        /// <summary> 
        /// Gets a value indicating whether the user is attempting to go up
        /// </summary>
        public bool GoClose
        {
            get
            {
                return this.goClose;
            }

            private set
            {
                this.SetProperty(ref this.goClose, value);
            }
        }

        /// <summary> 
        /// Gets a value indicating whether the user is attempting to go down 
        /// </summary>
        public bool GoFar
        {
            get
            {
                return this.goFar;
            }

            private set
            {
                this.SetProperty(ref this.goFar, value);
            }
        }

 

        /// <summary> 
        /// Gets a value indicating the progress associated with the 'scrollUpProgress' gesture for the tracked body 
        /// </summary>
        public float ZoomProgress
        {
            get
            {
                return this.zoomProgress;
            }

            private set
            {
                this.SetProperty(ref this.zoomProgress, value);
            }
        }
        /// <summary> 
        /// Gets a value indicating whether the user is attempting to press the Up button
        /// </summary>
        public bool UpButton
        {
            get
            {
                return this.upButton;
            }

            private set
            {
                this.SetProperty(ref this.upButton, value);
            }
        }

        /// <summary> 
        /// Gets a value indicating whether the user is attempting to press the down button
        /// </summary>
        public bool DownButton
        {
            get
            {
                return this.downButton;
            }

            private set
            {
                this.SetProperty(ref this.downButton, value);
            }
        }

        /// <summary> 
        /// Gets a value indicating whether the user is trying to press the middle button.
        /// </summary>
        public bool MiddleButton
        {
            get
            {
                return this.middleButton;
            }

            private set
            {
                this.SetProperty(ref this.middleButton, value);
            }
        }

        public bool StartApp
        {
            get
            {
                return this.startApp;
            }

            private set
            {
                this.SetProperty(ref this.startApp, value);
            }
        }



        /// <summary>
        /// Updates gesture detection result values for display in the UI
        /// </summary>
        /// <param name="isBodyTrackingIdValid"
        /// <param name="up"
        /// <param name="down"
        /// <param name="straightScroll"
        /// <param name="progressScroll"
        /// <param name="close"
        /// <param name="far"
        /// <param name="straightZoom"
        /// <param name="progressZoom"
        /// <param name="upPoint"
        /// <param name="downPoint"
        /// <param name="middlePoint"
        /// <param name="init"

        public void UpdateGestureResult(bool isBodyTrackingIdValid, bool up, bool down, bool straightScroll, float progressScroll, bool close, bool far, bool straightZoom, float progressZoom, bool upPoint, bool downPoint, bool middlePoint, bool init)
        {
            this.IsTracked = isBodyTrackingIdValid;

            if (!this.isTracked)
            {
                this.goUp = false;
                this.goDown = false;
                this.keepLevel = false;
                this.scrollProgress = -1.0f;
                this.goClose = false;
                this.goFar = false;
                this.keepingLevelZoom = false;
                this.zoomProgress = -1.0f;
                this.UpButton = false;
                this.DownButton = false;
                this.MiddleButton = false;
                this.StartApp = false;
            }
            else
            {
                this.goUp = up;
                this.goDown = down;
                this.keepLevel = straightScroll;
                this.scrollProgress = progressScroll;
                this.goClose = close;
                this.goFar = far;
                this.keepingLevelZoom = straightZoom;
                this.zoomProgress = progressZoom;
                this.UpButton = upPoint;
                this.DownButton = downPoint;
                this.MiddleButton = middlePoint;
                this.StartApp = startApp;

            }
            
        }
    }
}