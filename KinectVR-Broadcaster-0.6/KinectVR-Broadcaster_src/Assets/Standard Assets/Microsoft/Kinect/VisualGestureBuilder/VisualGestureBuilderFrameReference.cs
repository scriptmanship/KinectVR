using RootSystem = System;
using System.Linq;
using System.Collections.Generic;
namespace Microsoft.Kinect.VisualGestureBuilder
{
    //
    // Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameReference
    //
    public sealed partial class VisualGestureBuilderFrameReference : Helper.INativeWrapper

    {
        internal RootSystem.IntPtr _pNative;
        RootSystem.IntPtr Helper.INativeWrapper.nativePtr { get { return _pNative; } }

        // Constructors and Finalizers
        internal VisualGestureBuilderFrameReference(RootSystem.IntPtr pNative)
        {
            _pNative = pNative;
            Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameReference_AddRefObject(ref _pNative);
        }

        ~VisualGestureBuilderFrameReference()
        {
            Dispose(false);
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameReference_ReleaseObject(ref RootSystem.IntPtr pNative);
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameReference_AddRefObject(ref RootSystem.IntPtr pNative);
        private void Dispose(bool disposing)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                return;
            }

            __EventCleanup();

            Helper.NativeObjectCache.RemoveObject<VisualGestureBuilderFrameReference>(_pNative);
                Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameReference_ReleaseObject(ref _pNative);

            _pNative = RootSystem.IntPtr.Zero;
        }


        // Public Properties
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern long Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameReference_get_RelativeTime(RootSystem.IntPtr pNative);
        public  RootSystem.TimeSpan RelativeTime
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameReference");
                }

                return RootSystem.TimeSpan.FromMilliseconds(Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameReference_get_RelativeTime(_pNative));
            }
        }


        // Public Methods
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern RootSystem.IntPtr Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameReference_AcquireFrame(RootSystem.IntPtr pNative);
        public Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrame AcquireFrame()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameReference");
            }

            RootSystem.IntPtr objectPointer = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameReference_AcquireFrame(_pNative);
            Helper.ExceptionHelper.CheckLastError();
            if (objectPointer == RootSystem.IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrame>(objectPointer, n => new Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrame(n));
        }

        private void __EventCleanup()
        {
        }
    }

}
