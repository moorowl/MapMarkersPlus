using System.Collections.Generic;

namespace MapMarkersPlus.Common {
	public class PlusMarkerCategory {
		public static readonly PlusMarkerCategory Ping = new("Ping", new List<PlusMarkerType> {
			PlusMarkerType.Ping
		});
		
		public static readonly PlusMarkerCategory General = new("General", new List<PlusMarkerType> {
			PlusMarkerType.QuestionMark,
			PlusMarkerType.ExclamationMark,
			PlusMarkerType.MusicNote,
			PlusMarkerType.Cross,
			PlusMarkerType.ArrowLeft,
			PlusMarkerType.ArrowRight,
			PlusMarkerType.ArrowUp,
			PlusMarkerType.ArrowDown,
			PlusMarkerType.Chest,
			PlusMarkerType.Sign,
			PlusMarkerType.StructureWood,
			PlusMarkerType.StructureStone,
			PlusMarkerType.Leaf,
			PlusMarkerType.Fish,
			PlusMarkerType.Cog,
			PlusMarkerType.Heart,
			PlusMarkerType.Skull,
			PlusMarkerType.SkullRed,
			PlusMarkerType.Flames,
			PlusMarkerType.Shield,
			PlusMarkerType.Dagger,
			PlusMarkerType.Axe
		});
		
		public static readonly PlusMarkerCategory OresAndGems = new("OresAndGems", new List<PlusMarkerType> {
			PlusMarkerType.AncientCrystal,
			PlusMarkerType.Copper,
			PlusMarkerType.Tin,
			PlusMarkerType.Iron,
			PlusMarkerType.Gold,
			PlusMarkerType.Scarlet,
			PlusMarkerType.Octarine,
			PlusMarkerType.Galaxite,
			PlusMarkerType.Solarite,
			PlusMarkerType.Pandorium,
			PlusMarkerType.Relucite
		});

		public static readonly PlusMarkerCategory Flags = new("Flags", new List<PlusMarkerType> {
			PlusMarkerType.FlagRed,
			PlusMarkerType.FlagOrange,
			PlusMarkerType.FlagPeach,
			PlusMarkerType.FlagYellow,
			PlusMarkerType.FlagGreen,
			PlusMarkerType.FlagTeal,
			PlusMarkerType.FlagCyan,
			PlusMarkerType.FlagBlue,
			PlusMarkerType.FlagPurple,
			PlusMarkerType.FlagPink,
			PlusMarkerType.FlagBrown,
			PlusMarkerType.FlagBlack,
			PlusMarkerType.FlagGray,
			PlusMarkerType.FlagWhite
		});

		public static readonly PlusMarkerCategory Letters = new("Letters", new List<PlusMarkerType> {
			PlusMarkerType.LetterA,
			PlusMarkerType.LetterB,
			PlusMarkerType.LetterC,
			PlusMarkerType.LetterD,
			PlusMarkerType.LetterE,
			PlusMarkerType.LetterF,
			PlusMarkerType.LetterG,
			PlusMarkerType.LetterH,
			PlusMarkerType.LetterI,
			PlusMarkerType.LetterJ,
			PlusMarkerType.LetterK,
			PlusMarkerType.LetterL,
			PlusMarkerType.LetterM,
			PlusMarkerType.LetterN,
			PlusMarkerType.LetterO,
			PlusMarkerType.LetterP,
			PlusMarkerType.LetterQ,
			PlusMarkerType.LetterR,
			PlusMarkerType.LetterS,
			PlusMarkerType.LetterT,
			PlusMarkerType.LetterU,
			PlusMarkerType.LetterV,
			PlusMarkerType.LetterW,
			PlusMarkerType.LetterX,
			PlusMarkerType.LetterY,
			PlusMarkerType.LetterZ
		});

		public static readonly PlusMarkerCategory Numbers = new("Numbers", new List<PlusMarkerType> {
			PlusMarkerType.Number1,
			PlusMarkerType.Number2,
			PlusMarkerType.Number3,
			PlusMarkerType.Number4,
			PlusMarkerType.Number5,
			PlusMarkerType.Number6,
			PlusMarkerType.Number7,
			PlusMarkerType.Number8,
			PlusMarkerType.Number9,
			PlusMarkerType.Number0
		});

		public static readonly List<PlusMarkerCategory> All = new() {
			Ping,
			General,
			OresAndGems,
			Flags,
			// Letters,
			Numbers
		};

		public readonly string Name;

		private readonly List<PlusMarker> _markers;
		public IReadOnlyList<PlusMarker> Markers => _markers;

		public PlusMarkerCategory(string name, List<PlusMarkerType> types) {
			Name = name;
			_markers = new List<PlusMarker>();

			foreach (var type in types) {
				if (PlusMarkerUtility.TryGetVanillaInfo(type, out var mapMarkerVariation, out var userMapMarkerType))
					_markers.Add(new PlusMarker(type, this, mapMarkerVariation, userMapMarkerType));
				else
					_markers.Add(new PlusMarker(type, this));
			}
		}
	}
}