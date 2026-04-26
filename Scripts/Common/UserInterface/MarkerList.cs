using System.Linq;
using HarmonyLib;
using PugMod;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace MapMarkersPlus.Common.UserInterface {
	public class MarkerList : UIelement, IScrollable {
		private static bool _isBeingHovered;
		private static readonly MemberInfo IsMouseWithinScrollAreaMember = typeof(UIScrollWindow).GetMembersChecked().First(x => x.GetNameChecked() == "IsMouseWithinScrollArea");

		public MarkerDrawerUI markerDrawer;
		public UIScrollWindow scrollWindow;
		public PugText headerTemplate;
		public UserMapMarkerToggleInDrawer markerTemplate;
		public Transform contents;
		public Vector2 markerSpacing = new(17f / 16f, 13f / 16f);
		public int markersPerRow = 7;
        public float bottomPadding = 3f / 16f;
        public float headerHorizontalOffset = -0.3f;
		
		private float _height;
		
		public void AddCategory(PlusMarkerCategory category) {
			var left = 0f;
			var markersInRow = 0;
			var toggleGroup = Manager.ui.mapUI.GetComponentInChildren<ToggleUIGroup>();

			var categoryTerm = $"MapMarkersPlus-Category/{category.Name}";
			if (API.Localization.GetLocalizedTerm(categoryTerm) != null) {
				var header = Instantiate(headerTemplate, contents);
				header.gameObject.SetActive(true);
				header.transform.localPosition = new Vector3(left + headerHorizontalOffset, _height, 0f);
				header.SetOutlines(true, true, true, true);
				header.Render(categoryTerm);
				
				_height -= markerSpacing.y;
			}

			for (var i = 0; i < category.Markers.Count; i++) {
				var marker = category.Markers[i];

				var userMapMarkerToggle = Instantiate(markerTemplate, contents);
				userMapMarkerToggle.SetMarkerDrawer(markerDrawer);
				userMapMarkerToggle.gameObject.SetActive(true);
				userMapMarkerToggle.userMapMarkerType = marker.MapMarkerAmount > 0 ? (UserMapMarkerType) marker.MapMarkerAmount : marker.UserMapMarkerType;
				userMapMarkerToggle.mapUI = Manager.ui.mapUI;
				foreach (var sr in userMapMarkerToggle.deactivatedSprites)
					sr.sprite = marker.GetIcon(false);
				foreach (var sr in userMapMarkerToggle.activatedSprites)
					sr.sprite = marker.GetIcon(false);
				userMapMarkerToggle.belongsToGroup = toggleGroup;
				userMapMarkerToggle.isOn = userMapMarkerToggle.userMapMarkerType == UserMapMarkerType.Ping;
				userMapMarkerToggle.transform.localPosition = new Vector3(left, _height, 0f);

				toggleGroup.toggleUIElements.Add(userMapMarkerToggle);
				childElements.Add(userMapMarkerToggle);
				
				left += markerSpacing.x;
				markersInRow++;
				
				if (markersInRow >= markersPerRow || i == category.Markers.Count - 1) {
					left = 0f;
					markersInRow = 0;
					_height -= markerSpacing.y;
				}
			}
		}

		protected override void LateUpdate() {
			base.LateUpdate();

			_isBeingHovered = (bool) API.Reflection.Invoke(IsMouseWithinScrollAreaMember, scrollWindow);
		}
		
		public override UIelement GetClosestUIElement(Vector3 position) {
			UIelement result = null;
			var closestDistance = float.MaxValue;

			foreach (var childElement in childElements) {
				if (!childElement.isShowing || !childElement.isVisibleOnScreen)
					continue;

				var distance = (position - childElement.transform.position).sqrMagnitude;
				if (distance <= closestDistance) {
					closestDistance = distance;
					result = childElement;
				}
			}
			return result;
		}

		public void UpdateContainingElements(float scroll) { }

		public bool IsBottomElementSelected() {
			return false;
		}

		public bool IsTopElementSelected() {
			return false;
		}

		public float GetCurrentWindowHeight() {
			return math.abs(_height) + bottomPadding;
		}

		[HarmonyPatch]
		public static class DisableMapInputPatches {
			[HarmonyPatch(typeof(MapUI), "UpdateZoom")]
			[HarmonyPrefix]
			public static bool MapUI_UpdateZoom(MapUI __instance) {
				return !_isBeingHovered;
			}
			
			[HarmonyPatch(typeof(MapUI), "UpdateMapFromUserInput")]
			[HarmonyPrefix]
			public static bool MapUI_UpdateMapFromUserInput(MapUI __instance) {
				return !_isBeingHovered;
			}
		}
	}
}