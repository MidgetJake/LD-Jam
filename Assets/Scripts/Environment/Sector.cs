using System.Collections;
using Game;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;

namespace Environment {

	public struct SectorData {
		public Sector attachedSector;
		public string attachedSide;
		public bool isSealed;
	}
	
	public class Sector : MonoBehaviour {
		
		[SerializeField] private GameObject Gas;
		private EnvironmentControl GasStats;
	
		private MainController m_GameController;
		
		public List<SectorData> attachedSectors = new List<SectorData>();
		public bool isSafe = true;
		public float destructionTime;
		
		public bool DIEDIEDIE = false;
		public bool goingToDie = false;
		public bool dead = false;	
		[SerializeField] private GameObject SuckerCube;	

		private void Start() {
			m_GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainController>();
			GasStats = Gas.GetComponent<EnvironmentControl>();
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
				
				if (destructionTime <= 0 && !dead) {
					StartCoroutine(DestroySector());
					dead = true;
				}
			}
		}

		public void InitiateCountDown() {
			goingToDie = true;
			destructionTime = Random.Range(5, 5);
			m_GameController.Play_DestructionWarning();
		}

		public IEnumerator DestroySector() {
			isSafe = false;
			GasStats.isSafe = false;
			GasStats.isSealed = false;
			yield return new WaitForSeconds(1f);
			foreach (SectorData sect in attachedSectors) {
				if (!sect.isSealed && sect.attachedSector.isSafe) {
					StartCoroutine(sect.attachedSector.DestroySector());
				}
			}

			gameObject.active = false;
			Instantiate(SuckerCube, gameObject.transform.position, gameObject.transform.rotation);
		}

		public void AddSection(SectorData newSector) {
			print("Got another!");
			print(newSector);
			attachedSectors.Add(newSector);
		}
	}
}
