using UnityEngine;

namespace Effects {
	public class EnergyBeam : MonoBehaviour {
		public Transform playerPosition;
		public bool active;
		[SerializeField] private LineRenderer m_LineRenderer;

		private void Start() {
			m_LineRenderer.enabled = false;
		}
		
		// Update is called once per frame
		private void Update () {
			if (active) {
				m_LineRenderer.enabled = true;
				Vector3 pos = new Vector3(playerPosition.position.x, playerPosition.position.y - 1, playerPosition.position.z);
				m_LineRenderer.SetPosition(0, pos);
				m_LineRenderer.SetPosition(1, transform.position);
			} else {
				m_LineRenderer.enabled = false;
			}
		}
	}
}
