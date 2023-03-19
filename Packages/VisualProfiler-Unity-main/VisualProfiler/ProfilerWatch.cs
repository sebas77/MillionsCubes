using System.Collections.Generic;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

public class ProfilerWatch
{
    public ProfilerWatch()
    {
        SetPlayerLoop();
    }

    public float lastGPU => (float)_frameTimings[_currentFrame % 30].gpuFrameTime;
    public float lastCPU => (float)_frameTimings[_currentFrame % 30].cpuFrameTime;

    void SetPlayerLoop()
    {
        var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

        _subsystems = playerLoop.subSystemList;

        for (int i = 0; i < _subsystems.Length; i++)
        {
            ref var subSystem = ref _subsystems[i];

            if (subSystem.type == typeof(Initialization))
            {
                subSystem.updateDelegate += StopTimer;
            }
            else if (subSystem.type == typeof(PostLateUpdate))
            {
                List<PlayerLoopSystem> list =
                        new List<PlayerLoopSystem>(subSystem.subSystemList.Length + 1);

                for (int iindex = 0; iindex < subSystem.subSystemList.Length; iindex++)
                {
                    //all this crap because we don't want the WaitForPresent to be part
                    //of the CPU time, but must be part of the GPU time
                    if (subSystem.subSystemList[iindex].type == typeof(PostLateUpdate.FinishFrameRendering))
                    {
                        list.Add(
                            new PlayerLoopSystem()
                            {
                                    updateDelegate = StartTimer,
                                    type = typeof(ProfilerWatch)
                            });
                    }

                    list.Add(subSystem.subSystemList[iindex]);
                }

                subSystem.subSystemList = list.ToArray();
            }
        }

        PlayerLoop.SetPlayerLoop(playerLoop);
    }

    void StartTimer()
    {
        _frameTimings[_currentFrame % 30].gpuFrameTime = _gpustopwatch.ElapsedMilliseconds;
        _frameTimings[_currentFrame % 30].cpuFrameTime = _cpustopwatch.ElapsedMilliseconds;
        _currentFrame++;

        _gpustopwatch.Restart();
        _cpustopwatch.Restart();
    }

    void StopTimer()
    {
        _gpustopwatch.Stop();
    }

    public void AverageFrameTiming(out double cpuFrameTime, out double gpuFrameTime)
    {
        double cpuTime = 0.0f;
        double gpuTime = 0.0f;

        for (int i = 0; i < frameTimingsCount; ++i)
        {
            cpuTime += _frameTimings[i].cpuFrameTime;
            gpuTime += _frameTimings[i].gpuFrameTime;
        }

        cpuTime /= frameTimingsCount;
        gpuTime /= frameTimingsCount;

        cpuFrameTime = (float)(cpuTime);
        gpuFrameTime = (float)(gpuTime);
    }

    readonly System.Diagnostics.Stopwatch _gpustopwatch = new System.Diagnostics.Stopwatch();
    readonly System.Diagnostics.Stopwatch _cpustopwatch = new System.Diagnostics.Stopwatch();
    PlayerLoopSystem[] _subsystems;
    readonly CustomFrameTimings[] _frameTimings = new CustomFrameTimings[frameTimingsCount];
    uint _currentFrame;
    static readonly int frameTimingsCount = 30;

    struct CustomFrameTimings
    {
        public double cpuFrameTime;
        public double gpuFrameTime;
    }
}