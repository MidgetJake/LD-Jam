using Game;
using UnityEngine;

namespace Environment {
	public class Sector : MonoBehaviour {
		[SerializeField] private MainController m_GameController;
		
		public float m_DestructionTime;
		
		public bool DIEDIEDIE = false;
		public bool goingToDie = false;

		private void Update() {
			if (DIEDIEDIE) {
				DIEDIEDIE = false;
				InitiateCountDown();
			}
			
			if (goingToDie) {
				m_DestructionTime -= Time.deltaTime;

				if (m_DestructionTime <= 1.5f) {
					m_GameController.Play_Destruction(gameObject);
				}
				
				if (m_DestructionTime <= 0) {
					Destroy(gameObject);
				}
			}
		}

		public void InitiateCountDown() {
			goingToDie = true;
			m_DestructionTime = Random.Range(15, 15);
			m_GameController.Play_DestructionWarning();
		}
	}
}
