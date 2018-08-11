using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Environment {
	public class SectorGenerator : MonoBehaviour {
		public int attachedIndex = -1;
		public float selfRotate;
		public List<Transform> points = new List<Transform>();
		
		[SerializeField] private List<SectorGenerator> m_SectorList = new List<SectorGenerator>();
		[SerializeField] private Transform m_ConnectionHolder;
		
		private Dictionary<string, float> m_RotateValues = new Dictionary<string, float>();
		private bool m_Setup;
		private int m_CreationAttempts = 0;
		
		public void Generate() {
			if (!m_Setup) {
				m_RotateValues.Add("NorthPoint", 0);
				m_RotateValues.Add("EastPoint", 90);
				m_RotateValues.Add("SouthPoint", 180);
				m_RotateValues.Add("WestPoint", 270);
				m_Setup = true;
			}

			EnvironmentSettings.passCount++;
			int count = -1;
			foreach (Transform child in points) {
				count++;
				if (count == attachedIndex) continue;
				m_CreationAttempts = 0;
				GenerateSector(child);
			}
		}

		private void GenerateSector(Transform child) {
			m_CreationAttempts++;
			bool valid = true;
			Vector3 newPosition = transform.position + ((transform.position - child.position) * 2);
			foreach (Vector3 point in EnvironmentSettings.sectorPoints) {
				if (
					point.x - newPosition.x <= 0.01f && point.x - newPosition.x >= -0.01f &&
					point.y - newPosition.y <= 0.01f && point.y - newPosition.y >= -0.01f &&
					point.z - newPosition.z <= 0.01f && point.z - newPosition.z >= -0.01f
				) {
					valid = false;
					if (m_CreationAttempts < 5) {
						GenerateSector(child);
					}
				}
			}
			
			
			if(valid) {
				SectorGenerator nextSector = Instantiate(m_SectorList[Random.Range(0, m_SectorList.Count - 1)]);
					
				nextSector.attachedIndex = Random.Range(0, nextSector.points.Count - 1);
				Transform attachedPoint = nextSector.points[nextSector.attachedIndex];
				float baseRotate = m_RotateValues[child.name] - m_RotateValues[attachedPoint.name] + selfRotate;
				nextSector.transform.rotation = Quaternion.Euler(-90, 0, baseRotate);
				nextSector.selfRotate = baseRotate;
					
				//nextSector.transform.SetParent(transform);
				nextSector.transform.position = newPosition;
				nextSector.transform.position = new Vector3(nextSector.transform.position.x, 0, nextSector.transform.position.z);

				m_CreationAttempts = 0;
				nextSector.transform.SetParent(EnvironmentSettings.topParent);
				EnvironmentSettings.sectorPoints.Add(nextSector.transform.position);
				print(" - - - - ");
				print(transform.position + ((transform.position - child.position) * 2));
				print(" - - - - ");
				
				//nextSector.transform.SetParent(null);
				
				EnvironmentSettings.sectorList.Add(nextSector.GetComponent<Sector>());
				if (EnvironmentSettings.passCount < EnvironmentSettings.generationPasses) {
					nextSector.Generate();
				}
			}
		}
	}
}
