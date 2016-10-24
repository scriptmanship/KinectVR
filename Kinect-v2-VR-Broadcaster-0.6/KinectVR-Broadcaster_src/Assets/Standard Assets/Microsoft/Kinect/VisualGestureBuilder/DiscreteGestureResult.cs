using RootSystem = System;
using System.Linq;
using System.Collections.Generic;
namespace Microsoft.Kinect.VisualGestureBuilder
{
    //
    // Microsoft.Kinect.VisualGestureBuilder.DiscreteGestureResult
    //
    public sealed partial class DiscreteGestureResult : Helper.INativeWrapper

    {
        internal RootSystem.IntPtr _pNative;
        RootSystem.IntPtr Helper.INativeWrapper.nativePtr { get { return _pNative; } }

        // Constructors and Finalizers
        internal DiscreteGestureResult(RootSystem.IntPtr pNative)
        {
            _pNative = pNative;
            Microsoft_Kinect_VisualGestureBuilder_DiscreteGestureResult_AddRefObject(ref _pNative);
        }

        ~DiscreteGestureResult()
        {
            Dispose(false);
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_DiscreteGestureResult_ReleaseObject(ref RootSystem.IntPtr pNative);
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_DiscreteGestureResult_AddRefObject(ref RootSystem.IntPtr pNative);
        private void Dispose(bool disposing)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                return;
            }

            __EventCleanup();

            Helper.NativeObjectCache.RemoveObject<DiscreteGestureResult>(_pNative);
                Microsoft_Kinect_VisualGestureBuilder_DiscreteGestureResult_ReleaseObject(ref _pNative);

            _pNative = RootSystem.IntPtr.Zero;
        }


        // Public Properties
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern float Microsoft_Kinect_VisualGestureBuilder_DiscreteGestureResult_get_Confidence(RootSystem.IntPtr pNative);
        public  float Confidence
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("DiscreteGestureResult");
                }

                return Microsoft_Kinect_VisualGestureBuilder_DiscreteGestureResult_get_Confidence(_pNative);
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern bool Microsoft_Kinect_VisualGestureBuilder_DiscreteGestureResult_get_Detected(RootSystem.IntPtr pNative);
        public  bool Detected
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("DiscreteGestureResult");
                }

                return Microsoft_Kinect_VisualGestureBuilder_DiscreteGestureResult_get_Detected(_pNative);
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern bool Microsoft_Kinect_VisualGestureBuilder_DiscreteGestureResult_get_FirstFrameDetected(RootSystem.IntPtr pNative);
        public  bool FirstFrameDetected
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("DiscreteGestureResult");
                }

                return Microsoft_Kinect_VisualGestureBuilder_DiscreteGestureResult_get_FirstFrameDetected(_pNative);
            }
        }

        private void __EventCleanup()
        {
        }
    }

}
