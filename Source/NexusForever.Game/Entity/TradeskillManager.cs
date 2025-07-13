using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Crafting;
using NexusForever.GameTable;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Entity
{
    public class TradeskillManager : ITradeskillManager
    {
        private uint maxProfessions;
        private IPlayer player;
        private List<ITradeskill> activeTradeskills { get; }

        public ITradeskill Cooking { get;}
        public ITradeskill Farmer { get; }
        public ITradeskill Runecrafting { get; }
        public ITradeskill Fishing { get; }
        public uint MaxProfessions 
        { 
            get => maxProfessions; 
            set 
            { 
                maxProfessions = value;
            }
        }

        

        public bool CanLearnTradeskill(TradeskillType type)
        {
            switch (type)
            {
                case TradeskillType.Cooking:
                    return false;
                case TradeskillType.Farmer:
                    return false;
                case TradeskillType.Runecrafting:
                    return false;
                default:
                    break;
            }
            if (activeTradeskills.Count >= MaxProfessions)
                return false;
            foreach(var tradeskill in activeTradeskills)
            {
                if (tradeskill.Type == type)
                    return false;
            }
            return true;
        }

        public void Save(CharacterContext context)
        {
            //todo
        }

        public bool UnlearnTradeskill(uint index)
        {
            //todo
            return false;
        }

        public bool UnlearnTradeskill(TradeskillType type)
        {
            //todo
            return true;
        }

        public void LearnTradeskill(TradeskillType type)
        {
            var tradeskill = new Tradeskill(type);
            LearnTradeskill(tradeskill);
        }

        public void LearnTradeskill(ITradeskill tradeskill)
        {
            activeTradeskills.Add(tradeskill);
        }

        public ImmutableList<ITradeskill> GetActiveTradeskills()
        {
            return activeTradeskills.ToImmutableList();
        }

        public ServerProfessionsLoad BuildLoadMessage()
        {
            var message = new ServerProfessionsLoad()
            {
                RelearnCooldown = 300
            };

            var tradeskills = new List<TradeskillInfo>();
            foreach(var tradeskill in activeTradeskills)
            {
                var info = new TradeskillInfo()
                {
                    IsActive = 1,
                    TradeskillId = tradeskill.Type,
                    PropertyProficiencyFlags = tradeskill.PropertyProficiencyFlags,
                    TalentPoints = tradeskill.TalentPoints,
                    TradeskillXp = tradeskill.TradeskillXp,
                    TradeskillTalentTierIds = tradeskill.TradeskillTalentTierIds
                };
                tradeskills.Add(info);
            }
            foreach(TradeskillType t in (TradeskillType[])Enum.GetValues(typeof(TradeskillType)))
            {
                var info = new TradeskillInfo()
                {
                    IsActive = 0,
                    TradeskillId = t,
                    PropertyProficiencyFlags = 0,
                    TalentPoints = 0,
                    TradeskillXp = 0
                };
                if (t == TradeskillType.Cooking || t == TradeskillType.Weaponsmith)
                    info.IsActive = 1;
                tradeskills.Add(info);
            }
            foreach (var tradeskill in (new ITradeskill[] { Cooking /*Others*/}))
            {
                var info = new TradeskillInfo()
                {
                    IsActive = 1,
                    TradeskillId = tradeskill.Type,
                    PropertyProficiencyFlags = 0,
                    TalentPoints = 0,
                    TradeskillXp = 0,
                    TradeskillTalentTierIds = [65]
                };
                tradeskills.Add(info);
            }
            
            message.LearnedSchematics = [148,149,151];//test
            message.DiscoveredSchematics.Add(new ServerProfessionsLoad.DiscoveredSchematic()
            {
                TradeskillSchematic2Id = 148,
                Coordinates = new System.Numerics.Vector2(0,0)
            });
            message.DiscoveredSchematics.Add(new ServerProfessionsLoad.DiscoveredSchematic()
            {
                TradeskillSchematic2Id = 149,
                Coordinates = new System.Numerics.Vector2(0, 0)
            });
            message.DiscoveredSchematics.Add(new ServerProfessionsLoad.DiscoveredSchematic()
            {
                TradeskillSchematic2Id = 150,
                Coordinates = new System.Numerics.Vector2(0, 0)
            });
            //message.UnknownArray = [3303];//test

            //TODO: Handle schematics and discoveries. Handle hobbies

            message.Tradeskills = tradeskills;

            return message;
        }
        public void SendInitialPackets()
        {
            //var msg = BuildLoadMessage();
            //player.Session.EnqueueMessage(msg);
        }

        public TradeskillManager(IPlayer player, CharacterModel model)
        {
            this.player = player;
            activeTradeskills = new List<ITradeskill>(2);

            //PH - load from the db
            Cooking = new Tradeskill(TradeskillType.Cooking);
            Farmer = new Tradeskill(TradeskillType.Farmer);
            Runecrafting = new Tradeskill(TradeskillType.Runecrafting);
            Fishing = new Tradeskill(TradeskillType.Fishing);
        }
    }
}
