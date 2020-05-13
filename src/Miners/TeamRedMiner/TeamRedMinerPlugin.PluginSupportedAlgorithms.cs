﻿using NHM.MinerPluginToolkitV1.Configs;
using NHM.Common.Enums;
using System.Collections.Generic;

using SAS = NHM.MinerPluginToolkitV1.Configs.PluginSupportedAlgorithmsSettings.SupportedAlgorithmSettings;

namespace TeamRedMiner
{
    public partial class TeamRedMinerPlugin
    {
        protected override PluginSupportedAlgorithmsSettings DefaultPluginSupportedAlgorithmsSettings => new PluginSupportedAlgorithmsSettings
        {
            DefaultFee = 2.5,
            AlgorithmFees = new Dictionary<AlgorithmType, double>
            {
                { AlgorithmType.DaggerHashimoto, 1.0 }

            },
            Algorithms = new Dictionary<DeviceType, List<SAS>>
            {
                {
                    DeviceType.AMD,
                    new List<SAS>
                    {
                        new SAS(AlgorithmType.Lyra2REv3),
                        new SAS(AlgorithmType.X16R) { Enabled=false },
                        new SAS(AlgorithmType.GrinCuckatoo31),
                        new SAS(AlgorithmType.GrinCuckarood29),
                        new SAS(AlgorithmType.X16Rv2),
                        new SAS(AlgorithmType.DaggerHashimoto)
                    }
                }
            },
            AlgorithmNames = new Dictionary<AlgorithmType, string>
            {
                { AlgorithmType.Lyra2REv3, "lyra2rev3" },
                { AlgorithmType.X16R, "x16r" },
                { AlgorithmType.GrinCuckatoo31, "cuckatoo31_grin" },
                { AlgorithmType.GrinCuckarood29, "cuckarood29_grin" },
                { AlgorithmType.X16Rv2, "x16rv2" },
                { AlgorithmType.DaggerHashimoto, "ethash" }
            }
        };
    }
}
