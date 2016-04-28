using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectEx;
using KinectEx.Smoothing;
using Microsoft.Kinect;
using System.Windows.Forms;
using KinectV2MouseControl;
using System.Drawing;
using System.Timers;
using System.Windows.Threading;

namespace MoMoker.src
{
    class KinectGestures
    {
        KinectSensor _sensor = null;
        BodyFrameReader _bodyReader = null;

        List<CustomBody> _bodies = null;
        SmoothedBodyList<KalmanSmoother> _kalmanBodies = new SmoothedBodyList<KalmanSmoother>();
        SmoothedBodyList<ExponentialSmoother> _exponentialBodies = new SmoothedBodyList<ExponentialSmoother>();
        private GestureState rotateInitState;
        private readonly float zRotateDelta;
        private readonly float yRotateDelta;
        private readonly float xRotateDelta;
        private MouseGestureState startScrollGesture;
        private MouseGestureState startScrollClickGesture;
        private double yKinectTop = 0.3;
        private double yKinectBottom = -0.25;
        private double xKinectLeft = -0.3;
        private double xKinectRight = 0.6;

        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler rightHandRotatedRightWays;
        System.Timers.Timer _cursorPositionTimer;
        private Point _cursorPosition;
        private Point _previousCursorPosition;
        private bool mouseInScroll = false;
        private MouseGestureState startRightHandClickGestureState;
        private MouseGestureState startLeftHandClickGestureState;
        private static readonly double MOUSESENSITIVITY = 2.5f;
        private double mouseSensitivity = MOUSESENSITIVITY;
        private int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        private int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        private static readonly double CURSORSMOOTHING = 0.9f;
        private double cursorSmoothing = CURSORSMOOTHING;
        private JointType REFERENCEJOINT = JointType.SpineMid;
        private System.Timers.Timer timer = new System.Timers.Timer();
        private bool doClick = true;
        private bool useGripGesture = true;
        private bool wasLeftGrip = false;
        private bool wasRightGrip = false;
        private bool alreadyTrackedPos = false;
        private float timeCount = 0f;
        private System.Windows.Point lastCurPos;
        private double pauseThresold = 60f;
        private float timeRequired = 2f;
        private bool RightDown;
        private MouseGestureState dragMouseGestureState;
        private MouseGestureState clickMouseGestureState;
        private MouseGestureState doubleClickMouseGestureState;
        private bool mouseInDrag;
        private MouseGestureState clickRightMouseGestureState;

        public KinectGestures()
        {
            rightHandRotatedRightWays += new ChangedEventHandler(LeftHandRotateHandler);

            _sensor = KinectSensor.GetDefault();

            timer.Interval = 100;
            timer.Elapsed += Timer_Tick;
            //timer.Start();

            _bodyReader = _sensor.BodyFrameSource.OpenReader();
            _bodyReader.FrameArrived += _bodyReader_FrameArrived;
            _sensor.Open();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //if (!doClick || useGripGesture) return;

            //if (!alreadyTrackedPos)
            //{
            //    timeCount = 0;
            //    return;
            //}
            //System.Windows.Point curPos = MouseControl.GetCursorPosition();

            //if ((lastCurPos - curPos).Length < pauseThresold)
            //{
            //    if ((timeCount += 0.1f) > timeRequired)
            //    {
            //        //MouseControl.MouseLeftDown();
            //        //MouseControl.MouseLeftUp();
            //        MouseControl.DoDoubleMouseClick();
            //        timeCount = 0;
            //    }
            //}
            //else
            //{
            //    timeCount = 0;
            //}

            //lastCurPos = curPos;
            
        }

        private void LeftHandRotateHandler(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void _bodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            IEnumerable<IBody> bodies = null; // to make the GetBitmap call a little cleaner
            bool dataReceived = false;
            using (BodyFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.GetAndRefreshBodyData(_kalmanBodies);
                    bodies = _kalmanBodies;
                    dataReceived = true;
                }
            }

            if (!dataReceived)
            {
                alreadyTrackedPos = false;
                return;
            }

