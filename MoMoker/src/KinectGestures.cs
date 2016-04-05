﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectEx;
using KinectEx.Smoothing;
using Microsoft.Kinect;
using bbv.Common.StateMachine;

namespace MoMoker.src
{
    class KinectGestures
    {
        KinectSensor _sensor = null;
        BodyFrameReader _bodyReader = null;

        List<CustomBody> _bodies = null;
        SmoothedBodyList<KalmanSmoother> _kalmanBodies = null;
        SmoothedBodyList<ExponentialSmoother> _exponentialBodies = new SmoothedBodyList<ExponentialSmoother>();

        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler leftHandRotatedRightWays;
        

        public KinectGestures()
        {
            leftHandRotatedRightWays += new ChangedEventHandler(LeftHandRotateHandler);

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
            IJoint rightWrist = getJoint(body,JointType.WristRight);
            IJoint leftWrist = getJoint(body, JointType.WristLeft);
            IJoint spineMid = getJoint(body, JointType.SpineMid);
            
            if (leftHandRotatedRightWays != null)
                leftHandRotatedRightWays(body.Joints.Select(joint => joint.Key == JointType.WristRight),EventArgs.Empty);
        }

        IJoint getJoint(IBody body,JointType jointName)
        {
            return (from joint in body.Joints
                        where joint.Key == jointName
                        select joint.Value).FirstOrDefault();
        }

        private void print(IEnumerable<IBody> bodies)
        {
            foreach (var item in bodies)
            {
                Console.WriteLine(item.Joints.Select(joint => joint.Value).Where(joint => joint.JointType.Equals(JointType.WristRight)).First().Position.X);
            }
        }
    }
}
