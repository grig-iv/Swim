using System;

namespace Core.Configurations;

public interface IConfigProvider
{
    IObservable<SwimConfig> WhenConfigChanged { get; }
    SwimConfig Config { get; }
}