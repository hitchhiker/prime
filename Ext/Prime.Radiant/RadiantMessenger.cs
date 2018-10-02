using System;
using System.Collections.Generic;
using System.Text;
using Prime.Base;
using Prime.Base.Messaging.Common;
using Prime.Core;

namespace Prime.Radiant
{
    public class RadiantMessenger : CommonBase
    {
        public RadiantMessenger(PrimeInstance instance) : base(instance)
        {
            instance.M.RegisterAsync<PrimePublishRequest>(this, m =>
            {
                RadiantEntry.Publish(instance, m.PublisherConfigPath);
                instance.M.Send(new PrimePublishResponse(m) { Success = true });
            });

            instance.M.RegisterAsync<PrimeUpdateRequest>(this, m =>
            {
                RadiantEntry.UpdateAll(instance);
                instance.M.Send(new PrimeUpdateResponse(m) { Success = true });
            });
        }
    }
}
