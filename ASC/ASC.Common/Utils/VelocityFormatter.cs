using System.Collections.Generic;
using System.IO;
using System.Text;
using ASC.Common.Collections;
using Commons.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime.Resource.Loader;

namespace ASC.Common.Utils
{
    public class TextLoader : ResourceLoader
    {
        public override void Init(ExtendedProperties configuration)
        {
            //nothing to configure
        }

        public override Stream GetResourceStream(string source)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(source));
        }

        public override long GetLastModified(NVelocity.Runtime.Resource.Resource resource)
        {
            return 1;
        }

        public override bool IsSourceModified(NVelocity.Runtime.Resource.Resource resource)
        {
            return false;
        }
    }

    public class VelocityFormatter
    {
        private static bool _isVelocityInitialized;
        private static readonly CachedDictionary<Template> Patterns = new CachedDictionary<Template>("velocity_patterns");

        public static string FormatText(string templateText, IDictionary<string,object> values)
        {
            var nvelocityContext = new VelocityContext();
            foreach (var tagValue in values)
                nvelocityContext.Put(tagValue.Key, tagValue.Value);
            return FormatText(templateText, nvelocityContext);
        }

        public static string FormatText(string templateText, VelocityContext context)
        {
            if (!_isVelocityInitialized)
            {
                var properties = new ExtendedProperties();
                properties.AddProperty("resource.loader", "custom");
                properties.AddProperty("custom.resource.loader.class", "ASC.Common.Utils.TextLoader; ASC.Common");
                properties.AddProperty("input.encoding", Encoding.UTF8.WebName);
                properties.AddProperty("output.encoding", Encoding.UTF8.WebName);
                Velocity.Init(properties);
                _isVelocityInitialized = true;
            }
            using (var writer = new StringWriter())
            {
                var template = Patterns.Get(templateText.GetHashCode().ToString(), () => Velocity.GetTemplate(templateText));
                template.Merge(context, writer);
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}