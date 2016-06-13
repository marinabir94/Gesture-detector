//------------------------------------------------------------------------------
// <copyright file="GestureDetector.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.ContinuousGestureBasics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;
    using System.Net;
    using Bespoke.Common.Osc;






    /// <summary>
    /// Gesture Detector class which polls for VisualGestureBuilderFrames from the Kinect sensor
    /// Updates the associated GestureResultView object with the latest gesture results
    /// </summary>
    public sealed class GestureDetector : IDisposable

    {
        IPEndPoint myDetector = new IPEndPoint(IPAddress.Loopback, 7001);
        IPEndPoint vizArtist = new IPEndPoint(IPAddress.Loopback, 8000);


        
        /// <summary> Path to the gesture database that was trained with VGB </summary>
        private readonly string gestureDatabase = @"C:\Users\Gesture\Desktop\KINECTv2\ContiDiscrGestureBasics\GESTURE_DETECTOR\GESTURE_DETECTOR\Database\GESTURE_DETECTOR_VGB.gbd";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is scrolling up at the maximum level </summary>
        private readonly string maxUpGestureName = "Max_Up";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is scrolling up at the minimum level </summary>
        private readonly string maxDownGestureName = "Max_Down";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is actively scrolling up the map </summary>
        private readonly string scrollUpGestureName = "Scroll_Up";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is actively scrolling down the map </summary>
        private readonly string scrollDownGestureName = "Scroll_Down";

        /// <summary> Name of the continuous gesture in the database which tracks the scrolling up progress </summary>
        private readonly string scrollProgressGestureName = "ScrollProgress";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is zooming in at the maximum level </summary>
        private readonly string maxInGestureName = "Max_In";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is zooming up at the minimum level </summary>
        private readonly string maxOutGestureName = "Max_Out";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is actively zooming in the map </summary>
        private readonly string zoomInGestureName = "Zoom_In";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is actively zooming out the map </summary>
        private readonly string zoomOutGestureName = "Zoom_Out";

        /// <summary> Name of the continuous gesture in the database which tracks the scrolling up progress </summary>
        private readonly string zoomProgressGestureName = "ZoomProgress";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is holding the maximum up turn position </summary>
        private readonly string pointUpGestureName = "PointUp";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is holding the maximum right turn position </summary>
        private readonly string pointDownGestureName = "PointDown";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is holding the wheel straight </summary>
        private readonly string pointMiddleGestureName = "PointMiddle";


        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        /// <summary>
        /// Initializes a new instance of the GestureDetector class along with the gesture frame source and reader
        /// </summary>
        /// <param name="kinectSensor">Active sensor to initialize the VisualGestureBuilderFrameSource object with</param>
        /// <param name="gestureResultView">GestureResultView object to store gesture results of a single body to</param>
        public GestureDetector(KinectSensor kinectSensor, GestureResultView gestureResultView)
        {
            if (kinectSensor == null)
            {
                throw new ArgumentNullException("kinectSensor");
            }

            if (gestureResultView == null)
            {
                throw new ArgumentNullException("gestureResultView");
            }

            this.GestureResultView = gestureResultView;


            // create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            this.vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);

            // open the reader for the vgb frames
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.IsPaused = true;
            }

            // load all gestures from the gesture database
            using (var database = new VisualGestureBuilderDatabase(this.gestureDatabase))
            {
                this.vgbFrameSource.AddGestures(database.AvailableGestures);
            }


        }

        /// <summary> 
        /// Gets the GestureResultView object which stores the detector results for display in the UI 
        /// </summary>
        public GestureResultView GestureResultView { get; private set; }


        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return this.vgbFrameSource.TrackingId;
            }

            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                {
                    this.vgbFrameSource.TrackingId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the detector is currently paused
        /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }

        /// <summary>
        /// Retrieves the latest gesture detection results from the sensor
        /// </summary>
        public void UpdateGestureData()
        {
            using (var frame = this.vgbFrameReader.CalculateAndAcquireLatestFrame())
            {
                if (frame != null)
                {
                    // get all discrete and continuous gesture results that arrived with the latest frame
                    var discreteResults = frame.DiscreteGestureResults;
                    var continuousResults = frame.ContinuousGestureResults;

                    if (discreteResults != null)
                    {
                        bool maxUp = false;
                        bool maxDown = false;
                        bool scrollUp = this.GestureResultView.GoUp;
                        bool scrollDown = this.GestureResultView.GoDown;
                        bool keepLevel = this.GestureResultView.KeepLevel;
                        float scrollProgress = this.GestureResultView.ScrollProgress;
                        bool maxIn = false;
                        bool maxOut = false;
                        bool zoomIn = this.GestureResultView.GoClose;
                        bool zoomOut = this.GestureResultView.GoFar;
                        bool keepLevelZoom = this.GestureResultView.KeepLevelZoom;
                        float zoomProgress = this.GestureResultView.ZoomProgress;
                        bool upButton = this.GestureResultView.UpButton;
                        bool downButton = this.GestureResultView.DownButton;
                        bool middleButton = this.GestureResultView.MiddleButton;


                        foreach (var gesture in this.vgbFrameSource.Gestures)
                        {
                            if (gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    if (gesture.Name.Equals(this.scrollUpGestureName))
                                    {
                                        scrollUp = result.Detected;


                                        if (scrollUp = result.Detected)
                                        {

                                            OscMessage OSCmessageScrollUp = new OscMessage(myDetector, "/ScrollUp", 1.0f);

                                            OSCmessageScrollUp.Send(myDetector);
                                        }

                                    }
                                    else if (gesture.Name.Equals(this.scrollDownGestureName))
                                    {
                                        scrollDown = result.Detected;

                                        if (scrollDown = result.Detected)
                                        {

                                            OscMessage OSCmessageScrollDown = new OscMessage(myDetector, "/ScrollDown", 1.0f);

                                            OSCmessageScrollDown.Send(myDetector);
                                        }

                                    }
                                    else if (gesture.Name.Equals(this.maxUpGestureName))
                                    {
                                        maxUp = result.Detected;

                                        if (maxUp = result.Detected)
                                        {

                                            OscMessage OSCmessageMaxUp = new OscMessage(myDetector, "/MaxUp", 1.0f);

                                            OSCmessageMaxUp.Send(myDetector);

                                        }
                                    }
                                    else if (gesture.Name.Equals(this.maxDownGestureName))
                                    {
                                        maxDown = result.Detected;

                                        if (maxDown = result.Detected)
                                        {

                                            OscMessage OSCmessageMaxDown = new OscMessage(myDetector, "/MaxDown", 1.0f);

                                            OSCmessageMaxDown.Send(myDetector);

                                        }
                                    }

                                    else if (gesture.Name.Equals(this.zoomInGestureName))
                                    {
                                        zoomIn = result.Detected;

                                        if (zoomIn = result.Detected)
                                        {

                                            OscMessage OSCmessageZoomIn = new OscMessage(myDetector, "/ZoomIn", 1.0f);

                                            OSCmessageZoomIn.Send(myDetector);
                                        }
                                    }
                                    else if (gesture.Name.Equals(this.zoomOutGestureName))
                                    {
                                        zoomOut = result.Detected;

                                        if (zoomOut = result.Detected)
                                        {

                                            OscMessage OSCmessageZoomOut = new OscMessage(myDetector, "/ZoomOut", 1.0f);

                                            OSCmessageZoomOut.Send(myDetector);

                                        }
                                    }
                                    else if (gesture.Name.Equals(this.maxInGestureName))
                                    {
                                        maxIn = result.Detected;

                                        if (maxIn = result.Detected)
                                        {

                                            OscMessage OSCmessageMaxIn = new OscMessage(myDetector, "/MaxIn", 1.0f);

                                            OSCmessageMaxIn.Send(myDetector);

                                        }

                                    }
                                    else if (gesture.Name.Equals(this.maxOutGestureName))
                                    {
                                        maxOut = result.Detected;

                                        if (maxOut = result.Detected)
                                        {

                                            OscMessage OSCmessageMaxOut = new OscMessage(myDetector, "/MaxOut", 1.0f);

                                            OSCmessageMaxOut.Send(myDetector);

                                        }
                                    }

                                    else if (gesture.Name.Equals(this.pointUpGestureName))
                                    {
                                        upButton = result.Detected;

                                        if (upButton = result.Detected)
                                        {

                                            OscMessage OSCmessagePointUp = new OscMessage(myDetector, "/PointUp", 1.0f);

                                            OSCmessagePointUp.Send(myDetector);

                                        }
                                    }
                                    else if (gesture.Name.Equals(this.pointDownGestureName))
                                    {
                                        downButton = result.Detected;

                                        if (downButton = result.Detected)
                                        {

                                            OscMessage OSCmessagePointDown = new OscMessage(myDetector, "/PointDown", 1.0f);

                                            OSCmessagePointDown.Send(myDetector);
                                        }
                                    }
                                    else if (gesture.Name.Equals(this.pointMiddleGestureName))
                                    {
                                        middleButton = result.Detected;

                                        if (middleButton = result.Detected)
                                        {

                                            OscMessage OSCmessagePointMiddle = new OscMessage(myDetector, "/PointMiddle", 1.0f);

                                            OSCmessagePointMiddle.Send(myDetector);
                                        }
                                    }
                                }
                            }
                            // It gives a value for each continuous gesture ZOOMING & SCROLLING
                            if (continuousResults != null)
                            {
                                if (gesture.Name.Equals(this.scrollProgressGestureName) && gesture.GestureType == GestureType.Continuous)
                                {
                                    ContinuousGestureResult result = null;
                                    continuousResults.TryGetValue(gesture, out result);

                                   
                                    if (result != null)
                                    {
                                       scrollProgress = result.Progress;

                                        if (scrollUp || scrollDown)
                                        {

                                            OscMessage OSCmessagescrollProgress = new OscMessage(myDetector, "/ScrollProgress", scrollProgress);

                                            OSCmessagescrollProgress.Send(myDetector);

                                        }
                                    }
                                }

                                if (gesture.Name.Equals(this.zoomProgressGestureName) && gesture.GestureType == GestureType.Continuous)
                                {
                                    ContinuousGestureResult result = null;
                                    continuousResults.TryGetValue(gesture, out result);

                                    if (result != null)
                                    {
                                        zoomProgress = result.Progress;

                                        if (zoomIn || zoomOut )
                                        {

                                            OscMessage OSCmessagezoomProgress = new OscMessage(myDetector, "/ZoomProgress", zoomProgress);

                                            OSCmessagezoomProgress.Send(myDetector);

                                        }
                                         
                                    }
                                }



                            }
                        }
/*
                        // if either the 'Steer_Left' or 'MaxTurn_Left' gesture is detected, then we want to turn the ship left
                        if (scrollUp || maxUp)
                        {
                            scrollUp = true;
                            scrollDown = false;
                            keepLevel = false;
                            keepLevelZoom = true;

                        }

                        // if either the 'Steer_Right' or 'MaxTurn_Right' gesture is detected, then we want to turn the ship right
                        if ((scrollDown) || maxDown)
                        {
                            scrollDown = true;
                            scrollUp = false;
                            keepLevel = false;
                            keepLevelZoom = true;
                        }

                        // if "Point_Up" is detected, then we want to go forwards.
                        if (upButton)
                        {
                            upButton = true;
                            downButton = false;
                            middleButton = true;
                            keepLevel = true;
                            keepLevelZoom = true;

                        }


                        //If "Point_Down" is detected, then we want to go backwards.
                        if( downButton)
                        {
                            downButton = true;
                            upButton = false;
                            middleButton = false;
                            keepLevel = true;
                            keepLevelZoom = true;
                        }


                        //If "Point_Middle" is detected, then we want to go stop.
                        if (middleButton)
                        {
                            middleButton = true;
                            keepLevel = true;
                            keepLevelZoom = true;
                        }
//
                        if (zoomIn)
                        {
                            zoomIn = true;
                            zoomOut = false;
                            keepLevelZoom = false;
                            keepLevel = true;

                        }

                        if(zoomOut)
                        {
                            zoomIn = false;
                            zoomOut = true;
                            keepLevelZoom = false;
                            keepLevel = true;
                        }


*/

                        // clamp the progress value between 0 and 1
                        if (scrollProgress < 0)
                        {
                            scrollProgress = 0;
                        }
                        else if (scrollProgress > 1)
                        {
                            scrollProgress = 1;
                        }

                        // clamp the progress value between 0 and 1
                        if (zoomProgress < 0)
                        {
                            zoomProgress = 0;
                        }
                        else if (zoomProgress > 1)
                        {
                            zoomProgress = 1;
                        }

                        // Continuous gestures will always report a value while the body is tracked. 
                        // We need to provide context to this value by mapping it to one or more discrete gestures.
                        // For this sample, we will ignore the progress value whenever the user is not performing any of the discrete gestures.
                        if (!zoomIn && !zoomOut)
                        {
                            zoomProgress = -1;
                        }

                        if (!scrollUp && !scrollDown)
                        {
                            scrollProgress = -1;
                        }

                        // update the UI with the latest gesture detection results
                        this.GestureResultView.UpdateGestureResult(true, scrollUp, scrollDown, keepLevel,  scrollProgress,  zoomIn,  zoomOut,  keepLevelZoom,  zoomProgress,  upButton, downButton, middleButton);
                    }
                }
            }
        }

        /// <summary>
        /// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
        /// </summary>
        public void Dispose()
        {
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.Dispose();
                this.vgbFrameReader = null;
            }

            if (this.vgbFrameSource != null)
            {
                this.vgbFrameSource.Dispose();
                this.vgbFrameSource = null;
            }
        }
    }
}

