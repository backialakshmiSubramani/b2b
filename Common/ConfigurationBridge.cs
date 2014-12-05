// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationBridge.cs" company="">
//   
// </copyright>
// <summary>
//   Use ConfigurationBridge to retrieve configuration value from config file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;

namespace Modules.Channel.B2B.Common
{
    /// <summary>
    /// Use ConfigurationBridge to retrieve configuration value from config file.
    /// </summary>
    public class ConfigurationBridge
    {
        /// <summary>
        /// The configuration object.
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configFileName">Configuration File Name</param>
        public ConfigurationBridge(string configFileName)
        {
            MakeConfigurationAvailable(configFileName);
        }

        /// <summary>
        /// Use GetSetting to get the value based on the key from Config file
        /// </summary>
        /// <param name="configuredKey">The key to search with</param>
        /// <returns>The value based on the key</returns>
        public string GetSetting(string configuredKey)
        {
            try
            {
                var configuredValue = this.configuration.AppSettings.Settings[configuredKey].Value;
                return configuredValue;
            }
            catch
            {
                throw new ConfigurationErrorsException(string.Format(
                    "Error retrieving configuration value for: '{0}' within config file: {1}.", configuredKey, this.configuration.FilePath));
            }
        }

        /// <summary>
        /// Instantiates the Configuration object. Also sets the path of the config file
        /// </summary>
        /// <param name="configFileName">Configuration File Name</param>
        private void MakeConfigurationAvailable(string configFileName)
        {
            try
            {
                this.configuration = ConfigurationManager.OpenMappedExeConfiguration(
                    new ExeConfigurationFileMap { ExeConfigFilename = configFileName }, ConfigurationUserLevel.None);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException("Cannot locate application configuration file.", ex);
            }
        }
    }
}
