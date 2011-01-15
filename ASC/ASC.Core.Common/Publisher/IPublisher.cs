#region usings

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace ASC.Core.Common.Publisher
{
    public interface IPublisher
    {
        Version Version { get; }

        Version PublisherVersion { get; }

        Hashtable Properties { get; }

        void Initialize(Hashtable properties);

        List<Article> HandleRequest(RequestContext context, List<Zone> visibleZones);
    }
}