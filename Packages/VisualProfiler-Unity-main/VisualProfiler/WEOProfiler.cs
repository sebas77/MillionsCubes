using UnityEngine;

//this seemed to work
//class GPUProfilerRecorder
//{
//    ProfilerRecorder setPassCallsRecorder;
//    
//    public GPUProfilerRecorder()
//    {
//        setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Gfx.WaitForPresentOnGfxThread", 30);
//    }
//        
//    public float CalculateAverageFromSampleValuesPerFrame()
//    {
//        return setPassCallsRecorder.LastValue / 1000000.0f;
//    }
//    
//    ~GPUProfilerRecorder()
//    {
//        if (setPassCallsRecorder.Valid)
//            setPassCallsRecorder.Dispose();
//    }
//}
//strangely couldn't make this work    
//class CPUProfilerRecorder
//{
//    ProfilerRecorder setPassCallsRecorder;
//    
//    public CPUProfilerRecorder()
//    {
//        setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Scripts", 30);
//    }
//        
//    public float CalculateAverageFromSampleValuesPerFrame()
//    {
//        return setPassCallsRecorder.CurrentValue / 1000000.0f;
//    }
//    
//    ~CPUProfilerRecorder()
//    {
//        if (setPassCallsRecorder.Valid)
//            setPassCallsRecorder.Dispose();
//    }
//}
//https://twitter.com/sebify/status/1615758928700112918

public static class WEOProfiler
{
    public static ProfilerWatch ProfilerWatch;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        ProfilerWatch = new ProfilerWatch();
    }
}