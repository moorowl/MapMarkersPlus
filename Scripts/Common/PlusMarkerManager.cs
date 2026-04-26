using System.Collections.Generic;
using HarmonyLib;
using MapMarkersPlus.Common.Networking;
using PlayerCommand;
using PugMod;
using Unity.Entities;
using Unity.Mathematics;

// ReSharper disable InconsistentNaming

namespace MapMarkersPlus.Common {
	public class PlusMarkerManager {
		public static PlusMarkerManager Instance { get; private set; } = new();

		private readonly Dictionary<PlusMarkerType, PlusMarker> _typeToMarker = new();
		private readonly Dictionary<int, PlusMarkerType> _markerAmountToType = new();
		private readonly Dictionary<UserMapMarkerType, PlusMarkerType> _userMarkerTypeToType = new();

		private static CreatePlusMarkerClientSystem _clientSystem;

		public void Init() {
			API.Client.OnWorldCreated += () => {
				_clientSystem = API.Client.World.GetOrCreateSystemManaged<CreatePlusMarkerClientSystem>();
			};
			
			foreach (var category in PlusMarkerCategory.All) {
				foreach (var marker in category.Markers)
					RegisterMarker(marker);
			}
		}

		private void RegisterMarker(PlusMarker marker) {
			_typeToMarker[marker.Type] = marker;
			_markerAmountToType[marker.MapMarkerAmount] = marker.Type;
			_userMarkerTypeToType[marker.MapMarkerAmount > 0 ? (UserMapMarkerType) marker.MapMarkerAmount : marker.UserMapMarkerType] = marker.Type;
		}

		public bool TryGetMarker(PlusMarkerType type, out PlusMarker marker) {
			return _typeToMarker.TryGetValue(type, out marker);
		}

		public bool TryGetMarker(int mapMarkerAmount, out PlusMarker marker) {
			marker = null;
			return _markerAmountToType.TryGetValue(mapMarkerAmount, out var type) && TryGetMarker(type, out marker);
		}
		
		public bool TryGetMarker(UserMapMarkerType userMapMarkerType, out PlusMarker marker) {
			marker = null;
			return _userMarkerTypeToType.TryGetValue(userMapMarkerType, out var type) && TryGetMarker(type, out marker);
		}

		[HarmonyPatch]
		public static class Patches {
			[HarmonyPatch(typeof(ClientSystem), "CreateMapUI")]
			[HarmonyPrefix]
			public static bool ClientSystem_CreateMapUI(ClientSystem __instance, float3 worldPosition, int activeUserMapMarkerType) {
				if (Instance.TryGetMarker(activeUserMapMarkerType, out var marker) && marker.IsModded) {
					_clientSystem.CreatePlusMarker(worldPosition, marker.Type);
					return false;
				}

				return true;
			}
			
			[HarmonyPatch(typeof(EntityPrespawnSystem), "CreatePrespawnEntity")]
			[HarmonyPrefix]
			public static void EntityPrespawnSystem_CreatePrespawnEntity_Prefix(EntityPrespawnSystem __instance, ref ObjectDataCD objectData, float3 position, float3 direction, out bool __state) {
				__state = false;
				
				if (objectData.objectID == ObjectID.MapMarker && Instance.TryGetMarker(objectData.variation + 2, out var marker) && marker.IsModded) {
					objectData = new ObjectDataCD {
						objectID = objectData.objectID,
						variation = marker.MapMarkerVariation,
						amount = marker.MapMarkerAmount
					};
					__state = true;
				}
			}
			
			[HarmonyPatch(typeof(EntityPrespawnSystem), "CreatePrespawnEntity")]
			[HarmonyPostfix]
			public static void EntityPrespawnSystem_CreatePrespawnEntity_Postfix(EntityPrespawnSystem __instance, ObjectDataCD objectData, float3 position, float3 direction, ref Entity __result, bool __state) {
				if (__state && __result != Entity.Null)
					EntityUtility.SetAmount(__result, API.Client.World, objectData.amount);
			}
		}
	}
}