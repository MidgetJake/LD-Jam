using UnityEngine;

namespace Environment {
	public class EnvironmentControl : MonoBehaviour {
	
		private void OnTriggerExit(Collider other) {
			PlayerStats player = other.GetComponent<PlayerStats>();
			if (other.transform.CompareTag("Player") && !player.enteredSector && player) {
				print("LEFT");
				player.outOfArea = true;
			} else {
				player.enteredSector = false;
			}
		}
		
		private void OnTriggerEnter(Collider other) {
			PlayerStats player = other.GetComponent<PlayerStats>();
			if (!other.transform.CompareTag("Player") || !player) return;
			print("ENTER");
			player.outOfArea = false;
			player.enteredSector = true;
		}
	}
}
