namespace MapMarkersPlus.Common.UserInterface {
	public class UserMapMarkerToggleInDrawer : UserMapMarkerToggle {
		public override float localScrollPosition => transform.localPosition.y + transform.parent.localPosition.y - 0.5f;
		public override bool isVisibleOnScreen => uiScrollWindow == null || uiScrollWindow.IsShowingPosition(localScrollPosition);
		public override UIScrollWindow uiScrollWindow => _markerDrawer.list.scrollWindow;
		
		private MarkerDrawerUI _markerDrawer;

		public void SetMarkerDrawer(MarkerDrawerUI markerDrawer) {
			_markerDrawer = markerDrawer;
		}

		public override void OnLeftClicked(bool mod1, bool mod2) {
			base.OnLeftClicked(mod1, mod2);
			
			_markerDrawer.PushToRecent(userMapMarkerType);
		}

		public override void OnSelected() {
			base.OnSelected();
			
			_markerDrawer.list.scrollWindow.MoveScrollToIncludePosition(localScrollPosition, 1f);
		}
	}
}