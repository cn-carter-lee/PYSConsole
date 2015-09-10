using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Text;

namespace XO.Registry.Utility
{
    public class ConfigurableFactory
    {
        public static ConcurrentDictionary<string, ConfigurableObject> configurableObjectCache = new ConcurrentDictionary<string, ConfigurableObject>();

        public static T CreateFromSection<T>(string sectionName = null) where T : ConfigurableObject
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                sectionName = typeof(T).Name;
            }
            ConfigurableObject configurableObject = null;
            if (configurableObjectCache.TryGetValue(sectionName, out configurableObject) == false)
            {
                IDictionary config = ConfigurationManager.GetSection(sectionName) as IDictionary;
                if (config == null) throw new KeyNotFoundException();
                configurableObject = Activator.CreateInstance(typeof(T), new object[] { config }) as ConfigurableObject;
                configurableObjectCache[sectionName] = configurableObject;
            }
            return configurableObject as T;
        }

        public static ConfigurableObject CreateFromSection(string sectionName)
        {
            return CreateFromSection<ConfigurableObject>(sectionName);
        }
    }
}
