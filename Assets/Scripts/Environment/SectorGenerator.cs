using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Environment {
	public class SectorGenerator : MonoBehaviour {
		public int attachedIndex = -1;
		public float selfRotate;
		public List<Transform> points = new List<Transform>();
		
		[SerializeField] private List<SectorGenerator> m_SectorList = new List<SectorGenerator>();
		[SerializeField] private GameObject m_DeadEndWall;
		[SerializeField] private Transform m_ConnectionHolder;
		[SerializeField] private Transform m_CornerPoint;
		[SerializeField] private Transform m_EastSwap;
		[SerializeField] private Transform m_WestSwap;
		[SerializeField] private Transform m_NorthSwap;
		[SerializeField] private Transform m_SouthSwap;
		
		private Dictionary<string, float> m_RotateValues = new Dictionary<string, float>();
		private bool m_Setup;
		private int m_CreationAttempts = 0;
		private bool m_HasCast;
		private bool m_IsCorner;
		
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
			List<SectorGenerator> generatedSectors = new List<SectorGenerator>();
			foreach (Transform child in points) {
				count++;
				if (count == attachedIndex) continue;
				if (EnvironmentSettings.sectorCount < EnvironmentSettings.maxSectors) {
					m_CreationAttempts = 0;
					SectorGenerator newSect = GenerateSector(child);
					if (newSect != null) {
						generatedSectors.Add(newSect);
					}
				}
			}
			
			if (EnvironmentSettings.sectorCount < EnvironmentSettings.maxSectors) {
				StartCoroutine(NextGenStep(generatedSectors));
			}
		}

		public IEnumerator NextGenStep(List<SectorGenerator> generatedSectors) {
			foreach (SectorGenerator genSect in generatedSectors) {
				genSect.Generate();
			}
			yield return null;
		}
		
		public void GenerateEnds() {
			print("Generate Ends");
			int count = -1;
			foreach (Transform child in points) {
				count++;
				print(child.name + " - " + count);
				if (count == attachedIndex) continue;
				m_CreationAttempts = 0;
				GenerateSectorEnd(child);
			}
		}

		private void GenerateSectorEnd(Transform child) {
			if (m_IsCorner && child.name == "EastPoint") {
				child = m_CornerPoint;
			}
			Vector3 newPosition = transform.position + ((child.position - transform.position) * 2);
			Vector3 top = new Vector3(newPosition.x , newPosition.y + 0.2f, newPosition.z);
			print("Casting! " + child.name + " | " + top + " | Parent: " + transform.position);
			RaycastHit hit;
			if (Physics.Raycast(top, Vector3.down, out hit)) {
				print("Has hit!");
				if (hit.collider.transform.position != transform.position) {
					print("Not self");
					Vector3 testPosition = transform.position + ((child.position - transform.position) * 1.1f);
					testPosition.y += 0.2f;
					RaycastHit hitAgain;
					if (Physics.Raycast(testPosition, Vector3.down, out hitAgain)) {
						print("There is no gap");
						/*SectorData sectorData = new SectorData();
						sectorData.attachedSector = hit.collider.GetComponent<Sector>();
						sectorData.isSealed = false;
						sectorData.attachedSide = child.name;
						transform.GetComponent<Sector>().attachedSectors.Add(sectorData);

						SectorData parentSectorData = new SectorData();
						parentSectorData.attachedSector = transform.GetComponent<Sector>();
						parentSectorData.isSealed = false;
						parentSectorData.attachedSide = "Unknown";
						hit.collider.GetComponent<Sector>().AddSection(parentSectorData);*/
						m_HasCast = true;
					} else {
						print("There is a gap!");
						Debug.DrawLine(testPosition, testPosition + Vector3.down, Color.magenta, 100f);
						float rotation;
						if (child.name == "NorthPoint" || child.name == "SouthPoint") {
							rotation = 90;
						} else {
							rotation = 0;
						}

						rotation += selfRotate;
						
						GameObject deadEnd = Instantiate(m_DeadEndWall);
						//deadEnd.transform.position = child.position;
						deadEnd.transform.localRotation = Quaternion.Euler(90, rotation, 0);
						deadEnd.transform.position = transform.position + ((child.position - transform.position));
						transform.GetComponent<Sector>().deadEnds.Add(deadEnd);
						/*if (m_IsCorner && child.name == "EastPoint") {
							deadEnd.transform.SetParent(m_CornerPoint);
						} else {
							deadEnd.transform.SetParent(child);
						}*/
					}
				} else {
					print("Hitting Itself");
				}
			} else {
				print("Nothing is there!");
				Debug.DrawLine(top, top + Vector3.down, Color.green, 100f);
				float rotation;
				if (child.name == "NorthPoint" || child.name == "SouthPoint") {
					rotation = 90;
				} else {
					rotation = 0;
				}
						
				rotation += selfRotate;
				GameObject deadEnd = Instantiate(m_DeadEndWall);
				//deadEnd.transform.position = child.position;
				deadEnd.transform.localRotation = Quaternion.Euler(90, rotation, 0);
				deadEnd.transform.position = transform.position + ((child.position - transform.position));
				transform.GetComponent<Sector>().deadEnds.Add(deadEnd);
				/*if (m_IsCorner && child.name == "EastPoint") {
					deadEnd.transform.SetParent(m_CornerPoint);
				} else {
					deadEnd.transform.SetParent(child);
				}*/
			}
		}

		private SectorGenerator GenerateSector(Transform child) {
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
						if (Physics.Raycast(testPosition, Vector3.down, out hit)) {
							valid = false;
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
						} else {
							/*Debug.DrawLine(testPosition, testPosition + Vector3.down, Color.red, 100f);
							float rotation;
							if (child.name == "NorthPoint" || child.name == "SouthPoint") {
								rotation = 90;
							} else {
								rotation = 0;
							}

							rotation += selfRotate;
						
							GameObject deadEnd = Instantiate(m_DeadEndWall);
							//deadEnd.transform.position = child.position;
							deadEnd.transform.localRotation = Quaternion.Euler(90, rotation, 0);
							deadEnd.transform.position = transform.position + ((transform.position - child.position));
							if (m_IsCorner) {
								deadEnd.transform.SetParent(m_CornerPoint);
							} else {
								deadEnd.transform.SetParent(child);
							}*/
						}

						/*if (m_CreationAttempts < 5) {
							GenerateSector(child);
						}*/
					}
				} else {
					m_HasCast = false;
					SectorGenerator nextSector = Instantiate(m_SectorList[Random.Range(0, m_SectorList.Count - 1)]);
					if (nextSector.transform.name.Split(' ')[0] == "Corner") {
						nextSector.m_IsCorner = true;
					};
					
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

					EnvironmentSettings.sectorCount++;
					EnvironmentSettings.sectorList.Add(nextSector.GetComponent<Sector>());

					return nextSector;
					/*if (EnvironmentSettings.passCount < EnvironmentSettings.generationPasses) {
						nextSector.Generate();
					} else {
						print("Test");
						nextSector.GenerateEnds();
					}*/
				}

			return null;
		}
	}
}
