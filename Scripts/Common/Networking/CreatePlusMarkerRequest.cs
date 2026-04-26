using Unity.Mathematics;
using Unity.NetCode;

namespace MapMarkersPlus.Common.Networking {
	public struct CreatePlusMarkerRequest : IRpcCommand {
		public float3 Position;
		public PlusMarkerType Type;
	}
}