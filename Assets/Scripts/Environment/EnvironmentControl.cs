using System.Collections;
using UnityEngine;

namespace Environment {
	public class EnvironmentControl : MonoBehaviour {
		public bool isSafe;
		public bool sucked;
	
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

			if (!isSafe) {
				float test = Vector3.Distance(other.transform.position, GameObject.FindGameObjectWithTag("Suction").transform.position);
				print(test);
				if (test > 5) {
					//other.transform.position = Vector3.MoveTowards(transform.position, gameObject.transform.position, step);
					StartCoroutine(MovePieceTowards(other, GameObject.FindGameObjectWithTag("Suction").transform.position, test / 10));
				}

				if (test < 0.25f) {
					sucked = true;
				}
			}
		}
		
		IEnumerator MovePieceTowards(Collider piece, Vector3 end, float speed) {
			while (piece.transform.position != end && !sucked) {
				piece.transform.position = Vector3.MoveTowards(piece.transform.position, end, speed * Time.deltaTime);
				yield return null;
			}
		}
	}
}
