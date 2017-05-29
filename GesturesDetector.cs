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
    public sealed class GesturesDetector : IDisposable

    {
        IPEndPoint myDetector = new IPEndPoint(IPAddress.Loopback, 8000);
        IPEndPoint vizArtist = new IPEndPoint(IPAddress.Loopback, 7000);



        /// <summary> Path to the gesture database that was trained with VGB </summary>
        private readonly string gestureDatabase = @"C:\Users\marin\Documents\4º TELECOMUNICACIONES\Hochschule Düsseldorf\Trabajo Final de Grado\Kinect Bachelorarbeit_MarinaBallesterRipoll\KinectGesture2OSC\GESTURE_DETECTOR\Database\Gestures_database.gbd";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is holding the maximum up turn position </summary>
        private readonly string maxUpGestureName = "Max_Up";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is holding the maximum down turn position </summary>
        private readonly string maxDownGestureName = "Max_Down";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is actively scrolling up the map </summary>
        private readonly string scrollUpGestureName = "Scroll_Up";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is actively scrolling down the map </summary>
        private readonly string scrollDownGestureName = "Scroll_Down";

        /// <summary> Name of the continuous gesture in the database which tracks the scrolling up progress </summary>
        private readonly string scrollProgressGestureName = "ScrollProgress";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is actively scrolling up the map </summary>
        private readonly string scrollRightGestureName = "Scroll_Right";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is actively scrolling down the map </summary>
        private readonly string scrollLeftGestureName = "Scroll_Left";

        /// <summary> Name of the continuous gesture in the database which tracks the scrolling up progress </summary>
        private readonly string scrollSideProgressGestureName = "ScrollSideProgress";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is holding the maximum up turn position </summary>
        private readonly string maxInGestureName = "Max_In";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is holding the maximum down turn position </summary>
        private readonly string maxOutGestureName = "Max_Out";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is actively scrolling up the map </summary>
        private readonly string zoomInGestureName = "Zoom_In";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is actively scrolling down the map </summary>
        private readonly string zoomOutGestureName = "Zoom_Out";

        /// <summary> Name of the continuous gesture in the database which tracks the scrolling up progress </summary>
        private readonly string zoomProgressGestureName = "ZoomProgress";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is pointing up. </summary>
        private readonly string pointUpGestureName = "PointUp";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is pointing down.</summary>
        private readonly string pointDownGestureName = "PointDown";

        /// <summary> Name of the discrete gesture in the database for detecting when the user is pointing to the middle </summary>
        private readonly string pointMiddleGestureName = "PointMiddle";

        /// <summary> Name of the discrete gesture in the database for detecting when the user wants to start the application </summary>
        private readonly string startGestureName = "Start";


        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        /// <summary>
        /// Initializes a new instance of the GestureDetector class along with the gesture frame source and reader
        /// </summary>
        /// <param name="kinectSensor">Active sensor to initialize the VisualGestureBuilderFrameSource object with</param>
        /// <param name="gestureResultView">GestureResultView object to store gesture results of a single body to</param>
        public GesturesDetector(KinectSensor kinectSensor, GestureResult gestureResultView)
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
        public GestureResult GestureResultView { get; private set; }


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
                    // gets all discrete and continuous gesture results that arrived with the latest frame
                    var discreteResults = frame.DiscreteGestureResults;
                    var continuousResults = frame.ContinuousGestureResults;

                    if (discreteResults != null)
                    {
                       
                        bool scrollUp = this.GestureResultView.GoUp;
                        bool scrollDown = this.GestureResultView.GoDown;
                        bool scrollRight = this.GestureResultView.GoRight;
                        bool scrollLeft = this.GestureResultView.GoLeft;
                       
                        float scrollProgress = this.GestureResultView.ScrollProgress;
                        float scrollSideProgress = this.GestureResultView.ScrollSideProgress;
                       
                        bool zoomIn = this.GestureResultView.GoClose;
                        bool zoomOut = this.GestureResultView.GoFar;
                        
                        float zoomProgress = this.GestureResultView.ZoomProgress;
                        bool upButton = this.GestureResultView.UpButton;
                        bool downButton = this.GestureResultView.DownButton;
                        bool middleButton = this.GestureResultView.MiddleButton;
                        bool startApp = this.GestureResultView.StartApp;


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

                                    else if (gesture.Name.Equals(this.scrollRightGestureName))
                                    {
                                        scrollRight = result.Detected;

                                        if (scrollRight = result.Detected)
                                        {

                                            OscMessage OSCmessageScrollRight = new OscMessage(myDetector, "/ScrollRight", 1.0f);

                                            OSCmessageScrollRight.Send(myDetector);
                                        }

                                    }

                                    else if (gesture.Name.Equals(this.scrollLeftGestureName))
                                    {
                                        scrollLeft = result.Detected;

                                        if (scrollLeft = result.Detected)
                                        {

                                            OscMessage OSCmessageScrollLeft = new OscMessage(myDetector, "/ScrollLeft", 1.0f);

                                            OSCmessageScrollLeft.Send(myDetector);
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
                                
                                   

                                    else if (gesture.Name.Equals(this.pointUpGestureName))
                                    {
                                        upButton = result.Detected;

                                        if (upButton = result.Detected)
                                        {

                                            OscMessage OSCmessagePointUp = new OscMessage(myDetector, "/PointUp", 1.0f);

                                            OSCmessagePointUp.Send(myDetector);

                                            System.Threading.Thread.Sleep(1000);

                                        }
                                    }
                                    else if (gesture.Name.Equals(this.pointDownGestureName))
                                    {
                                        downButton = result.Detected;

                                        if (downButton = result.Detected)
                                        {

                                            OscMessage OSCmessagePointDown = new OscMessage(myDetector, "/PointDown", 1.0f);

                                            OSCmessagePointDown.Send(myDetector);

                                            System.Threading.Thread.Sleep(1000);
                                        }
                                    }
                                    else if (gesture.Name.Equals(this.pointMiddleGestureName))
                                    {
                                        middleButton = result.Detected;

                                        if (middleButton = result.Detected)
                                        {

                                            OscMessage OSCmessagePointMiddle = new OscMessage(myDetector, "/PointMiddle", 1.0f);

                                            OSCmessagePointMiddle.Send(myDetector);

                                            System.Threading.Thread.Sleep(1000);
                                        }
                                    }

                                    else if (gesture.Name.Equals(this.startGestureName))
                                    {
                                        startApp = result.Detected;

                                        if (startApp = result.Detected)
                                        {

                                            OscMessage OSCmessageStart = new OscMessage(myDetector, "/Start", 1.0f);

                                            OSCmessageStart.Send(myDetector);

                                            System.Threading.Thread.Sleep(1000);
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
                                if (gesture.Name.Equals(this.scrollSideProgressGestureName) && gesture.GestureType == GestureType.Continuous)
                                {
                                    ContinuousGestureResult result = null;
                                    continuousResults.TryGetValue(gesture, out result);


                                    if (result != null)
                                    {
                                        scrollSideProgress = result.Progress;

                                        if (scrollRight || scrollLeft)
                                        {

                                            OscMessage OSCmessagescrollSideProgress = new OscMessage(myDetector, "/ScrollSideProgress", scrollProgress);

                                            OSCmessagescrollSideProgress.Send(myDetector);

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

                                        if (zoomIn || zoomOut)
                                        {

                                            OscMessage OSCmessagezoomProgress = new OscMessage(myDetector, "/ZoomProgress", zoomProgress);

                                            OSCmessagezoomProgress.Send(myDetector);

                                        }

                                    }
                                }



                            }
                        }
                       
            
                        // clamp the progress value between 0 and 1
                        if (scrollProgress < 0)
                        {
                            scrollProgress = 0;
                        }
                        else if (scrollProgress > 1)
                        {
                            scrollProgress = 1;
                        }

                        if (scrollSideProgress < 0)
                        {
                            scrollSideProgress = 0;
                        }
                        else if (scrollSideProgress > 1)
                        {
                            scrollSideProgress = 1;
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

                        if (!scrollRight && !scrollLeft)
                        {
                            scrollSideProgress = -1;
                        }

                        // update the UI with the latest gesture detection results
                        this.GestureResultView.UpdateGestureResult(false, false, false, false, false, -1.0f, -1.0f, false, false,-1.0f, false, false, false, false);
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
