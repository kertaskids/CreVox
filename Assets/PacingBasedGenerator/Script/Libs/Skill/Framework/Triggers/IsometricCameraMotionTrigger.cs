using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Trigger to change IsometricCamera
    /// </summary> 
    public class IsometricCameraMotionTrigger : Trigger
    {
        /// <summary> reference to IsometricCameraMotion attached to camera </summary>
        public IsometricCameraMotion Motion;

        /// <summary> faild of view </summary>
        public float Fov = 60;
        /// <summary> Rotation angle around target ( 0 - 360) </summary>
        public float AroundAngle = 0;
        /// <summary> Rotation angle behind target( 0 - 90). 0 is completely horizontal to target and 90 is completely vertical to target. </summary>
        public float LookAngle = 0;
        /// <summary> Camera moves by mouse when mouse position gets far from center of screen. </summary>
        public float Preview = 2.0f;
        /// <summary> Minimum distance to target when PointOfIntrest is close to target</summary>
        public float ZoomIn = 8;
        /// <summary> Maximum distance to target when PointOfIntrest is far from target</summary>
        public float ZoomOut = 16;
        /// <summary> Apply relative custom offset to position of camera </summary>
        public Vector3 CustomOffset = Vector3.zero;
        /// <summary> length of motion to reach this values</summary>
        public float MotionTime = 0.5f;


        private static IsometricCameraMotionTrigger _LastTrigger;


        protected override void Awake()
        {
            base.Awake();
            if (Motion == null)
            {
                Camera[] allCameras = UnityEngine.Camera.allCameras;
                foreach (var c in allCameras)
                {
                    Motion = c.GetComponent<IsometricCameraMotion>();
                    if (Motion != null)
                        break;
                }
            }
        }

        /// <summary>
        /// On enter trigger
        /// </summary>
        /// <param name="other"> other collider</param>
        /// <returns>true if trigger accepted, otherwise false</returns>
        protected override bool OnEnter(UnityEngine.Collider other)
        {
            if (Motion != null)
            {
                if (_LastTrigger != this)
                {
                    _LastTrigger = this;
                    Apply();
                }
                return true;
            }
            else
            {
                Debug.LogError("Miisin reference to IsometricCameraMotion");
            }
            return false;
        }

        public void Apply(bool fast = false)
        {
            if (Motion != null)
            {
                if (fast && Motion.Camera != null)
                {
                    Motion.Camera.Fov = this.Fov;
                    Motion.Camera.AroundAngle = this.AroundAngle;
                    Motion.Camera.LookAngle = this.LookAngle;
                    Motion.Camera.CameraPreview = this.Preview;
                    Motion.Camera.ZoomIn = this.ZoomIn;
                    Motion.Camera.ZoomOut = this.ZoomOut;
                    Motion.Camera.CustomOffset = this.CustomOffset;
                    Motion.Camera.FastUpdate();
                }
                else
                {
                    Motion.MotionFov(this.Fov, MotionTime);
                    Motion.MotionAroundAngle(this.AroundAngle, MotionTime);
                    Motion.MotionLookAngle(this.LookAngle, MotionTime);
                    Motion.MotionPreview(this.Preview, MotionTime);
                    Motion.MotionZoomIn(this.ZoomIn, MotionTime);
                    Motion.MotionZoomOut(this.ZoomOut, MotionTime);
                    Motion.MotionCustomOffset(this.CustomOffset, MotionTime);
                }
            }
        }
    }
}
