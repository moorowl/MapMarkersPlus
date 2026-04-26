using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PugMod;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace MapMarkersPlus.Common.UserInterface {
	public class MarkerDrawerUI : UIelement {
		private static readonly List<UserMapMarkerType> _recentMarkerTypes = new();
		private static readonly List<UserMapMarkerToggle> _recentMarkerToggles = new();
		
		public GameObject root;
		public MarkerList list;

		private bool _isOpen;

		public void ToggleState() {
			_isOpen = !_isOpen;
			root.SetActive(_isOpen);
		}

		private void Awake() {
			root.SetActive(_isOpen);

			foreach (var category in PlusMarkerCategory.All)
				list.AddCategory(category);
		}

		public void PushToRecent(UserMapMarkerType type) {
			_recentMarkerTypes.Remove(type);
			_recentMarkerTypes.Insert(0, type);
				
			if (_recentMarkerTypes.Count > _recentMarkerToggles.Count)
				_recentMarkerTypes.RemoveAt(_recentMarkerTypes.Count - 1);

			for (var i = 0; i < _recentMarkerToggles.Count; i++) {
				var recentMarkerToggle = _recentMarkerToggles[i];
				if (!PlusMarkerManager.Instance.TryGetMarker(_recentMarkerTypes[i], out var marker))
					continue;

				recentMarkerToggle.userMapMarkerType = _recentMarkerTypes[i];
				recentMarkerToggle.isOn = _recentMarkerTypes[i] == type;

				foreach (var sr in recentMarkerToggle.deactivatedSprites)
					sr.sprite = marker.GetIcon(false);
				foreach (var sr in recentMarkerToggle.activatedSprites)
					sr.sprite = marker.GetIcon(false);
			}
		}
		
		[HarmonyPatch]
		public static class Patches {
			private const string MarkerDrawerPrefabPath = "Assets/MapMarkersPlus/Prefabs/MarkerDrawerUI.prefab";

			private static readonly MemberInfo MiCurrentUserMapMarkerToggle = typeof(MapUI).GetMembersChecked().First(x => x.GetNameChecked() == "_currentUserMapMarkerToggle");

			[HarmonyPatch(typeof(MapUI), "Awake")]
			[HarmonyPostfix]
			public static void MapUI_Awake(MapUI __instance) {
				// Modify mapUI and instantiate the marker drawer

				var toggleGroup = Manager.ui.mapUI.transform.Find("container/largeMapBorder");
				var markerDrawerPrefab = Main.AssetBundle.LoadAsset<GameObject>(MarkerDrawerPrefabPath);
				var markerDrawer = Instantiate(markerDrawerPrefab, toggleGroup.transform).GetComponent<MarkerDrawerUI>();

				var background = Manager.ui.mapUI.transform.Find("container/largeMapBorder/MapMarkerToggles/background")?.GetComponent<SpriteRenderer>();
				if (background != null) {
					background.transform.localPosition = new Vector3(
						background.transform.localPosition.x + (12.5f / 16f),
						background.transform.localPosition.y,
						background.transform.localPosition.z
					);
					background.size = new Vector2(background.size.x + (24f / 16f), background.size.y);
				}

				var border = Manager.ui.mapUI.transform.Find("container/largeMapBorder/MapMarkerToggles/border")?.GetComponent<SpriteRenderer>();
				if (border != null) {
					border.sortingOrder += 3;
					border.transform.localPosition = new Vector3(
						border.transform.localPosition.x + (12.5f / 16f),
						border.transform.localPosition.y,
						border.transform.localPosition.z
					);
					border.size = new Vector2(border.size.x + (24f / 16f), border.size.y);
				}
				
				_recentMarkerTypes.Clear();
				_recentMarkerToggles.Clear();
				
				var toggleUIGroup = __instance.transform.Find("container/largeMapBorder/MapMarkerToggles").GetComponent<ToggleUIGroup>();
				foreach (var toggleUIElement in toggleUIGroup.toggleUIElements) {
					toggleUIElement.topUIElements.Add(markerDrawer.list);
					markerDrawer.list.bottomUIElements.Add(toggleUIElement);

					if (toggleUIElement is UserMapMarkerToggle userMapMarkerToggle) {
						_recentMarkerToggles.Add(userMapMarkerToggle);
						_recentMarkerTypes.Add(userMapMarkerToggle.userMapMarkerType);
					}
				}
			}

			[HarmonyPatch(typeof(MapMarkerUIElement), "LateUpdate")]
			[HarmonyPostfix]
			public static void MapMarkerUIElement_LateUpdate(MapMarkerUIElement __instance) {
				if (__instance.markerType == MapMarkerType.UserPlacedMarker) {
					var amount = EntityUtility.GetAmount(__instance.mapMarkerEntity, API.Client.World);

					if (PlusMarkerManager.Instance.TryGetMarker(amount, out var marker) && marker.IsModded)
						__instance.sr.sprite = marker.GetIcon(!Manager.ui.mapUI.IsShowingBigMap);
				}
			}
			
			[HarmonyPatch(typeof(ToggleUIGroup), "OnToggle")]
			[HarmonyPrefix]
			public static bool ToggleUIGroup_OnToggle(ToggleUIGroup __instance, ToggleUIElement toggleUIElementActivated) {
				if (toggleUIElementActivated is UserMapMarkerToggle currentUserMapMarkerToggle) {
					foreach (var toggleUIElement in __instance.toggleUIElements) {
						if (toggleUIElement is not UserMapMarkerToggle otherUserMapMarkerToggle)
							continue;
						
						otherUserMapMarkerToggle.isOn = currentUserMapMarkerToggle.userMapMarkerType == otherUserMapMarkerToggle.userMapMarkerType;
					}

					return false;
				}

				return true;
			}
		}
	}
}