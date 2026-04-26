using System.Collections.Generic;
using UnityEngine;

namespace MapMarkersPlus.Common {
	public class PlusMarker {
		private const int AmountBase = 6000;
		private const int PlusUserMapMarkerVariation = 1;
		private const UserMapMarkerType PlusUserMapMarkerType = UserMapMarkerType.Marker2;
		
		public readonly PlusMarkerType Type;
		public readonly PlusMarkerCategory Category;
		public readonly int MapMarkerAmount;
		public readonly int MapMarkerVariation;
		public readonly UserMapMarkerType UserMapMarkerType;

		private bool _loadedSprites;
		private Sprite _icon;
		private Sprite _smallIcon;

		public bool IsModded => MapMarkerAmount > 3;
		
		public PlusMarker(PlusMarkerType type, PlusMarkerCategory category) {
			Type = type;
			Category = category;
			MapMarkerAmount = AmountBase + (int) Type;
			MapMarkerVariation = PlusUserMapMarkerVariation;
			UserMapMarkerType = PlusUserMapMarkerType;
		}
		
		public PlusMarker(PlusMarkerType type, PlusMarkerCategory category, int mapMarkerVariation, UserMapMarkerType userMapMarkerType) {
			Type = type;
			Category = category;
			MapMarkerAmount = 0;
			MapMarkerVariation = mapMarkerVariation;
			UserMapMarkerType = userMapMarkerType;
		}
		
		public Sprite GetIcon(bool isSmall) {
			if (!_loadedSprites)
				LoadIcons();
			
			return isSmall ? _smallIcon : _icon;
		}

		private void LoadIcons() {
			_icon = Main.Sprites.GetValueOrDefault($"markers_{Type}") ?? Main.Sprites.GetValueOrDefault("markers_Unknown");
			_smallIcon = Main.Sprites.GetValueOrDefault($"markers_{Type}_small") ?? Main.Sprites.GetValueOrDefault("markers_Unknown_small");
			_loadedSprites = true;
		}
	}
}