            if (bodies != null)
            {
                //bodies.MapDepthPositions();
                IBody trackedbody = null;
                foreach (IBody body in bodies)
                {
                    if (body.IsTracked)
                    {
                        trackedbody = body;
                        break;
                    }
                }
                if (trackedbody != null)
                    detectGestures(trackedbody);
            }
        }

        private void detectGestures(IBody body)
        {
            // We need to recenter body center to kinect plane center
            body = RecenterBody(body);
            IJoint referenceJoint = getJoint(body, REFERENCEJOINT);
            IJoint rightHand = getJoint(body, JointType.HandRight);
            IJoint leftHand = getJoint(body, JointType.HandLeft);
            IJoint spineBase = getJoint(body, JointType.SpineBase);
            //if(body.HandRightState == HandState.Open || body.HandRightState == HandState.Closed)
            //    Console.WriteLine(body.HandRightState);
            MoveMouse(body, leftHand, rightHand, referenceJoint);
            //MouseLeftClick(body.HandLeftState, leftHand, referenceJoint);
            //MouseRightClick(body.HandRightState, rightHand, referenceJoint);
            //MouseScroll(body, rightHand, referenceJoint);
        }

        private IBody RecenterBody(IBody body)
        {
            var joints = body.Joints;
            Dictionary<JointType, IJoint> newjoints = new Dictionary<JointType, IJoint>(joints.Count);
            foreach (var item in joints)
            {
                IJoint joint = new CustomJoint(item.Key);
                joint.ColorPosition = item.Value.ColorPosition;
                joint.DepthPosition = item.Value.DepthPosition;
                joint.Position = item.Value.Position;
                joint.TrackingState = item.Value.TrackingState;
                newjoints.Add(item.Key,joint);
            }
            newjoints[REFERENCEJOINT].Position = new CameraSpacePoint();
            foreach (var item in newjoints)
            {
                if (item.Key == REFERENCEJOINT)
                    continue;
                CameraSpacePoint convertedPoint = new CameraSpacePoint();
                convertedPoint.X = newjoints[item.Key].Position.X - joints[JointType.SpineMid].Position.X;
                convertedPoint.Y = newjoints[item.Key].Position.Y - joints[JointType.SpineMid].Position.Y;
                convertedPoint.Z = newjoints[item.Key].Position.Z - joints[JointType.SpineMid].Position.Z;
                newjoints[item.Key].Position = convertedPoint;
            }
            body.Joints = newjoints;
            return body;
        }
        
        private void MoveMouse(IBody body, IJoint lefthand, IJoint rightHand, IJoint spineMid)
        {
            //Map body Spine Mid - Shoulder Left/Right to center Screen
            var zRightHandBodyDistance = spineMid.Position.Z - rightHand.Position.Z;
            var zLeftHandBodyDistance = spineMid.Position.Z - lefthand.Position.Z;

            //Console.WriteLine("(" + zLeftHandBodyDistance + "," + zRightHandBodyDistance + ")");
            //Console.WriteLine(zLeftHandBodyDistance);
            if (zRightHandBodyDistance > 0.45f && zLeftHandBodyDistance < 0.3f) // Right Hand Moving Cursor
            {
                RightHandActions(body, rightHand, spineMid);
            }
            else if (zRightHandBodyDistance < 0.3f && zLeftHandBodyDistance > 0.4f) // Left Hand Moving cursor
            {
                LeftHandActions(body, lefthand, spineMid);
            }
            else
            {
                // Do Nothing Maybe Lock Cursor Position
                cursorSmoothing = CURSORSMOOTHING;
                wasLeftGrip = false;
                wasRightGrip = false;
                alreadyTrackedPos = false;
            }
        }

