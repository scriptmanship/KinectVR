using RootSystem = System;
using System.Linq;
using System.Collections.Generic;
namespace Microsoft.Kinect.VisualGestureBuilder
{
    //
    // Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameSource
    //
    public sealed partial class VisualGestureBuilderFrameSource : RootSystem.IDisposable, Helper.INativeWrapper

    {
        internal RootSystem.IntPtr _pNative;
        RootSystem.IntPtr Helper.INativeWrapper.nativePtr { get { return _pNative; } }

        // Constructors and Finalizers
        internal VisualGestureBuilderFrameSource(RootSystem.IntPtr pNative)
        {
            _pNative = pNative;
            Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_AddRefObject(ref _pNative);
        }

        ~VisualGestureBuilderFrameSource()
        {
            Dispose(false);
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_ReleaseObject(ref RootSystem.IntPtr pNative);
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_AddRefObject(ref RootSystem.IntPtr pNative);
        private void Dispose(bool disposing)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                return;
            }

            __EventCleanup();

            Helper.NativeObjectCache.RemoveObject<VisualGestureBuilderFrameSource>(_pNative);

            if (disposing)
            {
                Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_Dispose(_pNative);
            }
                Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_ReleaseObject(ref _pNative);

            _pNative = RootSystem.IntPtr.Zero;
        }


