using RootSystem = System;
using System.Linq;
using System.Collections.Generic;
namespace Microsoft.Kinect.VisualGestureBuilder
{
    //
    // Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrame
    //
    public sealed partial class VisualGestureBuilderFrame : RootSystem.IDisposable, Helper.INativeWrapper

    {
        internal RootSystem.IntPtr _pNative;
        RootSystem.IntPtr Helper.INativeWrapper.nativePtr { get { return _pNative; } }

        // Constructors and Finalizers
        internal VisualGestureBuilderFrame(RootSystem.IntPtr pNative)
        {
            _pNative = pNative;
            Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_AddRefObject(ref _pNative);
        }

        ~VisualGestureBuilderFrame()
        {
            Dispose(false);
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_ReleaseObject(ref RootSystem.IntPtr pNative);
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_AddRefObject(ref RootSystem.IntPtr pNative);
        private void Dispose(bool disposing)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                return;
            }

            __EventCleanup();

            Helper.NativeObjectCache.RemoveObject<VisualGestureBuilderFrame>(_pNative);

            if (disposing)
            {
                Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_Dispose(_pNative);
            }
                Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_ReleaseObject(ref _pNative);

            _pNative = RootSystem.IntPtr.Zero;
        }


        // Public Properties
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern int Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_ContinuousGestureResults(RootSystem.IntPtr pNative, [RootSystem.Runtime.InteropServices.Out] RootSystem.IntPtr[] outKeys, [RootSystem.Runtime.InteropServices.Out] RootSystem.IntPtr[] outValues, int outCollectionSize);
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern int Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_ContinuousGestureResults_Length(RootSystem.IntPtr pNative);
        public  RootSystem.Collections.Generic.Dictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, Microsoft.Kinect.VisualGestureBuilder.ContinuousGestureResult> ContinuousGestureResults
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrame");
                }

                int outCollectionSize = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_ContinuousGestureResults_Length(_pNative);
                var outKeys = new RootSystem.IntPtr[outCollectionSize];
                var outValues = new RootSystem.IntPtr[outCollectionSize];
                var managedDictionary = new RootSystem.Collections.Generic.Dictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, Microsoft.Kinect.VisualGestureBuilder.ContinuousGestureResult>();

                outCollectionSize = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_ContinuousGestureResults(_pNative, outKeys, outValues, outCollectionSize);
                Helper.ExceptionHelper.CheckLastError();
                for(int i=0;i<outCollectionSize;i++)
                {
                    if(outKeys[i] == RootSystem.IntPtr.Zero || outValues[i] == RootSystem.IntPtr.Zero)
                    {
                        continue;
                    }

                    var keyObj = Helper.NativeObjectCache.CreateOrGetObject<Microsoft.Kinect.VisualGestureBuilder.Gesture>(outKeys[i], n => new Microsoft.Kinect.VisualGestureBuilder.Gesture(n));

                    var valueObj = Helper.NativeObjectCache.CreateOrGetObject<Microsoft.Kinect.VisualGestureBuilder.ContinuousGestureResult>(outValues[i], n => new Microsoft.Kinect.VisualGestureBuilder.ContinuousGestureResult(n));

                    managedDictionary.Add(keyObj, valueObj);
                }
                return managedDictionary;
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern int Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_DiscreteGestureResults(RootSystem.IntPtr pNative, [RootSystem.Runtime.InteropServices.Out] RootSystem.IntPtr[] outKeys, [RootSystem.Runtime.InteropServices.Out] RootSystem.IntPtr[] outValues, int outCollectionSize);
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern int Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_DiscreteGestureResults_Length(RootSystem.IntPtr pNative);
        public  RootSystem.Collections.Generic.Dictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, Microsoft.Kinect.VisualGestureBuilder.DiscreteGestureResult> DiscreteGestureResults
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrame");
                }

                int outCollectionSize = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_DiscreteGestureResults_Length(_pNative);
                var outKeys = new RootSystem.IntPtr[outCollectionSize];
                var outValues = new RootSystem.IntPtr[outCollectionSize];
                var managedDictionary = new RootSystem.Collections.Generic.Dictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, Microsoft.Kinect.VisualGestureBuilder.DiscreteGestureResult>();

                outCollectionSize = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_DiscreteGestureResults(_pNative, outKeys, outValues, outCollectionSize);
                Helper.ExceptionHelper.CheckLastError();
                for(int i=0;i<outCollectionSize;i++)
                {
                    if(outKeys[i] == RootSystem.IntPtr.Zero || outValues[i] == RootSystem.IntPtr.Zero)
                    {
                        continue;
                    }

                    var keyObj = Helper.NativeObjectCache.CreateOrGetObject<Microsoft.Kinect.VisualGestureBuilder.Gesture>(outKeys[i], n => new Microsoft.Kinect.VisualGestureBuilder.Gesture(n));

                    var valueObj = Helper.NativeObjectCache.CreateOrGetObject<Microsoft.Kinect.VisualGestureBuilder.DiscreteGestureResult>(outValues[i], n => new Microsoft.Kinect.VisualGestureBuilder.DiscreteGestureResult(n));

                    managedDictionary.Add(keyObj, valueObj);
                }
                return managedDictionary;
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern bool Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_IsTrackingIdValid(RootSystem.IntPtr pNative);
        public  bool IsTrackingIdValid
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrame");
                }

                return Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_IsTrackingIdValid(_pNative);
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern long Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_RelativeTime(RootSystem.IntPtr pNative);
        public  RootSystem.TimeSpan RelativeTime
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrame");
                }

                return RootSystem.TimeSpan.FromMilliseconds(Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_RelativeTime(_pNative));
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern ulong Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_TrackingId(RootSystem.IntPtr pNative);
        public  ulong TrackingId
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrame");
                }

                return Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_TrackingId(_pNative);
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern RootSystem.IntPtr Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_VisualGestureBuilderFrameSource(RootSystem.IntPtr pNative);
        public  Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameSource VisualGestureBuilderFrameSource
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrame");
                }

                RootSystem.IntPtr objectPointer = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_get_VisualGestureBuilderFrameSource(_pNative);
                Helper.ExceptionHelper.CheckLastError();
                if (objectPointer == RootSystem.IntPtr.Zero)
                {
                    return null;
                }

                return Helper.NativeObjectCache.CreateOrGetObject<Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameSource>(objectPointer, n => new Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameSource(n));
            }
        }


        // Public Methods
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrame_Dispose(RootSystem.IntPtr pNative);
        public void Dispose()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                return;
            }

            Dispose(true);
            RootSystem.GC.SuppressFinalize(this);
        }

        private void __EventCleanup()
        {
        }
    }

}
