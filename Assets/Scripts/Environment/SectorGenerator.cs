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
		private bool m_HasCast;
		
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
//			Vector3 newPosition = transform.position + ((transform.position - child.position) * 1.1f);
			/*foreach (Vector3 point in EnvironmentSettings.sectorPoints) {
				if (
					point.x - newPosition.x <= 0.01f && point.x - newPosition.x >= -0.01f &&
					point.y - newPosition.y <= 0.01f && point.y - newPosition.y >= -0.01f &&
					point.z - newPosition.z <= 0.01f && point.z - newPosition.z >= -0.01f
				) {*/

			//if (!m_HasCast) {
				Vector3 top = new Vector3(newPosition.x , newPosition.y + 0.5f, newPosition.z);
				RaycastHit hit;
				if (Physics.Raycast(top, Vector3.down, out hit)) {
					if (hit.collider.transform.position != transform.position) {
						Vector3 testPosition = transform.position + ((transform.position - child.position) * 1.1f);
						testPosition.y += 0.5f;
						Debug.DrawLine(testPosition, testPosition + Vector3.down, Color.red, 100f);
						if (Physics.Raycast(testPosition, Vector3.down, out hit)) {
							valid = false;
							print("One here already!");
							SectorData sectorData = new SectorData();
							sectorData.attachedSector = hit.collider.GetComponent<Sector>();
							sectorData.isSealed = false;
							sectorData.attachedSide = child.name;
							transform.GetComponent<Sector>().attachedSectors.Add(sectorData);

							SectorData parentSectorData = new SectorData();
							parentSectorData.attachedSector = transform.GetComponent<Sector>();
							parentSectorData.isSealed = false;
							parentSectorData.attachedSide = "Unknown";
							hit.collider.GetComponent<Sector>().AddSection(parentSectorData);
							m_HasCast = true;
						}

						/*if (m_CreationAttempts < 5) {
							GenerateSector(child);
						}*/
					}
				} else {
					m_HasCast = false;
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
					//nextSector.transform.SetParent(EnvironmentSettings.topParent);
					EnvironmentSettings.sectorPoints.Add(nextSector.transform.position);
				
					//nextSector.transform.SetParent(null);
				
					SectorData parentSectorData = new SectorData();
					parentSectorData.attachedSector = transform.GetComponent<Sector>();
					parentSectorData.isSealed = false;
					parentSectorData.attachedSide = attachedPoint.name;
					nextSector.GetComponent<Sector>().attachedSectors.Add(parentSectorData);
				
					SectorData sectorData = new SectorData();
					sectorData.attachedSector = nextSector.GetComponent<Sector>();
					sectorData.isSealed = false;
					sectorData.attachedSide = child.name;
					transform.GetComponent<Sector>().attachedSectors.Add(sectorData);
				
					EnvironmentSettings.sectorList.Add(nextSector.GetComponent<Sector>());
				
					if (EnvironmentSettings.passCount < EnvironmentSettings.generationPasses) {
						nextSector.Generate();
					}
				}
			//}

					
				//}
			//}
			
			
			if (valid) {
				/*m_HasCast = false;
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
				//nextSector.transform.SetParent(EnvironmentSettings.topParent);
				EnvironmentSettings.sectorPoints.Add(nextSector.transform.position);
				
				//nextSector.transform.SetParent(null);
				
				SectorData parentSectorData = new SectorData();
				parentSectorData.attachedSector = transform.GetComponent<Sector>();
				parentSectorData.isSealed = false;
				parentSectorData.attachedSide = attachedPoint.name;
				nextSector.GetComponent<Sector>().attachedSectors.Add(parentSectorData);
				
				SectorData sectorData = new SectorData();
				sectorData.attachedSector = nextSector.GetComponent<Sector>();
				sectorData.isSealed = false;
				sectorData.attachedSide = child.name;
				transform.GetComponent<Sector>().attachedSectors.Add(sectorData);
				
				EnvironmentSettings.sectorList.Add(nextSector.GetComponent<Sector>());
				
				if (EnvironmentSettings.passCount < EnvironmentSettings.generationPasses) {
					nextSector.Generate();
				}*/
			}
		}
	}
}
