using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Environment {
	public class EnvironmentControl : MonoBehaviour {
		public bool isSafe = true;
		public bool isSealed = true;
		public float oxygen = 100;
		public bool hasOxygen = true;
		public PlayerStats masterPlayer;

		private void Update() {
			if (!isSafe && !isSealed) {
				if (masterPlayer) {
					if (oxygen > 0) {
						oxygen = oxygen - (Time.deltaTime * 10);
					}

					if (oxygen < 50) {
						hasOxygen = false;
					}
				}
			} else {
				if (masterPlayer) {
					if (oxygen < 100) {
						oxygen = oxygen + (Time.deltaTime * 10);
					}
					
					if (oxygen > 50) {
						hasOxygen = true;
					}
				}
			}
		}

		private void Start() {
			masterPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
		}

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
			
			if (isSafe && hasOxygen) {
				player.enteredSector = true;
				player.outOfArea = false;
				
			} else {
				player.enteredSector = false;
				player.outOfArea = true;
				player.ReduceOxygen(0.15f);
			}

			if (!isSafe) {
				GameObject topTarget;
				for (var test = 0; test < GameObject.FindGameObjectsWithTag("Suction").Length; test++) {
					GameObject target = GameObject.FindGameObjectsWithTag("Suction")[test];
					if (Physics.Linecast(masterPlayer.transform.position, target.transform.position)) {
 
						print("YES");
 
					} else {
						print("NOOOO");
					}
				}
				
				
			}
		}
	}
}
