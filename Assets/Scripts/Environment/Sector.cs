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
		
		[SerializeField] private GameObject m_Gas;
		[SerializeField] private Material m_WarningMaterial;
		[SerializeField] private Material m_SafeMaterial;
		[SerializeField] private Material m_DeadMaterial;
		[SerializeField] private List<MeshRenderer> m_LightList = new List<MeshRenderer>();
		private EnvironmentControl m_GasStats;
		private MainController m_GameController;
		
		public List<SectorData> attachedSectors = new List<SectorData>();
		public bool isSafe = true;
		public int suctionPathNum;
		public float destructionTime;
		public List<GameObject> deadEnds = new List<GameObject>();
		
		public bool DIEDIEDIE = false;
		public bool goingToDie = false;
		public bool dead = false;	

		private void Start() {
			m_GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainController>();
			m_GasStats = m_Gas.GetComponent<EnvironmentControl>();
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
					StartCoroutine(DestroySector(0));
					dead = true;
				}
			}
		}

		public void InitiateCountDown() {
			foreach (MeshRenderer cornerLight in m_LightList) {
				cornerLight.GetComponent<Renderer>().material = m_WarningMaterial;
			}
			
			goingToDie = true;
			destructionTime = Random.Range(15, 15);
			m_GameController.Play_DestructionWarning();
		}

		public void InitWarning() {
			foreach (MeshRenderer cornerLight in m_LightList) {
				cornerLight.GetComponent<Renderer>().material = m_WarningMaterial;
			}
		}

		public IEnumerator DestroySector(int pathNumber) {
			foreach (MeshRenderer cornerLight in m_LightList) {
				cornerLight.GetComponent<Renderer>().material = m_DeadMaterial;
			}
			suctionPathNum = pathNumber + 1;
			isSafe = false;
			m_GasStats.isSafe = false;
			foreach (SectorData sect in attachedSectors) {
				if (!sect.isSealed && sect.attachedSector.isSafe) {
					sect.attachedSector.InitWarning();
				}
			}
			yield return new WaitForSeconds(2.5f);
			foreach (SectorData sect in attachedSectors) {
				if (!sect.isSealed && sect.attachedSector.isSafe) {
					StartCoroutine(sect.attachedSector.DestroySector(suctionPathNum));
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
