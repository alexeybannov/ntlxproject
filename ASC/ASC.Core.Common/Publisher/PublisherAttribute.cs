#region usings

using System;
using ASC.Reflection;

#endregion

namespace ASC.Core.Common.Publisher
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class PublisherAttribute : Attribute
    {
        public PublisherAttribute(Type type)
        {
            PublisherType = type;
        }

        public Type PublisherType { get; private set; }

        public IPublisher CreatePublisher()
        {
            return (IPublisher) TypeInstance.Create(PublisherType);
        }
    }
}