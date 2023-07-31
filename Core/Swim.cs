using System.Collections.Generic;
using System.Linq;
using Core.Configurations;
using Core.Modules;
using Domain;
using Optional;
using Optional.Collections;

namespace Core
{
    public class Swim
    {
        private readonly List<ISwimModule> _modules;

        public Swim(DesktopService desktopService, SwimConfig swimConfig)
        {
            _modules = new List<ISwimModule>();
        }

        public Option<TModule> GetModule<TModule>()
        {
            return _modules
                .OfType<TModule>()
                .FirstOrNone();
        }
    }
}