        private void LeftHandActions(IBody body, IJoint lefthand, IJoint spineMid)
        {
            //Point cursorPlanePosition = ConvertToDisplayCoordinates(lefthand.Position.X, lefthand.Position.Y);
            //this._previousCursorPosition = this._cursorPosition;
            //this._cursorPosition = cursorPlanePosition;
            //var haltCursorMove = PointDistance(_cursorPosition, _previousCursorPosition) < 5 ? true : false;
            float x = lefthand.Position.X - spineMid.Position.X + 0.3f;
            float y = spineMid.Position.Y - lefthand.Position.Y + 0.3f;
            Point curPos = Cursor.Position;
            double smoothing = 1 - cursorSmoothing;

            //if (!haltCursorMove)
            //MouseControl.SetCursorPos(curPos.X, curPos.Y);
            MouseControl.SetCursorPos((int)(curPos.X + (x * mouseSensitivity * screenWidth - curPos.X) * smoothing), (int)(curPos.Y + ((y + 0.25f) * mouseSensitivity * screenHeight - curPos.Y) * smoothing));
            alreadyTrackedPos = true;
            if (doClick && useGripGesture)
            {
                #region Drag With Left and Single Click
                if(body.HandLeftState == HandState.Closed)
                {
                    if (!mouseInDrag)
                        cursorSmoothing = 0.9999;
                    if (dragMouseGestureState == null)
                    {
                        dragMouseGestureState = new MouseGestureState(PHelper.CurrentTimeMillis());
                        dragMouseGestureState.oldCursorPosition = Cursor.Position;
                        dragMouseGestureState.oldHandPosition = new SpacePoint(x, y, 0);
                    }
                    else
                    {
                        SpacePoint handCurrentPosition = new SpacePoint(x, y, 0);
                        var distanceDiff = PointDistance(dragMouseGestureState.oldHandPosition,handCurrentPosition);
                        if(distanceDiff > 0.09 && !mouseInDrag)
                        {
                            cursorSmoothing = CURSORSMOOTHING;
                            MouseControl.MouseLeftDown();
                            mouseInDrag = true;
                        }
                    }

                    if (clickMouseGestureState == null)
                    {
                        clickMouseGestureState = new MouseGestureState(PHelper.CurrentTimeMillis());
                        clickMouseGestureState.oldCursorPosition = Cursor.Position;
                        clickMouseGestureState.oldHandPosition = new SpacePoint(x, y, 0);
                    }
                    if (doubleClickMouseGestureState == null)
                    {
                        doubleClickMouseGestureState = new MouseGestureState(PHelper.CurrentTimeMillis());
                        doubleClickMouseGestureState.oldCursorPosition = Cursor.Position;
                        doubleClickMouseGestureState.oldHandPosition = new SpacePoint(x, y, 0);
                    }
                    //else
                    //{
                    //    var distanceDiff = PointDistance(doubleClickMouseGestureState.oldCursorPosition, Cursor.Position);
                    //    if (distanceDiff < 4)
                    //    {

                    //    }
                    //}
                }
                else if(body.HandLeftState == HandState.Open)
                {
                    cursorSmoothing = CURSORSMOOTHING;
                    if (mouseInDrag)
                    {
                        MouseControl.MouseLeftUp();
                        mouseInDrag = false;
                        dragMouseGestureState = null;
                    }
                    else {
                        if (clickMouseGestureState != null)
                        {
                            SpacePoint handCurrentPosition = new SpacePoint(x, y, 0);
                            var distanceDiff = PointDistance(clickMouseGestureState.oldHandPosition, handCurrentPosition);
                            //var timeDiff = PHelper.CurrentTimeMillis() - 
                            if (distanceDiff < 0.05)
                            {
                                MouseControl.DoMouseClick();
                                Console.WriteLine("Mouse Double Click");
                            }
                            clickMouseGestureState = null;
                        }
                    }
                }
                //if (body.HandLeftState == HandState.Closed)
                //{
                //    if (!wasLeftGrip)
                //    {
                //        MouseControl.MouseLeftDown();
                //        wasLeftGrip = true;
                //    }
                //}
                //else if (body.HandLeftState == HandState.Open)
                //{
                //    if (wasLeftGrip)
                //    {
                //        MouseControl.MouseLeftUp();
                //        wasLeftGrip = false;
                //    }
                //}
                #endregion
            }
        }

