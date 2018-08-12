using System.Collections;
using Game;
using UnityEngine;
using System.Collections.Generic;

namespace Environment {

	public struct SectorData {
		public Sector attachedSector;
		public string attachedSide;
		public bool isSealed;
	}
	
	public class Sector : MonoBehaviour {
		[SerializeField] private GameObject m_GasArea;
		private MainController m_GameController;
		
		public List<SectorData> attachedSectors = new List<SectorData>();
		public bool isSafe = true;
		public float destructionTime;
		
		public bool DIEDIEDIE = false;
		public bool goingToDie = false;

		private void Start() {
			m_GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainController>();
		}

		private void Update() {
			if (DIEDIEDIE) {
				DIEDIEDIE = false;
				InitiateCountDown();
			}
			
			if (goingToDie) {
				destructionTime -= Time.deltaTime;

				if (destructionTime <= 1.5f) {
					m_GameController.Play_Destruction(gameObject);
				}
				
				if (destructionTime <= 0) {
					StartCoroutine(DestroySector());
				}
			}
		}

		public void InitiateCountDown() {
			goingToDie = true;
			destructionTime = Random.Range(15, 15);
			m_GameController.Play_DestructionWarning();
		}

		public IEnumerator DestroySector() {
			isSafe = false;
			yield return new WaitForSeconds(1f);
			foreach (SectorData sect in attachedSectors) {
				if (!sect.isSealed && sect.attachedSector.isSafe) {
					StartCoroutine(sect.attachedSector.DestroySector());
				}
			}

			//gameObject.SetActive(false); // TODO - FIND A WAY TO KILL SECTOR
		}

		public void AddSection(SectorData newSector) {
			print("Got another!");
			print(newSector);
			attachedSectors.Add(newSector);
		}
	}
}
