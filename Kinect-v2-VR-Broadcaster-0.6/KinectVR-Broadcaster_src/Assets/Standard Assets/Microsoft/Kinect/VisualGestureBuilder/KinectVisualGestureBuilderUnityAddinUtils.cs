using RootSystem = System;
using System.Linq;
using System.Collections.Generic;
namespace Microsoft.Kinect.VisualGestureBuilder
{
    //
    // Microsoft.Kinect.VisualGestureBuilder.KinectVisualGestureBuilderUnityAddinUtils
    //
    public sealed partial class KinectVisualGestureBuilderUnityAddinUtils
    {
        [RootSystem.Runtime.InteropServices.DllImport("KinectVisualGestureBuilderUnityAddin", CallingConvention=RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError=true)]
        private static extern void KinectVisualGestureBuilderUnityAddin_FreeMemory(RootSystem.IntPtr pToDealloc);
        public static void FreeMemory(RootSystem.IntPtr pToDealloc)
        {
            KinectVisualGestureBuilderUnityAddin_FreeMemory(pToDealloc);
        }
    }

}
