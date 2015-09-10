using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace XO.Registry.Utility
{
    public class ConfigurableObjectFactory
    {
        public static IDictionary<string, ConfigurableObject> configurableObjectCache = new Dictionary<string, ConfigurableObject>();

        public static T CreateFromSection<T>(string sectionName = null) where T : ConfigurableObject
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                sectionName = typeof(T).Name;
            }
            ConfigurableObject configurableObject = null;
            if (configurableObjectCache.ContainsKey(sectionName))
            {
                configurableObject = configurableObjectCache[sectionName];
            }
            else
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
