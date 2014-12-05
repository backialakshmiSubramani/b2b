using System.Configuration;

namespace Modules.Channel.B2B.Common
{
    /// <summary>
    /// Use ConfigurationReader to read the values in the app config file.
    /// </summary>
    public class ConfigurationReader
    {
        #region Public Static Methods

        /// <summary>
        /// Use GetValue to get the value based on the key in app config file
        /// </summary>
        /// <param name="configKey">Key to search by</param>
        /// <returns>Returns the value from the key value pair</returns>
        public static string GetValue(string configKey)
        {
            return !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[configKey]) ? ConfigurationManager.AppSettings[configKey] : string.Empty;
        }

        /// <summary>
        /// Checks if the key is available in the config file
        /// </summary>
        /// <param name="configKey">Key to search by</param>
        /// <returns>Returns true if the value for the key passed is available</returns>
        public static bool CheckKey(string configKey)
        {
            return !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[configKey]);
        }

        #endregion
    }
}
