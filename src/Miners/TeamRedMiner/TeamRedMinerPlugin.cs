﻿using MinerPlugin;
using NiceHashMinerLegacy.Common.Algorithm;
using NiceHashMinerLegacy.Common.Device;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using MinerPluginToolkitV1.Interfaces;
using System.IO;
using NiceHashMinerLegacy.Common;
using MinerPluginToolkitV1.ExtraLaunchParameters;
using MinerPluginToolkitV1.Configs;
using MinerPluginToolkitV1;

namespace TeamRedMiner
{
    public class TeamRedMinerPlugin : IMinerPlugin, IInitInternals, IBinaryPackageMissingFilesChecker
    {
        public TeamRedMinerPlugin(string pluginUUID = "189aaf80-4b23-11e9-a481-e144ccd86993")
        {
            _pluginUUID = pluginUUID;
        }
        private readonly string _pluginUUID;
        public string PluginUUID => _pluginUUID;

        public Version Version => new Version(1, 1);

        public string Name => "TeamRedMiner";

        public string Author => "stanko@nicehash.com";

        public bool CanGroup(MiningPair a, MiningPair b)
        {
            return a.Algorithm.FirstAlgorithmType == b.Algorithm.FirstAlgorithmType;
        }

        public IMiner CreateMiner()
        {
            return new TeamRedMiner(PluginUUID, AMDDevice.OpenCLPlatformID)
            {
                MinerOptionsPackage = _minerOptionsPackage,
                MinerSystemEnvironmentVariables = _minerSystemEnvironmentVariables
            };
        }

        public Dictionary<BaseDevice, IReadOnlyList<Algorithm>> GetSupportedAlgorithms(IEnumerable<BaseDevice> devices)
        {
            var supported = new Dictionary<BaseDevice, IReadOnlyList<Algorithm>>();
            // Get AMD GCN4+
            var amdGpus = devices.Where(dev => dev is AMDDevice gpu && Checkers.IsGcn4(gpu)).Cast<AMDDevice>();

            foreach (var gpu in amdGpus)
            {
                var algorithms = GetSupportedAlgorithms(gpu);
                if (algorithms.Count > 0) supported.Add(gpu, algorithms);
            }

            return supported;
        }

        IReadOnlyList<Algorithm> GetSupportedAlgorithms(AMDDevice gpu)
        {
            var algorithms = new List<Algorithm> {
                new Algorithm(PluginUUID, AlgorithmType.CryptoNightV8),
                new Algorithm(PluginUUID, AlgorithmType.CryptoNightR),
                new Algorithm(PluginUUID, AlgorithmType.Lyra2REv3),
                //new Algorithm(PluginUUID, AlgorithmType.Lyra2Z),
            };
            return algorithms;
        }

        #region Internal Settings
        public void InitInternals()
        {
            var pluginRoot = Path.Combine(Paths.MinerPluginsPath(), PluginUUID);

            var fileMinerOptionsPackage = InternalConfigs.InitInternalsHelper(pluginRoot, _minerOptionsPackage);
            if (fileMinerOptionsPackage != null) _minerOptionsPackage = fileMinerOptionsPackage;

            var readFromFileEnvSysVars = InternalConfigs.InitMinerSystemEnvironmentVariablesSettings(pluginRoot, _minerSystemEnvironmentVariables);
            if (readFromFileEnvSysVars != null) _minerSystemEnvironmentVariables = readFromFileEnvSysVars;
        }

        protected static MinerSystemEnvironmentVariables _minerSystemEnvironmentVariables = new MinerSystemEnvironmentVariables
        {
            DefaultSystemEnvironmentVariables = new Dictionary<string, string>()
            {
                {"GPU_MAX_ALLOC_PERCENT", "100"},
                {"GPU_USE_SYNC_OBJECTS", "1"},
                {"GPU_SINGLE_ALLOC_PERCENT", "100"},
                {"GPU_MAX_HEAP_SIZE", "100"},
            },
        };

        protected static MinerOptionsPackage _minerOptionsPackage = new MinerOptionsPackage{
            GeneralOptions = new List<MinerOption>
            {
                /// <summary>
                ///  Specified the init style (1 is default):
                ///  1: One gpu at the time, complete all before mining.
                ///  2: Three gpus at the time, complete all before mining.
                ///  3: All gpus in parallel, start mining immediately.
                /// </summary>
                new MinerOption
                {
                    Type = MinerOptionType.OptionWithSingleParameter,
                    ID = "teamRedMiner_initStyle",
                    ShortName = "init_style=",
                    DefaultValue = "1"
                }
            },
            TemperatureOptions = new List<MinerOption>
            {
                /// <summary>
                ///  Sets the temperature at which the miner will stop GPUs that are too hot.
                ///  Default is 85C.
                /// </summary>
                new MinerOption
                {
                    Type = MinerOptionType.OptionWithSingleParameter,
                    ID = "teamRedMiner_tempLimit",
                    ShortName = "temp_limit=",
                    DefaultValue = "85"
                },
                /// <summary>
                ///  Sets the temperature below which the miner will resume GPUs that were previously stopped due to temperature exceeding limit.
                ///  Default is 60C.
                /// </summary>
                new MinerOption
                {
                    Type = MinerOptionType.OptionWithSingleParameter,
                    ID = "teamRedMiner_tempResume",
                    ShortName = "temp_resume=",
                    DefaultValue = "60"
                }
            }
        };
        #endregion Internal Settings

        public IEnumerable<string> CheckBinaryPackageMissingFiles()
        {
            var miner = CreateMiner() as IBinAndCwdPathsGettter;
            if (miner == null) return Enumerable.Empty<string>();
            var pluginRootBinsPath = miner.GetBinAndCwdPaths().Item2;
            return BinaryPackageMissingFilesCheckerHelpers.ReturnMissingFiles(pluginRootBinsPath, new List<string> { "teamredminer.exe" });
        }
    }
}