        private void RightHandActions(IBody body, IJoint rightHand, IJoint spineMid)
        {
            //Point cursorPlanePosition = ConvertToDisplayCoordinates(rightHand.Position.X, rightHand.Position.Y);
            //this._previousCursorPosition = this._cursorPosition;
            //this._cursorPosition = cursorPlanePosition;
            //var haltCursorMove = PointDistance(_cursorPosition, _previousCursorPosition) < 5 ? true : false;
            //if (!haltCursorMove)
            //    MouseControl.SetCursorPos(cursorPlanePosition.X, cursorPlanePosition.Y);
            float x = rightHand.Position.X - spineMid.Position.X + 0.1f;
            float y = spineMid.Position.Y - rightHand.Position.Y + 0.18f;
            Point curPos = Cursor.Position;
            double smoothing = 1 - cursorSmoothing;

            //if (!haltCursorMove)
            //MouseControl.SetCursorPos(cursorPlanePosition.X, cursorPlanePosition.Y);
            //MouseControl.SetCursorPos((int)(curPos.X + (x * mouseSensitivity * screenWidth - curPos.X) * smoothing), (int)(curPos.Y + ((y + 0.25f) * mouseSensitivity * screenHeight - curPos.Y) * smoothing));
            MoveMouseTo((int)(curPos.X + (x * mouseSensitivity * screenWidth - curPos.X) * smoothing), (int)(curPos.Y + ((y + 0.25f) * mouseSensitivity * screenHeight - curPos.Y) * smoothing));

            alreadyTrackedPos = true;
            if (doClick && useGripGesture)
            {
                //if (body.HandRightState == HandState.Closed)
                //{
                //    cursorSmoothing = 0.9999;
                //    if (clickRightMouseGestureState == null)
                //    {
                //        clickRightMouseGestureState = new MouseGestureState(PHelper.CurrentTimeMillis());
                //        clickRightMouseGestureState.oldCursorPosition = Cursor.Position;
                //        clickRightMouseGestureState.oldHandPosition = new SpacePoint(x, y, 0);
                //    }
                //}
                //else if (body.HandRightState == HandState.Open)
                //{
                //    cursorSmoothing = CURSORSMOOTHING;
                //    if (clickRightMouseGestureState != null)
                //    {
                //        MouseControl.DoMouseRightClick();
                //        clickRightMouseGestureState = null;
                //    }
                //}
                if (body.HandRightState == HandState.Closed)
                {
                    if (!wasRightGrip)
                    {
                        //Console.WriteLine("Mouse Down");
                        MouseControl.MouseRightDown();
                        wasRightGrip = true;
                        RightDown = true;
                    }
                }
                else if (body.HandRightState == HandState.Open)
                {
                    if (wasRightGrip && RightDown)
                    {
                        //Console.WriteLine("Mouse Up");
                        MouseControl.MouseRightUp();
                        wasRightGrip = false;
                        RightDown = false;
                    }
                }
            }
        }

        #region OtherStuff
        private void MouseLeftClick(HandState handLeftState, IJoint leftHand, IJoint spineMid)
        {
            var zLeftHandBodyDistance = spineMid.Position.Z - leftHand.Position.Z;
            if (zLeftHandBodyDistance > 0.5f) // Right Hand Moving Cursor
            {
                if (handLeftState == HandState.Closed && startLeftHandClickGestureState == null)
                {
                    startLeftHandClickGestureState = new MouseGestureState(PHelper.CurrentTimeMillis());
                    startLeftHandClickGestureState.oldCursorPosition = Cursor.Position;
                }
                else if (handLeftState == HandState.Closed && startLeftHandClickGestureState != null)
                {
                    var cursorMoveDistance = PointDistance(Cursor.Position, startLeftHandClickGestureState.oldCursorPosition);
                    //Console.WriteLine(cursorMoveDistance);
                    if (cursorMoveDistance < 8)
                    {
                        MouseControl.DoMouseClick();
                    }
                }
            }
        }

