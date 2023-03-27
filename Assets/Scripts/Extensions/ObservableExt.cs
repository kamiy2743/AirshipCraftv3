using System;
using UniRx;
using UnityEngine;

namespace ACv3.Extensions
{
    public static class ObservableExt
    {
        public static IObservable<WinType> SmartAny(IObservable<Unit> left, IObservable<Unit> right)
        {
            return Observable.Merge(
                    left.Select(_ => (index: 0, Time.frameCount)),
                    right.Select(_ => (index: 1, Time.frameCount)))
                .BatchFrame()
                .Select(frames =>
                {
                    var leftLatestFrame = 0;
                    var rightLatestFrame = 0;
                    
                    foreach (var frame in frames)
                    {
                        if (frame.index == 0)
                        {
                            leftLatestFrame = frame.frameCount;
                        }
                        else
                        {
                            rightLatestFrame = frame.frameCount;
                        }
                    }

                    if (leftLatestFrame > rightLatestFrame) return WinType.Left;
                    if (rightLatestFrame > leftLatestFrame) return WinType.Right;
                    return WinType.Same;
                });
        }
        
        public enum WinType
        {
            Left,
            Right,
            Same
        }

        public static IObservable<T> AsReplayObservable<T>(this IObservable<T> source)
        {
            var connectable = source.Replay();
            connectable.Connect();
            return connectable;
        }
    }
}