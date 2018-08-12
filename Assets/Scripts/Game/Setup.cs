using System.Collections.Generic;
using Environment;
using UnityEngine;

namespace Game {
	public class Setup : MonoBehaviour {
		public int maxPasses = 5;
		public int maxSectors = 30;
		public bool generate;
		[SerializeField] private SectorGenerator m_InitialSector;
		[SerializeField] private Transform m_TopParent;

		private void Start () {
			print("Start Generation");
			EnvironmentSettings.topParent = m_TopParent;
			EnvironmentSettings.generationPasses = maxPasses;
			EnvironmentSettings.initialSector = m_InitialSector;
			EnvironmentSettings.maxSectors = maxSectors;
			EnvironmentSettings.sectorCount = 0;
			EnvironmentSettings.Generate();
			print(EnvironmentSettings.sectorList.Count);
			foreach (Sector sect in EnvironmentSettings.sectorList) {
				sect.GetComponent<SectorGenerator>().GenerateEnds();
			}
			if (EnvironmentSettings.sectorList.Count < 15) {
				ReGenerate();
			}
		}

		private void Update() {
			if (generate) {
				ReGenerate();
			}
		}

		private void ReGenerate() {
			print("Try");
			foreach (Transform child in EnvironmentSettings.topParent) {
				Destroy(child.gameObject);
			}
				
			generate = false;
			EnvironmentSettings.maxSectors = maxSectors;
			EnvironmentSettings.sectorCount = 0;
			EnvironmentSettings.sectorList = new List<Sector>();
			EnvironmentSettings.sectorPoints = new List<Vector3>();
			EnvironmentSettings.generationPasses = maxPasses;
			EnvironmentSettings.passCount = 0;
			EnvironmentSettings.initialSector = m_InitialSector;
			EnvironmentSettings.Generate();
			if (EnvironmentSettings.sectorList.Count < 15) {
				ReGenerate();
			}
		}
	}
}
