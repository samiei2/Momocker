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

namespace MoMoker.src
{
    class KinectGestures
    {
        KinectSensor _sensor = null;
        BodyFrameReader _bodyReader = null;

        List<CustomBody> _bodies = null;
        SmoothedBodyList<KalmanSmoother> _kalmanBodies = null;
        SmoothedBodyList<ExponentialSmoother> _exponentialBodies = new SmoothedBodyList<ExponentialSmoother>();
        private GestureState rotateInitState;
        private readonly float zRotateDelta;
        private readonly float yRotateDelta;
        private readonly float xRotateDelta;
        private GestureState startScrollGesture;
        private GestureState startScrollClickGesture;

        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler rightHandRotatedRightWays;
        

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
                    frame.GetAndRefreshBodyData(_exponentialBodies);
                    bodies = _exponentialBodies;
                }
            }

            if (bodies != null)
            {
                bodies.MapDepthPositions();
                detectGestures(bodies.FirstOrDefault());
            }
        }

        private void detectGestures(IBody body)
        {
            IJoint rightHand = getJoint(body, JointType.HandRight);
            IJoint leftHand = getJoint(body, JointType.HandLeft);
            IJoint spineMid = getJoint(body, JointType.SpineMid);

            DetectScrollGestures(body, rightHand, spineMid);
        }

        private void DetectScrollGestures(IBody body, IJoint rightHand, IJoint spineMid)
        {
            var zRightHandBodyDistance = rightHand.Position.Z - spineMid.Position.Z;
            if (zRightHandBodyDistance > 0.2f)
            {
                if (body.HandRightState == HandState.Closed)
                {
                    Console.WriteLine(body.HandRightConfidence);
                    if (startScrollGesture == null)
                    {
                        startScrollGesture = new ScrollGestureState(PHelper.CurrentTimeMillis());
                        startScrollGesture.oldCursorPosition = Cursor.Position;
                    }

                    if (startScrollClickGesture == null)
                    {
                        startScrollClickGesture = new ScrollGestureState(PHelper.CurrentTimeMillis());
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
                        }
                    }
                }
                else if (body.HandRightState == HandState.Open)
                {
                    if (startScrollGesture != null)
                    {
                        var currentPosition = Cursor.Position;
                        var yMovement = -1 * (currentPosition.Y - startScrollGesture.oldCursorPosition.Y);
                        if (yMovement > 0.1)
                        {
                            var times = 10;
                            MouseControl.Scroll(times);
                            startScrollGesture = null;
                        }
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