        // Public Properties
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern ulong Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_TrackingId(RootSystem.IntPtr pNative);
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_put_TrackingId(RootSystem.IntPtr pNative, ulong trackingId);
        public  ulong TrackingId
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
                }

                return Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_TrackingId(_pNative);
            }
            set
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
                }

                Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_put_TrackingId(_pNative, value);
                Helper.ExceptionHelper.CheckLastError();
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern bool Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_HorizontalMirror(RootSystem.IntPtr pNative);
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_put_HorizontalMirror(RootSystem.IntPtr pNative, bool horizontalMirror);
        public  bool HorizontalMirror
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
                }

                return Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_HorizontalMirror(_pNative);
            }
            set
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
                }

                Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_put_HorizontalMirror(_pNative, value);
                Helper.ExceptionHelper.CheckLastError();
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern int Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_Gestures(RootSystem.IntPtr pNative, [RootSystem.Runtime.InteropServices.Out] RootSystem.IntPtr[] outCollection, int outCollectionSize);
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern int Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_Gestures_Length(RootSystem.IntPtr pNative);
        public  RootSystem.Collections.Generic.IList<Microsoft.Kinect.VisualGestureBuilder.Gesture> Gestures
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
                }

                int outCollectionSize = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_Gestures_Length(_pNative);
                var outCollection = new RootSystem.IntPtr[outCollectionSize];
                var managedCollection = new Microsoft.Kinect.VisualGestureBuilder.Gesture[outCollectionSize];

                outCollectionSize = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_Gestures(_pNative, outCollection, outCollectionSize);
                Helper.ExceptionHelper.CheckLastError();
                for(int i=0;i<outCollectionSize;i++)
                {
                    if(outCollection[i] == RootSystem.IntPtr.Zero)
                    {
                        continue;
                    }

                    var obj = Helper.NativeObjectCache.CreateOrGetObject<Microsoft.Kinect.VisualGestureBuilder.Gesture>(outCollection[i], n => new Microsoft.Kinect.VisualGestureBuilder.Gesture(n));

                    managedCollection[i] = obj;
                }
                return managedCollection;
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern bool Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_IsActive(RootSystem.IntPtr pNative);
        public  bool IsActive
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
                }

                return Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_IsActive(_pNative);
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern bool Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_IsTrackingIdValid(RootSystem.IntPtr pNative);
        public  bool IsTrackingIdValid
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
                }

                return Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_IsTrackingIdValid(_pNative);
            }
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern RootSystem.IntPtr Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_KinectSensor(RootSystem.IntPtr pNative);
        public  Windows.Kinect.KinectSensor KinectSensor
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
                }

                RootSystem.IntPtr objectPointer = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_get_KinectSensor(_pNative);
                Helper.ExceptionHelper.CheckLastError();
                if (objectPointer == RootSystem.IntPtr.Zero)
                {
                    return null;
                }

                return Helper.NativeObjectCache.CreateOrGetObject<Windows.Kinect.KinectSensor>(objectPointer, n => new Windows.Kinect.KinectSensor(n));
            }
        }


        // Events
        private static RootSystem.Runtime.InteropServices.GCHandle _Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_Handle;
        [RootSystem.Runtime.InteropServices.UnmanagedFunctionPointer(RootSystem.Runtime.InteropServices.CallingConvention.Cdecl)]
        private delegate void _Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate(RootSystem.IntPtr args, RootSystem.IntPtr pNative);
        private static Helper.CollectionMap<RootSystem.IntPtr, List<RootSystem.EventHandler<Microsoft.Kinect.VisualGestureBuilder.TrackingIdLostEventArgs>>> Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_callbacks = new Helper.CollectionMap<RootSystem.IntPtr, List<RootSystem.EventHandler<Microsoft.Kinect.VisualGestureBuilder.TrackingIdLostEventArgs>>>();
        [AOT.MonoPInvokeCallbackAttribute(typeof(_Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate))]
        private static void Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_Handler(RootSystem.IntPtr result, RootSystem.IntPtr pNative)
        {
            List<RootSystem.EventHandler<Microsoft.Kinect.VisualGestureBuilder.TrackingIdLostEventArgs>> callbackList = null;
            Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_callbacks.TryGetValue(pNative, out callbackList);
            lock(callbackList)
            {
                var objThis = Helper.NativeObjectCache.GetObject<VisualGestureBuilderFrameSource>(pNative);
                var args = new Microsoft.Kinect.VisualGestureBuilder.TrackingIdLostEventArgs(result);
                foreach(var func in callbackList)
                {
                    Helper.EventPump.Instance.Enqueue(() => { try { func(objThis, args); } catch { } });
                }
            }
        }
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_add_TrackingIdLost(RootSystem.IntPtr pNative, _Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate eventCallback, bool unsubscribe);
        public  event RootSystem.EventHandler<Microsoft.Kinect.VisualGestureBuilder.TrackingIdLostEventArgs> TrackingIdLost
        {
            add
            {
                Helper.EventPump.EnsureInitialized();

                Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_callbacks.TryAddDefault(_pNative);
                var callbackList = Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_callbacks[_pNative];
                lock (callbackList)
                {
                    callbackList.Add(value);
                    if(callbackList.Count == 1)
                    {
                        var del = new _Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate(Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_Handler);
                        _Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_Handle = RootSystem.Runtime.InteropServices.GCHandle.Alloc(del);
                        Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_add_TrackingIdLost(_pNative, del, false);
                    }
                }
            }
            remove
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    return;
                }

                Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_callbacks.TryAddDefault(_pNative);
                var callbackList = Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_callbacks[_pNative];
                lock (callbackList)
                {
                    callbackList.Remove(value);
                    if(callbackList.Count == 0)
                    {
                        Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_add_TrackingIdLost(_pNative, Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_Handler, true);
                        _Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_Handle.Free();
                    }
                }
            }
        }

        private static RootSystem.Runtime.InteropServices.GCHandle _Windows_Data_PropertyChangedEventArgs_Delegate_Handle;
        [RootSystem.Runtime.InteropServices.UnmanagedFunctionPointer(RootSystem.Runtime.InteropServices.CallingConvention.Cdecl)]
        private delegate void _Windows_Data_PropertyChangedEventArgs_Delegate(RootSystem.IntPtr args, RootSystem.IntPtr pNative);
        private static Helper.CollectionMap<RootSystem.IntPtr, List<RootSystem.EventHandler<Windows.Data.PropertyChangedEventArgs>>> Windows_Data_PropertyChangedEventArgs_Delegate_callbacks = new Helper.CollectionMap<RootSystem.IntPtr, List<RootSystem.EventHandler<Windows.Data.PropertyChangedEventArgs>>>();
        [AOT.MonoPInvokeCallbackAttribute(typeof(_Windows_Data_PropertyChangedEventArgs_Delegate))]
        private static void Windows_Data_PropertyChangedEventArgs_Delegate_Handler(RootSystem.IntPtr result, RootSystem.IntPtr pNative)
        {
            List<RootSystem.EventHandler<Windows.Data.PropertyChangedEventArgs>> callbackList = null;
            Windows_Data_PropertyChangedEventArgs_Delegate_callbacks.TryGetValue(pNative, out callbackList);
            lock(callbackList)
            {
                var objThis = Helper.NativeObjectCache.GetObject<VisualGestureBuilderFrameSource>(pNative);
                var args = new Windows.Data.PropertyChangedEventArgs(result);
                foreach(var func in callbackList)
                {
                    Helper.EventPump.Instance.Enqueue(() => { try { func(objThis, args); } catch { } });
                }
            }
        }
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_add_PropertyChanged(RootSystem.IntPtr pNative, _Windows_Data_PropertyChangedEventArgs_Delegate eventCallback, bool unsubscribe);
        public  event RootSystem.EventHandler<Windows.Data.PropertyChangedEventArgs> PropertyChanged
        {
            add
            {
                Helper.EventPump.EnsureInitialized();

                Windows_Data_PropertyChangedEventArgs_Delegate_callbacks.TryAddDefault(_pNative);
                var callbackList = Windows_Data_PropertyChangedEventArgs_Delegate_callbacks[_pNative];
                lock (callbackList)
                {
                    callbackList.Add(value);
                    if(callbackList.Count == 1)
                    {
                        var del = new _Windows_Data_PropertyChangedEventArgs_Delegate(Windows_Data_PropertyChangedEventArgs_Delegate_Handler);
                        _Windows_Data_PropertyChangedEventArgs_Delegate_Handle = RootSystem.Runtime.InteropServices.GCHandle.Alloc(del);
                        Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_add_PropertyChanged(_pNative, del, false);
                    }
                }
            }
            remove
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    return;
                }

                Windows_Data_PropertyChangedEventArgs_Delegate_callbacks.TryAddDefault(_pNative);
                var callbackList = Windows_Data_PropertyChangedEventArgs_Delegate_callbacks[_pNative];
                lock (callbackList)
                {
                    callbackList.Remove(value);
                    if(callbackList.Count == 0)
                    {
                        Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_add_PropertyChanged(_pNative, Windows_Data_PropertyChangedEventArgs_Delegate_Handler, true);
                        _Windows_Data_PropertyChangedEventArgs_Delegate_Handle.Free();
                    }
                }
            }
        }


        // Public Methods
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_AddGesture(RootSystem.IntPtr pNative, RootSystem.IntPtr gesture);
        public void AddGesture(Microsoft.Kinect.VisualGestureBuilder.Gesture gesture)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
            }

            Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_AddGesture(_pNative, Helper.NativeWrapper.GetNativePtr(gesture));
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_AddGestures(RootSystem.IntPtr pNative, RootSystem.IntPtr gestures, int gesturesSize);
        public void AddGestures(Microsoft.Kinect.VisualGestureBuilder.Gesture[] gestures)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
            }

            int _gestures_idx=0;
            var _gestures_array = new RootSystem.IntPtr[gestures.Count()];
            foreach(var value in gestures)
            {
                _gestures_array[_gestures_idx] = Helper.NativeWrapper.GetNativePtr(value);
                _gestures_idx++;
            }
            var gesturesSmartGCHandle = new Helper.SmartGCHandle(RootSystem.Runtime.InteropServices.GCHandle.Alloc(_gestures_array, RootSystem.Runtime.InteropServices.GCHandleType.Pinned));
            var _gestures = gesturesSmartGCHandle.AddrOfPinnedObject();
            Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_AddGestures(_pNative, _gestures, gestures.Length);
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_RemoveGesture(RootSystem.IntPtr pNative, RootSystem.IntPtr gesture);
        public void RemoveGesture(Microsoft.Kinect.VisualGestureBuilder.Gesture gesture)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
            }

            Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_RemoveGesture(_pNative, Helper.NativeWrapper.GetNativePtr(gesture));
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_SetIsEnabled(RootSystem.IntPtr pNative, RootSystem.IntPtr gesture, bool isEnabled);
        public void SetIsEnabled(Microsoft.Kinect.VisualGestureBuilder.Gesture gesture, bool isEnabled)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
            }

            Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_SetIsEnabled(_pNative, Helper.NativeWrapper.GetNativePtr(gesture), isEnabled);
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern RootSystem.IntPtr Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_OpenReader(RootSystem.IntPtr pNative);
        public Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameReader OpenReader()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("VisualGestureBuilderFrameSource");
            }

            RootSystem.IntPtr objectPointer = Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_OpenReader(_pNative);
            Helper.ExceptionHelper.CheckLastError();
            if (objectPointer == RootSystem.IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameReader>(objectPointer, n => new Microsoft.Kinect.VisualGestureBuilder.VisualGestureBuilderFrameReader(n));
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_Dispose(RootSystem.IntPtr pNative);
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
            {
                Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_callbacks.TryAddDefault(_pNative);
                var callbackList = Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_callbacks[_pNative];
                lock (callbackList)
                {
                    if (callbackList.Count > 0)
                    {
                        callbackList.Clear();
                        if (_pNative != RootSystem.IntPtr.Zero)
                        {
                            Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_add_TrackingIdLost(_pNative, Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_Handler, true);
                        }
                        _Microsoft_Kinect_VisualGestureBuilder_TrackingIdLostEventArgs_Delegate_Handle.Free();
                    }
                }
            }
            {
                Windows_Data_PropertyChangedEventArgs_Delegate_callbacks.TryAddDefault(_pNative);
                var callbackList = Windows_Data_PropertyChangedEventArgs_Delegate_callbacks[_pNative];
                lock (callbackList)
                {
                    if (callbackList.Count > 0)
                    {
                        callbackList.Clear();
                        if (_pNative != RootSystem.IntPtr.Zero)
                        {
                            Microsoft_Kinect_VisualGestureBuilder_VisualGestureBuilderFrameSource_add_PropertyChanged(_pNative, Windows_Data_PropertyChangedEventArgs_Delegate_Handler, true);
                        }
                        _Windows_Data_PropertyChangedEventArgs_Delegate_Handle.Free();
                    }
                }
            }
        }
    }

}
