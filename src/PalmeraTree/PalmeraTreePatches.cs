using System;
using System.Collections.Generic;
using Harmony;
using TUNING;

namespace PalmeraTree
{
	public class PalmeraTreePatches
	{
		[HarmonyPatch(typeof(EntityConfigManager))]
		[HarmonyPatch("LoadGeneratedEntities")]
		public class EntityConfigManager_LoadGeneratedEntities_Patch
		{
			private static void Prefix()
			{
				Strings.Add($"STRINGS.BUILDINGS.PREFABS.{TrellisConfig.Id.ToUpperInvariant()}.NAME", TrellisConfig.Id);
				Strings.Add($"STRINGS.BUILDINGS.PREFABS.{TrellisConfig.Id.ToUpperInvariant()}.DESC", TrellisConfig.Description);
				Strings.Add($"STRINGS.BUILDINGS.PREFABS.{TrellisConfig.Id.ToUpperInvariant()}.EFFECT", TrellisConfig.Effect);

				Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{PalmeraTreeConfig.Id.ToUpperInvariant()}.NAME", PalmeraTreeConfig.SeedName);
				Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{PalmeraTreeConfig.Id.ToUpperInvariant()}.DESC", PalmeraTreeConfig.SeedDescription);

				Strings.Add($"STRINGS.CREATURES.SPECIES.{PalmeraTreeConfig.Id.ToUpperInvariant()}.NAME", PalmeraTreeConfig.Name);
				Strings.Add($"STRINGS.CREATURES.SPECIES.{PalmeraTreeConfig.Id.ToUpperInvariant()}.DESC", PalmeraTreeConfig.Description);
				Strings.Add($"STRINGS.CREATURES.SPECIES.{PalmeraTreeConfig.Id.ToUpperInvariant()}.DOMESTICATEDDESC", PalmeraTreeConfig.DomesticatedDescription);

				Strings.Add($"STRINGS.ITEMS.FOOD.{SteamedPalmeraBerryConfig.Id.ToUpperInvariant()}.NAME", SteamedPalmeraBerryConfig.Name);
				Strings.Add($"STRINGS.ITEMS.FOOD.{SteamedPalmeraBerryConfig.Id.ToUpperInvariant()}.DESC", SteamedPalmeraBerryConfig.Description);
				Strings.Add($"STRINGS.ITEMS.FOOD.{SteamedPalmeraBerryConfig.Id.ToUpperInvariant()}.RECIPEDESC", SteamedPalmeraBerryConfig.RecipeDescription);

				Strings.Add($"STRINGS.ITEMS.FOOD.{PalmeraBerryConfig.Id.ToUpperInvariant()}.NAME", PalmeraBerryConfig.Name);
				Strings.Add($"STRINGS.ITEMS.FOOD.{PalmeraBerryConfig.Id.ToUpperInvariant()}.DESC", PalmeraBerryConfig.Description);

				ModUtil.AddBuildingToPlanScreen("Food", TrellisConfig.Id);

				CROPS.CROP_TYPES.Add(new Crop.CropVal(PalmeraBerryConfig.Id, 12000f, 10));
			}
		}

		[HarmonyPatch(typeof(SupermaterialRefineryConfig))]
		[HarmonyPatch("ConfigureBuildingTemplate")]
		public class SupermaterialRefineryConfig_ConfigureBuildingTemplate_Patch
		{
			private const string MolecularForgeId = "SupermaterialRefinery";

			private static void Postfix()
			{
				var ingredients = new[]
				{
					new ComplexRecipe.RecipeElement(TagManager.Create("BasicSingleHarvestPlantSeed"), 10f),
					new ComplexRecipe.RecipeElement(BasicFabricConfig.ID.ToTag(), 10f)
				};

				var result = new[]
				{
					new ComplexRecipe.RecipeElement(TagManager.Create(PalmeraTreeConfig.SeedId), 1f)
				};

				new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(MolecularForgeId, ingredients, result), ingredients,
					result)
				{
					time = 50f,
					description = "What will happen if you mash some organic mass together?",
					useResultAsDescription = true
				}
					.fabricators = new List<Tag>()
				{
					TagManager.Create(MolecularForgeId)
				};
			}
		}

		[HarmonyPatch(typeof(BuildingLoader))]
		[HarmonyPatch("Add2DComponents")]
		public class BuildingLoader_Add2DComponents_Patch
		{
			private static void Prefix(ref string initialAnimState, BuildingDef def)
			{
				if (initialAnimState == "place" && def.Name.ToLower().Contains("trellis"))
				{
					initialAnimState = "place_1";
				}
			}
		}

		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch("Initialize")]
		public static class Db_Initialize_Patch
		{
			public static void Prefix()
			{
				var tech = new List<string>(Database.Techs.TECH_GROUPING["FarmingTech"]) { TrellisConfig.Id };
				Database.Techs.TECH_GROUPING["FarmingTech"] = tech.ToArray();
			}
		}

		[HarmonyPatch(typeof(KSerialization.Manager))]
		[HarmonyPatch("GetType")]
		[HarmonyPatch(new[] { typeof(string) })]
		public static class KSerializationManager_GetType_Patch
		{
			public static void Postfix(string type_name, ref Type __result)
			{
				if (type_name == "PalmeraTree.PalmeraTree")
				{
					__result = typeof(PalmeraTree);
				}
			}
		}
	}
}
