using UnityEngine;

namespace Environment {
	public class EnvironmentControl : MonoBehaviour {
		public bool isSafe;
	
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
			if (isSafe) {
				PlayerStats player = other.GetComponent<PlayerStats>();
				if (!other.transform.CompareTag("Player") || !player) return;
				print("ENTER");
				player.outOfArea = false;
				player.enteredSector = true;
			}
		}
		
		private void OnTriggerStay(Collider other) {
			if (!other.transform.CompareTag("Player")) return;
			PlayerStats player = other.GetComponent<PlayerStats>();
			if (isSafe) {
				player.enteredSector = true;
				player.outOfArea = false;
			} else {
				player.enteredSector = true;
				player.outOfArea = true;
			}
		}
	}
}
