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
		public bool sucked = false;
		
		private GameObject topTarget;

		private void Update() {
			if (Vector3.Distance(masterPlayer.transform.position, topTarget.transform.position) < 3) {
				sucked = true;
			}
			

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
				sucked = false;

			} else {
				player.enteredSector = false;
				player.outOfArea = true;
				player.ReduceOxygen(0.15f);
			}

			if (!isSafe) {
				
				for (var test = 0; test < GameObject.FindGameObjectsWithTag("Suction").Length; test++) {
					GameObject target = GameObject.FindGameObjectsWithTag("Suction")[test];
					
					int layerMask = 1 << 2;
					
					if (!Physics.Linecast(masterPlayer.transform.position, target.transform.position, layerMask)) {
						print("YES");
						if (Vector3.Distance(masterPlayer.transform.position, target.transform.position) < 3) {
							sucked = true;
						} else {
							topTarget = target;
							if (!sucked) {
								StartCoroutine(MovePieceTowards(other, GameObject.FindGameObjectWithTag("Suction").transform.position, 5f / 10));
							}
						}
						
					}
				}
			}
		}
		
		private IEnumerator MovePieceTowards(Collider piece, Vector3 end, float speed) {
			while (piece.transform.position != end) {
				piece.transform.position = Vector3.MoveTowards(piece.transform.position, end, speed * Time.deltaTime);
				yield return null;
			}
		}
	}
}