        private void MouseRightClick(HandState handRightState, IJoint rightHand, IJoint spineMid)
        {
            var zRightHandBodyDistance = spineMid.Position.Z - rightHand.Position.Z;
            if (zRightHandBodyDistance > 0.5f) // Right Hand Moving Cursor
            {
                if (handRightState == HandState.Open && startRightHandClickGestureState == null)
                {
                    startRightHandClickGestureState = new MouseGestureState(PHelper.CurrentTimeMillis());
                    startRightHandClickGestureState.oldCursorPosition = Cursor.Position;
                }
                else if (handRightState == HandState.Open && startRightHandClickGestureState != null)
                {
                    var cursorMoveDistance = PointDistance(Cursor.Position, startRightHandClickGestureState.oldCursorPosition);
                    if (cursorMoveDistance < 8)
                    {
                        var timeDiff = PHelper.CurrentTimeMillis() - startRightHandClickGestureState.TimeStamp;
                        if (timeDiff > 2000)
                        {
                            MouseControl.DoMouseRightClick();
                            startRightHandClickGestureState = null;
                        }
                    }
                    else
                        startRightHandClickGestureState = null;
                }
            }
        }

        private void MoveMouseTo(int x,int y)
        {
            var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var outputX = x * 65535 / screenBounds.Width;
            var outputY = y * 65535 / screenBounds.Height;
            MouseControl.MoveTo(outputX, outputY);
        }

        private double PointDistance(Point p1, Point p2)
        {
            double distance = Math.Round(Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2)), 1);
            return distance;
        }

        private double PointDistance(SpacePoint p1, SpacePoint p2)
        {
            double distance = Math.Round(Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2)), 5);
            return distance;
        }

        private Point ConvertToDisplayCoordinates(float x, float y)
        {
            var displayXDimension = Screen.PrimaryScreen.Bounds.Width;
            var displayYDimension = Screen.PrimaryScreen.Bounds.Height;

            var newYcoord = (y / yKinectTop - yKinectBottom) * displayYDimension;
            var newXcoord = (x / xKinectRight - xKinectLeft) * displayXDimension;
            return new Point((int)newXcoord,-(int)newYcoord);
        }

        private void MouseScroll(IBody body, IJoint rightHand, IJoint spineMid)
        {
            var zRightHandBodyDistance = spineMid.Position.Z - rightHand.Position.Z;
            if (zRightHandBodyDistance > 0.5f)
            {
                if (body.HandRightState == HandState.Closed)
                {
                    if (startScrollGesture == null)
                    {
                        startScrollGesture = new MouseGestureState(PHelper.CurrentTimeMillis());
                        startScrollGesture.oldCursorPosition = Cursor.Position;
                        mouseInScroll = true;
                    }

                    if (startScrollClickGesture == null)
                    {
                        startScrollClickGesture = new MouseGestureState(PHelper.CurrentTimeMillis());
                        startScrollClickGesture.oldCursorPosition = Cursor.Position;
                    }
                    else
                    {
                        var currentTime = PHelper.CurrentTimeMillis();
                        var timeDiff = currentTime - startScrollClickGesture.TimeStamp;
                        if(timeDiff > 1000)
                        {
                            MouseControl.ScrollClick();
                            startScrollGesture = null;
                            startScrollClickGesture = null;
                            mouseInScroll = false;
                        }
                    }
                }
                else if (body.HandRightState == HandState.Open)
                {
                    if (startScrollGesture != null)
                    {
                        var currentPosition = Cursor.Position;
                        var yMovement = -1 * (currentPosition.Y - startScrollGesture.oldCursorPosition.Y);
                        var times = 10;
                        MouseControl.Scroll(times);
                        startScrollGesture = null;
                        mouseInScroll = false;
                    }
                }
                else
                {
                    //ignore
                }
            }
        }
        #endregion

        IJoint getJoint(IBody body,JointType jointName)
        {
            return (from joint in body.Joints
                        where joint.Key == jointName
                        select joint.Value).FirstOrDefault();
        }
        
    }
}
