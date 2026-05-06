using System.Collections.Generic;
using System.Linq;
using MapMarkersPlus.Common;
using PugMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MapMarkersPlus {
	public class Main : IMod {
		public const string Version = "1.1";
		public const string InternalName = "MapMarkersPlus";
		public const string DisplayName = "MapMarkers+";
		
		internal static AssetBundle AssetBundle { get; private set; }
		internal static Dictionary<string, Sprite> Sprites { get; private set; } = new();

		public void EarlyInit() {
			var modInfo = API.ModLoader.LoadedMods.First(modInfo => modInfo.Handlers.Contains(this));
			Debug.Log($"[{DisplayName}]: Mod version: {Version}");

			AssetBundle = modInfo!.AssetBundles[0];
			foreach (var sprite in AssetBundle.LoadAllAssets<Sprite>())
				Sprites.TryAdd(sprite.name, sprite);
			
			PlusMarkerManager.Instance.Init();
		}

		public void Init() { }

		public void Shutdown() { }

		public void ModObjectLoaded(Object obj) { }

		public void Update() { }
	}
}