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
        private double mouseSensitivity = 3.0f;
        private int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        private int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        private double cursorSmoothing = 0.4f;

        public KinectGestures()
        {
            rightHandRotatedRightWays += new ChangedEventHandler(LeftHandRotateHandler);

            _sensor = KinectSensor.GetDefault();

            _bodyReader = _sensor.BodyFrameSource.OpenReader();
            _bodyReader.FrameArrived += _bodyReader_FrameArrived;
            _sensor.Open();
        }

        private void LeftHandRotateHandler(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void _bodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            IEnumerable<IBody> bodies = null; // to make the GetBitmap call a little cleaner
            using (BodyFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.GetAndRefreshBodyData(_kalmanBodies);
                    bodies = _kalmanBodies;
                }
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
            IJoint rightHand = getJoint(body, JointType.HandRight);
            IJoint leftHand = getJoint(body, JointType.HandLeft);
            IJoint spineBase = getJoint(body, JointType.SpineMid);
            MoveMouse(body, leftHand, rightHand, spineBase);
            MouseLeftClick(body.HandLeftState, leftHand, spineBase);
            MouseRightClick(body.HandRightState, rightHand, spineBase);
            MouseScroll(body, rightHand, spineBase);
        }

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
                else if(handRightState == HandState.Open && startRightHandClickGestureState != null)
                {
                    var cursorMoveDistance = PointDistance(Cursor.Position,startRightHandClickGestureState.oldCursorPosition);
                    if (cursorMoveDistance < 8)
                    {
                        var timeDiff = PHelper.CurrentTimeMillis() - startRightHandClickGestureState.TimeStamp;
                        if(timeDiff > 3000)
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

        private void MoveMouse(IBody body, IJoint lefthand, IJoint rightHand, IJoint spineMid)
        {
            var zRightHandBodyDistance = spineMid.Position.Z - rightHand.Position.Z;
            var zLeftHandBodyDistance = spineMid.Position.Z - lefthand.Position.Z;
            //Console.WriteLine(zLeftHandBodyDistance);
            if (zRightHandBodyDistance > 0.5f && zLeftHandBodyDistance < 0.4f) // Right Hand Moving Cursor
            {
                //Point cursorPlanePosition = ConvertToDisplayCoordinates(rightHand.Position.X, rightHand.Position.Y);
                //this._previousCursorPosition = this._cursorPosition;
                //this._cursorPosition = cursorPlanePosition;
                //var haltCursorMove = PointDistance(_cursorPosition, _previousCursorPosition) < 5 ? true : false;
                //if (!haltCursorMove)
                //    MouseControl.SetCursorPos(cursorPlanePosition.X, cursorPlanePosition.Y);
                float x = rightHand.Position.X - spineMid.Position.X;
                float y = spineMid.Position.Y - rightHand.Position.Y;
                Point curPos = Cursor.Position;
                double smoothing = 1 - cursorSmoothing;

                //if (!haltCursorMove)
                //MouseControl.SetCursorPos(cursorPlanePosition.X, cursorPlanePosition.Y);
                MouseControl.SetCursorPos((int)(curPos.X + (x * mouseSensitivity * screenWidth - curPos.X) * smoothing), (int)(curPos.Y + ((y + 0.25f) * mouseSensitivity * screenHeight - curPos.Y) * smoothing));
            }
            else if (zRightHandBodyDistance < 0.4f && zLeftHandBodyDistance > 0.5f) // Left Hand Moving cursor
            {
                //Point cursorPlanePosition = ConvertToDisplayCoordinates(lefthand.Position.X, lefthand.Position.Y);
                //this._previousCursorPosition = this._cursorPosition;
                //this._cursorPosition = cursorPlanePosition;
                //var haltCursorMove = PointDistance(_cursorPosition, _previousCursorPosition) < 5 ? true : false;
                float x = lefthand.Position.X - spineMid.Position.X;
                float y = spineMid.Position.Y - lefthand.Position.Y;
                Point curPos = Cursor.Position;
                double smoothing = 1 - cursorSmoothing;

                //if (!haltCursorMove)
                    //MouseControl.SetCursorPos(cursorPlanePosition.X, cursorPlanePosition.Y);
                MouseControl.SetCursorPos((int)(curPos.X + (x * mouseSensitivity * screenWidth - curPos.X) * smoothing), (int)(curPos.Y + ((y + 0.25f) * mouseSensitivity * screenHeight - curPos.Y) * smoothing));
            }
            else
            {
                // Do Nothing Maybe Lock Cursor Position
            }
        }

        private double PointDistance(Point p1, Point p2)
        {
            double distance = Math.Round(Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2)), 1);
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

        IJoint getJoint(IBody body,JointType jointName)
        {
            return (from joint in body.Joints
                        where joint.Key == jointName
                        select joint.Value).FirstOrDefault();
        }
        
    }
}
