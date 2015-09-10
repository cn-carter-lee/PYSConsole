using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XO.Registry.Utility
{
    public class SdkConfig : ConfigurableObject
    {
        public SdkConfig(IDictionary config)
            : base(config)
        { }

        public string ApiUrl
        {
            get
            {
                return this.Config["ApiUrl"] as string;
            }
        }

        public EnvironmentType Environment
        {
            get
            {
                string environgment = this.Config["Environment"] as string;
                EnvironmentType value;
                if (Enum.TryParse(environgment, out value) == true)
                {
                    return value;
                }

                throw new KeyNotFoundException("Can't find Environment key in config file.");
            }
        }

        public enum EnvironmentType
        {
            Test,
            Prodcution
        }
    }
}
