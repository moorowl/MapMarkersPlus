using Unity.Entities;
using Unity.NetCode;

namespace MapMarkersPlus.Common.Networking {
	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	[UpdateInGroup(typeof(RunSimulationSystemGroup))]
	public partial class CreatePlusMarkerServerSystem : PugSimulationSystemBase {
		protected override void OnCreate() {
			NeedDatabase();
			
			base.OnCreate();
		}

		protected override void OnUpdate() {
			var ecb = CreateCommandBuffer();
			var pugDatabaseBlob = database;

			Entities.ForEach((Entity entity, in CreatePlusMarkerRequest request, in ReceiveRpcCommandRequest receiveRpc) => {
				if (PlusMarkerManager.Instance.TryGetMarker(request.Type, out var marker))
					EntityUtility.CreateEntity(ecb, request.Position, ObjectID.MapMarker, marker.MapMarkerAmount, pugDatabaseBlob, out _, marker.MapMarkerVariation);

				ecb.DestroyEntity(entity);
			})
			.WithoutBurst()
			.Schedule();

			base.OnUpdate();
		}
	}
}