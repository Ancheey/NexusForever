using NexusForever.Game.Static.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Static.Crafting
{
    public static class CraftingCircuitPropertyMap
    {
        //Based on CraftingLib.GetAvailableProperties() when a CurrentCraft is non-nil.
        public static readonly Dictionary<CraftingCircuitSocketType, List<Property>> Properties = new()
        {
            { CraftingCircuitSocketType.Air, new() 
                {
                    Property.RatingAvoidReduce,
                    Property.RatingAvoidIncrease
                }
            },
            { CraftingCircuitSocketType.Water, new()
                {
                    Property.RatingMultiHitChance,
                    Property.RatingGlanceChance
                }
            },
            { CraftingCircuitSocketType.Earth, new()
                {
                    Property.RatingCritSeverityIncrease
                }
            },
            { CraftingCircuitSocketType.Fire, new()
                {
                    Property.RatingCritChanceIncrease,
                }
            },
            { CraftingCircuitSocketType.Logic, new()
                {
                    Property.RatingCriticalMitigation,
                    Property.RatingIntensity,
                    Property.RatingVigor
                }
            },
            { CraftingCircuitSocketType.Life, new()
                {
                    Property.BaseFocusPool,
                }
            },
            { CraftingCircuitSocketType.Fusion, new()
                {
                    Property.RatingAvoidReduce,
                    Property.RatingAvoidIncrease,
                    Property.RatingMultiHitChance,
                    Property.RatingGlanceChance,
                    Property.RatingCritSeverityIncrease,
                    Property.RatingCritChanceIncrease,
                    Property.RatingCriticalMitigation,
                    Property.RatingIntensity,
                    Property.RatingVigor,
                    Property.BaseFocusPool
                }
            },
        };
    }
}
