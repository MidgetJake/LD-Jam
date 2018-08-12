using System.Collections;
using UnityEngine;

namespace Environment {
	public class Sucker : MonoBehaviour {

		public bool isActive;
		public bool sucked;

		public GameObject player;
		public GameObject target;

		private void Update() {
			if (isActive) {
				
				if (Vector3.Distance(player.transform.position, gameObject.transform.position) > 5) {
					isActive = true;
				}
				
				if (Vector3.Distance(player.transform.position, gameObject.transform.position) < 2) {
					isActive = false;
					sucked = true;
					// Find next suction cube with the lowest number 
				}
				
				float test = Vector3.Distance(player.transform.position, gameObject.transform.position);
				StartCoroutine(MovePieceTowards(player, gameObject.transform.position, test / 10));
			}
		}
		
		private IEnumerator MovePieceTowards(GameObject piece, Vector3 end, float speed) {
			while (piece.transform.position != end && !sucked) {
				piece.transform.position = Vector3.MoveTowards(piece.transform.position, end, speed * Time.deltaTime);
				yield return null;
			}
		}
	}
}
