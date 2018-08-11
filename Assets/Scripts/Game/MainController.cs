using System.Collections.Generic;
using System.Security.Permissions;
using Player;
using UnityEngine;

namespace Game {
	public class MainController : MonoBehaviour {

		public AudioClip warningSound;
		public List<AudioClip> destructionSound = new List<AudioClip>();

		private AudioSource m_AudioSource;
		[SerializeField] private GameObject m_Player;
		private Controller m_PlayerController;

		private void Start() {
			m_AudioSource = gameObject.GetComponent<AudioSource>();
			m_PlayerController = m_Player.GetComponent<Controller>();
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
