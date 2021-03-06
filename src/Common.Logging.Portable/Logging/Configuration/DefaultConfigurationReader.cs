#region License

/*
 * Copyright � 2002-2009 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
#if NETSTANDARD20
using Microsoft.Extensions.Configuration;
#endif

namespace Common.Logging.Configuration
{
    /// <summary>
    /// Implementation of <see cref="IConfigurationReader"/> that uses the standard .NET 
    /// configuration APIs, ConfigurationSettings in 1.x and ConfigurationManager in 2.0
    /// </summary>
    /// <author>Mark Pollack</author>
    public class DefaultConfigurationReader : IConfigurationReader
    {
#if NETSTANDARD20
        private readonly string _jsonSection;
#endif

        /// <summary>
        /// configure a default configuration reader
        /// </summary>
        public DefaultConfigurationReader(string jsonSectionName)
        {
#if NETSTANDARD20
            _jsonSection = jsonSectionName;
#endif
        }

        /// <summary>
        /// Parses the configuration section and returns the resulting object.
        /// Using the <c>System.Configuration.ConfigurationManager</c>
        /// </summary>
        /// <param name="sectionName">Name of the configuration section.</param>
        /// <returns>
        /// Object created by a corresponding <c>IConfigurationSectionHandler"</c>
        /// </returns>
        /// <remarks>
        /// 	<p>
        /// Primary purpose of this method is to allow us to parse and
        /// load configuration sections using the same API regardless
        /// of the .NET framework version.
        /// </p>
        /// </remarks>
        public object GetSection(string sectionName)
        {
#if NETSTANDARD20 
            // No System.Configuration in .net core/standard, try appsettings.json
            var obj = TryAppSettingsJson(_jsonSection);
            if (obj != null)
                return obj;
#endif

            // try app.config
            return TrySystemDotConfig(sectionName);
        }

#if NETSTANDARD20    

        /// <summary>
        /// Probe for appsettings.json
        /// </summary>
        /// <returns>a tuple of the paths searched and the one where it was found (or null)</returns>
        public static (List<string> searched, string found) ProbeForAppSettings()
        {
            var searched = new List<string>();
            foreach (var location in AppSettingsPaths())
            {
                searched.Add(location);
                if (Directory.GetFiles(location, "appsettings.json").Length == 1)
                    return (searched, Path.Combine(location, "appsettings.json"));
            }

            foreach (var location in AppSettingsPaths())
            {
                var dir = Directory.GetParent(location);
                while (dir != null)
                {
                    searched.Add(dir.FullName);
                    if (Directory.GetFiles(dir.FullName, "appsettings.json").Length == 1)
                        return (searched, Path.Combine(dir.FullName, "appsettings.json"));
                    dir = dir.Parent;
                }
            }

            return (searched, null);
        }

        private static object TryAppSettingsJson(string sectionName)
        {
            foreach (var location in AppSettingsPaths())
            {
                var ret = TryLoad(location, sectionName);
                if (ret != null)
                    return ret;
            }

            foreach (var location in AppSettingsPaths())
            {
                var dir = Directory.GetParent(location);
                while (dir != null)
                {
                    if (dir.GetFiles("appsettings.json").Length == 1)
                    {
                        var ret = TryLoad(dir.FullName, sectionName);
                        if (ret != null)
                            return ret;
                    }
                    dir = dir.Parent;
                }
            }

            Debug.WriteLine($"no appsettings.json found");
            return null;
        }

        private static object TryLoad(string location, string sectionName)
        {
            try
            {
                var config = new ConfigurationBuilder()
                        .SetBasePath(location)
                        .AddJsonFile("appsettings.json", optional: true)
                        .Build();

                var logConfig = new LogConfiguration();
                config.GetSection(sectionName).Bind(logConfig);

                if (logConfig.FactoryAdapter != null)
                    return new LogConfigurationReader(logConfig).GetSection(null);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"exception {e.GetType().FullName} during load of {location}/appsettings.json {e.Message}");
            }

            return null;
        }

        private static IEnumerable<string> AppSettingsPaths()
        {
            string basePath = tryGetPath(() => AppContext.BaseDirectory, "base directory");
            if (basePath != null)
                yield return basePath;

            string entryAssembly = tryGetPath(() => Assembly.GetEntryAssembly().Location, "entry assembly");
            if (entryAssembly != null)
                yield return entryAssembly;

            string execAssembly = tryGetPath(() => Assembly.GetExecutingAssembly().Location, "exec assembly");
            if (execAssembly != null)
                yield return execAssembly;

            string mainModule = tryGetPath(() => Process.GetCurrentProcess().MainModule.FileName, "main exe");
            if (mainModule != null)
                yield return mainModule;

            yield return Directory.GetCurrentDirectory();

            string tryGetPath(Func<string> f, string desc)
            {
                string loc = null;
                try
                {
                    loc = f();
                }
                catch (Exception e)
                {
                    // getting assembly locations can fail sometimes
                    Debug.WriteLine($"exception {e.GetType().FullName} during load of {desc} {e.Message}");
                    return null;
                }
                return Path.GetDirectoryName(loc);
            }
        }
#endif

        /// <summary>
        /// this will attempt to load config from System.Configuration and app/web.config
        /// for full framework usage (which if .net 472 can use netstandard 2.0)
        /// for .net core, this won't find the class and will return null
        /// </summary>
        private static object TrySystemDotConfig(string sectionName)
        {
#if PORTABLE
            // IConfigurationReader in (platform specific) Common.Logging dll and use that
            const string configManager40 = "System.Configuration.ConfigurationManager, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            var configurationManager = Type.GetType(configManager40);
            if(configurationManager == null)
            {
                // Silverlight or .net core, and maybe if System.Configuration is not loaded?
                return null;
            }
    #if !WinRT
            var getSection = configurationManager.GetMethod("GetSection", new[] { typeof(string) });
    #else
            var getSection = configurationManager.GetMethod("GetSection");
    #endif
            if (getSection == null)
                throw new PlatformNotSupportedException("Could not find System.Configuration.ConfigurationManager.GetSection method");

            return getSection.Invoke(null, new[] {sectionName});
#else
            return System.Configuration.ConfigurationManager.GetSection(sectionName);
#endif
        }
    }
}