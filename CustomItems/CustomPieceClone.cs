using Jotunn.Configs;
using System.Collections.Generic;

namespace Cozyheim.CustomItems {
    public class CustomPieceClone {
        public string vanillaPieceName = "";
        public string newPieceName = "";
        public PieceConfig pieceConfig;
    }

    public class EpicLootItems {
        public static List<CustomPieceClone> items = new List<CustomPieceClone>() {
            new CustomPieceClone() {
                vanillaPieceName = "piece_groundtorch_mist",
                newPieceName = "MagicWeaponEnchant",
                pieceConfig = new PieceConfig() {
                    Description = "Magic materials for enchanting a weapon",
                    Requirements = new[] {
                        new RequirementConfig("RunestoneMagic", 1, 0, true),
                        new RequirementConfig("ShardMagic", 5, 0, true),
                        new RequirementConfig("EssenceMagic", 5, 0, true),
                        new RequirementConfig("Coins", 25, 0, true)
                    }
                }
            },
            new CustomPieceClone() {
                vanillaPieceName = "piece_groundtorch_mist",
                newPieceName = "RareWeaponEnchant",
                pieceConfig = new PieceConfig() {
                    Description = "Rare materials for enchanting a weapon",
                    Requirements = new[] {
                        new RequirementConfig("RunestoneRare", 1, 0, true),
                        new RequirementConfig("ShardRare", 5, 0, true),
                        new RequirementConfig("EssenceRare", 5, 0, true),
                        new RequirementConfig("Coins", 50, 0, true)
                    }
                }
            },
            new CustomPieceClone() {
                vanillaPieceName = "piece_groundtorch_mist",
                newPieceName = "EpicWeaponEnchant",
                pieceConfig = new PieceConfig() {
                    Description = "Rare materials for enchanting a weapon",
                    Requirements = new[] {
                        new RequirementConfig("RunestoneEpic", 1, 0, true),
                        new RequirementConfig("ShardEpic", 5, 0, true),
                        new RequirementConfig("EssenceEpic", 5, 0, true),
                        new RequirementConfig("Coins", 75, 0, true)
                    }
                }
            },
            new CustomPieceClone() {
                vanillaPieceName = "piece_groundtorch_mist",
                newPieceName = "LegendaryWeaponEnchant",
                pieceConfig = new PieceConfig() {
                    Description = "Rare materials for enchanting a weapon",
                    Requirements = new[] {
                        new RequirementConfig("RunestoneLegendary", 1, 0, true),
                        new RequirementConfig("ShardLegendary", 5, 0, true),
                        new RequirementConfig("EssenceLegendary", 5, 0, true),
                        new RequirementConfig("Coins", 100, 0, true)
                    }
                }
            }
        };
    }
}
