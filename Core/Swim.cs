using System;
using Core.Configurations;
using Core.EventSystem;
using Core.Modules.WorkspaceModule;
using Core.Services;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Optional;

namespace Core;

public class Swim
{

    public Swim()
    {
    }

    public static void RegisterServices(IServiceCollection services)
    {
        services
            .AddSingleton<Swim>()
            .AddSingleton<IDesktopService, DesktopService>()
            .AddSingleton<IConfigProvider, ConfigLoader>()
            .AddSingleton<IConfigLocator, ConfigLocator>()
            .AddSingleton<IUserEventPublisher, UserEventPublisher>()
                
            //modules
            .AddSingleton<WorkSpaceManager>();
    }

    public Option<TModule> GetModule<TModule>()
    {
        throw new Exception();
    }
}