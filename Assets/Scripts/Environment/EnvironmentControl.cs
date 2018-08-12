using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Environment {
	public class EnvironmentControl : MonoBehaviour {
		public bool isSafe = true;
		public bool sucked;
		public float oxygen = 100;
		public bool hasOxygen = true;
		public PlayerStats masterPlayer;

		private void Update() {
			if (!isSafe) {
				if (masterPlayer) {
					if (oxygen > 0) {
						oxygen = oxygen - (Time.deltaTime * 10);
					}

					if (oxygen < 50) {
						masterPlayer.outOfArea = true;
						masterPlayer.enteredSector = false;
					}	
				}
			} else {
				if (masterPlayer) {
					if (oxygen < 100) {
						oxygen = oxygen + (Time.deltaTime * 10);
					}
				
					if (oxygen > 50) {
						masterPlayer.outOfArea = false;
						masterPlayer.enteredSector = true;
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
					//other.transform.position = Vector3.MoveTowards(transform.position, gameObject.transform.position, step);//
					StartCoroutine(MovePieceTowards(other, GameObject.FindGameObjectWithTag("Suction").transform.position, test / 10));
				}

				if (test < 0.25f) {
					sucked = true;
				}
			}
		}

		private IEnumerator MovePieceTowards(Collider piece, Vector3 end, float speed) {
			while (piece.transform.position != end && !sucked) {
				piece.transform.position = Vector3.MoveTowards(piece.transform.position, end, speed * Time.deltaTime);
				yield return null;
			}
		}
	}
}
