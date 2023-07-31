using System;
using Core.Configurations;
using Core.Services;

namespace Core.EventSystem
{
    public class UserEventPublisher : IUserEventPublisher
    {
        public UserEventPublisher(IConfigProvider configProvider)
        {
            
        }
        
        public IDisposable Subscribe(IObserver<object> observer)
        {
            throw new NotImplementedException();
        }
    }
    
    
}