using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class UpdateStats : MonoBehaviour {
		[SerializeField] private PlayerStats m_PlayerStats;
		[SerializeField] private Image m_HealthBar;
		[SerializeField] private Image m_OxygenBar;
		[SerializeField] private Image m_BlackOut;

		private float m_InternalClock = 1f;
		private float m_Opacity = 0;

		private void Update() {
			if (m_PlayerStats.playerOxygen < 100f) {
				if (m_InternalClock <= 0.0f) {
					m_PlayerStats.Regen();
					m_InternalClock = 1f;
				} else {
					m_InternalClock -= Time.deltaTime;
				}
			}

			m_BlackOut.color = new Color(0, 0, 0, m_Opacity);
			m_Opacity = 1 - (m_PlayerStats.playerHealth / 100);

			m_OxygenBar.fillAmount = m_PlayerStats.playerOxygen / 100;
			m_HealthBar.fillAmount = m_PlayerStats.playerHealth / 100;
		}

	}
}
