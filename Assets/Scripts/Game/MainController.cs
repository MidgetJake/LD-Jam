﻿using System.Collections.Generic;
using System.Security.Permissions;
using System.Timers;
using Environment;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
	public class MainController : MonoBehaviour {
		[SerializeField] private GameObject m_Player;
		
		private AudioSource m_AudioSource;
		private Controller m_PlayerController;
		
		public AudioClip warningSound;
		public List<AudioClip> destructionSound = new List<AudioClip>();
		public float DeathClock = 100;
		public float DeathTick;
		public Sector DeadSector;
		
		
		[SerializeField] private GameObject DeadScreen;
		[SerializeField] private GameObject BlockSaved;
		[SerializeField] private GameObject TimerText;

		private void Start() {
			m_AudioSource = gameObject.GetComponent<AudioSource>();
			m_PlayerController = m_Player.GetComponent<Controller>();
			Cursor.visible = false;
			Cursor.visible = false;
		}

		public void Update() {
			DeathTick += Time.deltaTime;
			if (DeathTick > DeathClock) {
				DeathTick = 0;
				DeathClock = Mathf.Clamp(DeathClock * 0.75f, 20, 100);

				if (EnvironmentSettings.deadCount == EnvironmentSettings.sectorList.Count) {
					EnvironmentSettings.ActiveGame = false;
					DeadScreen.active = true;
					BlockSaved.transform.GetComponent<Text>().text = ""+EnvironmentSettings.safeBoxCount + " Boxes.";
					TimerText.GetComponent<Text>().text = ""+EnvironmentSettings.OveralTimer + " seconds.";
					m_Player.GetComponent<Controller>().m_CursorIsLocked = false;
					Cursor.visible = true;
				} else {
					SelectSector();
					EnvironmentSettings.BreakSector(DeadSector);
					DeadSector.InitiateCountDown();
				}
			}

			if (EnvironmentSettings.ActiveGame) {
				EnvironmentSettings.OveralTimer += Time.deltaTime;	
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
