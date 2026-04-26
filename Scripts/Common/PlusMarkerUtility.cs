namespace MapMarkersPlus.Common {
	public static class PlusMarkerUtility {
		public static bool TryGetVanillaInfo(PlusMarkerType type, out int mapMarkerVariation, out UserMapMarkerType userMapMarkerType) {
			switch (type) {
				case PlusMarkerType.AncientCrystal:
					mapMarkerVariation = 0;
					userMapMarkerType = UserMapMarkerType.Marker1;
					return true;
				case PlusMarkerType.QuestionMark:
					mapMarkerVariation = 1;
					userMapMarkerType = UserMapMarkerType.Marker2;
					return true;
				case PlusMarkerType.Skull:
					mapMarkerVariation = 2;
					userMapMarkerType = UserMapMarkerType.Marker3;
					return true;
				case PlusMarkerType.FlagGreen:
					mapMarkerVariation = 3;
					userMapMarkerType = UserMapMarkerType.Marker4;
					return true;
				case PlusMarkerType.Ping:
					mapMarkerVariation = -1;
					userMapMarkerType = UserMapMarkerType.Ping;
					return true;
				default:
					mapMarkerVariation = -1;
					userMapMarkerType = UserMapMarkerType.None;
					return false;
			}
		}
	}
}