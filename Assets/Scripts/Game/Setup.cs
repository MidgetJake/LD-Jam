using System.Collections.Generic;
using Environment;
using UnityEngine;

namespace Game {
	public class Setup : MonoBehaviour {
		public int maxPasses = 5;
		public bool generate;
		[SerializeField] private SectorGenerator m_InitialSector;
		[SerializeField] private Transform m_TopParent;

		private void Start () {
			EnvironmentSettings.topParent = m_TopParent;
			EnvironmentSettings.generationPasses = maxPasses;
			EnvironmentSettings.initialSector = m_InitialSector;
			EnvironmentSettings.Generate();
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
