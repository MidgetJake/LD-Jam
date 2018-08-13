using Environment;
using Player;
using UnityEngine;

namespace Game.Tools {
	public class DoorSealer : Tool {
		private SectorDoor m_SectorActive;
		public GameObject door;
		
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

		private void OnTriggerEnter(Collider other) {
			if (other.CompareTag("Player")) {
				other.GetComponent<Controller>().currDoor = this;
			}
		}
		
		private void OnTriggerStay(Collider other) {
			if (other.CompareTag("Player")) {
				other.GetComponent<Controller>().currDoor = this;
			}
		}
		
		private void OnTriggerExit(Collider other) {
			if (other.CompareTag("Player")) {
				other.GetComponent<Controller>().currDoor = null;
			}
		}
	}
}
