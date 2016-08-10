using RootSystem = System;
using System.Linq;
using System.Collections.Generic;
namespace Microsoft.Kinect.VisualGestureBuilder
{
    //
    // Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameArrivedEventArgs
    //
    public sealed partial class VisualGestureBuilderFrameArrivedEventArgs : RootSystem.EventArgs, Helper.INativeWrapper

    {
        internal RootSystem.IntPtr _pNative;
        RootSystem.IntPtr Helper.INativeWrapper.nativePtr { get { return _pNative; } }

        // Constructors and Finalizers
        internal VisualGestureBuilderFrameArrivedEventArgs(RootSystem.IntPtr pNative)
        {
            _pNative = pNative;
            Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameArrivedEventArgs_AddRefObject(ref _pNative);
        }

        ~VisualGestureBuilderFrameArrivedEventArgs()
        {
            Dispose(false);
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameArrivedEventArgs_ReleaseObject(ref RootSystem.IntPtr pNative);
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameArrivedEventArgs_AddRefObject(ref RootSystem.IntPtr pNative);
        private void Dispose(bool disposing)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                return;
            }

            __EventCleanup();

            Helper.NativeObjectCache.RemoveObject<VisualGestureBuilderFrameArrivedEventArgs>(_pNative);
                Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameArrivedEventArgs_ReleaseObject(ref _pNative);

            _pNative = RootSystem.IntPtr.Zero;
        }


        // Public Properties
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern RootSystem.IntPtr Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameArrivedEventArgs_get_FrameReference(RootSystem.IntPtr pNative);
        public  Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameReference FrameReference
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameArrivedEventArgs");
                }

                RootSystem.IntPtr objectPointer = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameArrivedEventArgs_get_FrameReference(_pNative);
                Helper.ExceptionHelper.CheckLastError();
                if (objectPointer == RootSystem.IntPtr.Zero)
                {
                    return null;
                }

                return Helper.NativeObjectCache.CreateOrGetObject<Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameReference>(objectPointer, n => new Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameReference(n));
            }
        }

        private void __EventCleanup()
        {
        }
    }

}
