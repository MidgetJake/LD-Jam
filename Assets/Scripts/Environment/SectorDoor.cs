using Player;
using UnityEngine;

namespace Environment {
	public class SectorDoor : MonoBehaviour {
		[SerializeField] private GameObject m_Door;

		public void SealDoor() {
			m_Door.SetActive(true);
		}
		
		private void OnTriggerEnter(Collider other) {
			if (other.CompareTag("Player")) {
				Controller player = other.GetComponent<Controller>();
				player.SetItemUsable(true, "Seal", gameObject);
			}
		}

		private void OnTriggerExit(Collider other) {
			if (other.CompareTag("Player")) {
				Controller player = other.GetComponent<Controller>();
				player.SetItemUsable(false, "Seal", gameObject);
			}
		}
	}
}
