using System.Collections.Generic;
using System.Security.Permissions;
using Environment;
using Player;
using UnityEngine;

namespace Game {
	public class MainController : MonoBehaviour {
		[SerializeField] private GameObject m_Player;
		
		private AudioSource m_AudioSource;
		private Controller m_PlayerController;
		
		public AudioClip warningSound;
		public List<AudioClip> destructionSound = new List<AudioClip>();
		public float DeathClock = 10;
		public float DeathTick;
		public Sector DeadSector;

		private void Start() {
			m_AudioSource = gameObject.GetComponent<AudioSource>();
			m_PlayerController = m_Player.GetComponent<Controller>();
		}

		public void Update() {
			DeathTick += Time.deltaTime;
			if (DeathTick > DeathClock) {
				DeathTick = 0;

				SelectSector();
				EnvironmentSettings.BreakSector(DeadSector);
				DeadSector.InitiateCountDown();
			}
		}

		private void SelectSector() {
			DeadSector = EnvironmentSettings.sectorList[Random.Range(0, EnvironmentSettings.sectorList.Count)];
			if (DeadSector.dead) {
				SelectSector();
			}
		}

		public void Play_DestructionWarning() {
			m_AudioSource.clip = warningSound;
			m_AudioSource.Play();
		}
		
		public void Play_Destruction(GameObject obj) {
			m_AudioSource.clip = destructionSound[Random.Range(0, destructionSound.Count)];
			m_PlayerController.deadSectorPlayerDistance = Vector3.Distance(m_Player.transform.position, obj.transform.position);
			m_PlayerController.m_CameraShake = true;
			m_AudioSource.Play();
		}
	}
}
