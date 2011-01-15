namespace System.Collections.Generic
{
    public static class DictionaryStringStringExtention
    {
        public static bool PropertyExists(this IDictionary<string, string> dict, string prop)
        {
            return dict.ContainsKey(prop);
        }

        public static string PropValue(this IDictionary<string, string> dict, string prop)
        {
            string value = null;
            if (!dict.TryGetValue(prop, out value))
                value = null;
            return value;
        }

        public static string PropStringValue(this IDictionary<string, string> dict, string prop)
        {
            return PropValue(dict, prop);
        }

        public static Guid? PropGuidValue(this IDictionary<string, string> dict, string prop)
        {
            string value = PropValue(dict, prop);
            if (String.IsNullOrEmpty(value)) return null;
            else
            {
                try
                {
                    return new Guid(value);
                }
                catch
                {
                }
                return null;
            }
        }

        public static bool? PropBoolValue(this IDictionary<string, string> dict, string prop)
        {
            string value = PropValue(dict, prop);
            if (String.IsNullOrEmpty(value)) return null;
            else
            {
                if (value == "1" || value.ToLower() == "true") return true;
                else if (value == "0" || value.ToLower() == "false") return false;
                else return null;
            }
        }

        public static int? PropInt32Value(this IDictionary<string, string> dict, string prop)
        {
            string value = PropValue(dict, prop);
            if (String.IsNullOrEmpty(value)) return null;
            else
            {
                int intres = -1;
                if (Int32.TryParse(value, out intres))
                    return intres;
                else
                    return null;
            }
        }

        public static DateTime? PropDateTimeValue(this IDictionary<string, string> dict, string prop)
        {
            string value = PropValue(dict, prop);
            if (String.IsNullOrEmpty(value)) return null;
            else
            {
                DateTime dtres;
                if (DateTime.TryParse(value, out dtres))
                    return dtres;
                else
                    return null;
            }
        }

        public static void Save(this IDictionary<string, string> dict, string key, string value)
        {
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }
    }
}