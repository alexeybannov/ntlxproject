#region usings

using System;
using System.Collections;

#endregion

namespace ASC.Core.Common.Publisher
{
    public interface IPublisherHost
    {
        Version Version { get; }

        Hashtable Properties { get; }

        void Initialize(IPublisher defaultPublisher, Hashtable properties);

        IPublisher GetPublisher();
    }
}