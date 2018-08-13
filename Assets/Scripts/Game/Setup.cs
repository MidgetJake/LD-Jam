using System.Collections.Generic;
using System.Collections;
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
			ReGenerate();
		}
		
		public void ReGenerate() {
			generate = false;
			EnvironmentSettings.maxSectors = maxSectors;
			EnvironmentSettings.sectorCount = 0;
			EnvironmentSettings.sectorList = new List<Sector>();
			EnvironmentSettings.sectorPoints = new List<Vector3>();
			EnvironmentSettings.generationPasses = maxPasses;
			EnvironmentSettings.passCount = 0;
			EnvironmentSettings.initialSector = m_InitialSector;
			EnvironmentSettings.Generate();
			foreach (Sector sect in EnvironmentSettings.sectorList) {
				sect.GetComponent<SectorGenerator>().GenerateEnds();
			}
		}
	}
}
