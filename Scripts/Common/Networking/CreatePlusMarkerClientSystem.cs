using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace MapMarkersPlus.Common.Networking {
	[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
	[UpdateInGroup(typeof(RunSimulationSystemGroup))]
	public partial class CreatePlusMarkerClientSystem : PugSimulationSystemBase {
		private NativeQueue<CreatePlusMarkerRequest> _requestQueue;
		private EntityArchetype _requestArchetype;
		
		protected override void OnCreate() {
			UpdatesInRunGroup();

			_requestQueue = new NativeQueue<CreatePlusMarkerRequest>(Allocator.Persistent);
			_requestArchetype = EntityManager.CreateArchetype(typeof(CreatePlusMarkerRequest), typeof(SendRpcCommandRequest));

			base.OnCreate();
		}
		
		protected override void OnDestroy() {
			_requestQueue.Dispose();

			base.OnDestroy();
		}

		protected override void OnUpdate() {
			var ecb = CreateCommandBuffer();

			while (_requestQueue.TryDequeue(out var request)) {
				var entity = ecb.CreateEntity(_requestArchetype);
				ecb.SetComponent(entity, request);
			}
			
			base.OnUpdate();
		}

		public void CreatePlusMarker(float3 worldPosition, PlusMarkerType type) {
			_requestQueue.Enqueue(new CreatePlusMarkerRequest {
				Position = worldPosition,
				Type = type
			});
		}
	}
}