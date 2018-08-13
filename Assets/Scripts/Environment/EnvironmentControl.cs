using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngineInternal;

namespace Environment {
	public class EnvironmentControl : MonoBehaviour {
		public bool isSafe = true;
		public bool isSealed;
		public float oxygen = 100;
		public bool hasOxygen = true;
		public PlayerStats masterPlayer;
		public bool sucked = false;
		public bool isOpen;
		
		private GameObject m_TopTarget;
		
		private void LateUpdate() {
			if (m_TopTarget) {
				if (Vector3.Distance(masterPlayer.transform.position, m_TopTarget.transform.position) < 3) {
					sucked = true;
				}
			}

			if (isOpen) {
				if (masterPlayer) {
					RaycastHit hit;
					Vector3 startPoint = transform.position;
					startPoint.y -= 3f;
					Vector3 playerDirection = (masterPlayer.transform.position - startPoint).normalized;
					if (Physics.Raycast(startPoint, playerDirection, out hit)) {
						Debug.DrawLine(startPoint, hit.point, Color.cyan);
						if (hit.collider.CompareTag("Player")) {
							masterPlayer.suckerList.Add(this);
						}
					}
				}
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
			if (other.CompareTag("PickUpable")) {
				EnvironmentSettings.safeBoxCount--;
				return;
			}
			
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
				if (other.CompareTag("PickUpable")) {
					EnvironmentSettings.safeBoxCount++;
					return;
				}
				
				PlayerStats player = other.GetComponent<PlayerStats>();
				if (!other.transform.CompareTag("Player") || !player) return;
				print("ENTER");
				player.outOfArea = false;
				player.enteredSector = true;
			}
		}
		
		private void OnTriggerStay(Collider other) {
			if (other.CompareTag("PickUpable") && !isSafe) {
				EnvironmentSettings.safeBoxCount--;
				Destroy(other.gameObject);
				return;
			}
			
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
							m_TopTarget = target;
							if (!sucked) {

								//StartCoroutine(MovePieceTowards(other, GameObject.FindGameObjectWithTag("Suction").transform.position, 5f / 10));

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
