using Environment;
using UnityEngine;

namespace Game.Tools {
	public class DoorSealer : Tool {
		private SectorDoor m_SectorActive;
		
		public override void UseItem() {
			if (canUse) {
				m_SectorActive.SealDoor();
				print("Used!");
			} else {
				print("Can't use!");
			}
		}

		public override void SetUsable(bool usable, string type, GameObject door) {
			if (type == "Seal") {
				canUse = usable;
				m_SectorActive = usable ? door.GetComponent<SectorDoor>() : null;
			}
		}
	}
}
