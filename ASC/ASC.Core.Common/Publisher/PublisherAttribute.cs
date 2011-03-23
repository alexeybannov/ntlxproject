using System;

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
            return (IPublisher)Activator.CreateInstance(PublisherType);
        }
    }
}