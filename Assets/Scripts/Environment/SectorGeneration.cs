using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment {
	public class SectorGeneration : MonoBehaviour {

		[SerializeField] private List<GameObject> m_Sectors = new List<GameObject>();
		
		private GameObject m_CurrentSector;
		private GameObject m_PrevSector;
		private Transform m_CurrentConnector;
		private GameObject m_CurrentOption;
		
		

		public int m_MaxNumChunks;
		private int m_ChunkNum = 0;
		private bool m_InitialSpawn = true;
		
		private List<GameObject> nextOptions = new List<GameObject>();
		
	
		// Use this for initialization
		void Start () {
			//m_MaxNumChunks = Random.Range(4, 7);
//			m_MaxNumChunks = 1;
			m_CurrentSector = gameObject;
			LoopThroughOptions(gameObject);
		}
	
		// Update is called once per frame
		void Update () {
			if (m_InitialSpawn) {
				//SpawnLevels();
			}
		}

		private void LoopThroughOptions(GameObject obj) {
			foreach(Transform child in obj.transform) {
				if (child.CompareTag("ConnectorOption")) {
					print("Connector Name: " + child.name);
					child.tag = "Connector";
					SpawnSector(child, obj.transform);
				}
			}
		}

		private void SpawnSector(Transform currSector, Transform prevSector) {
			GameObject newSector = Instantiate(m_Sectors[Random.Range(0, m_Sectors.Count)]);
			int childPick = Random.Range(0, newSector.transform.childCount);
				
			Transform selectedConnector = newSector.transform.GetChild(childPick);
			selectedConnector.tag = "AttachConnector";
			print("Previous: " + (prevSector.localEulerAngles.z - currSector.localEulerAngles.z));
			print("Now: " + ((prevSector.localEulerAngles.z - currSector.localEulerAngles.z) - selectedConnector.localEulerAngles.z));
			print("-----");
			float rotation = -360 + (prevSector.localEulerAngles.z - currSector.localEulerAngles.z) - selectedConnector.localEulerAngles.z;
//			float rotation = -360 + currSector.localEulerAngles.z - selectedConnector.eulerAngles.z;
			print(prevSector.name + " --- " + rotation);
//			newSector.transform.rotation = Quaternion.Euler(-90, 0, ((prevSector.eulerAngles.z - currSector.eulerAngles.z) + (newSector.transform.eulerAngles.z - selectedConnector.eulerAngles.z)));
			newSector.transform.rotation = Quaternion.Euler(-90, 0, (rotation + 180));
			Vector3 connectorDist = currSector.position - selectedConnector.position;
			connectorDist.y = 0;
			newSector.transform.position = connectorDist;
			if (m_ChunkNum < m_MaxNumChunks) {
				print("Another Itteration");
				m_ChunkNum++;
				LoopThroughOptions(newSector);
			}
		}
		
		private void SpawnLevels() {
			for (int index = 0; index < m_MaxNumChunks; index++) {
				m_PrevSector = m_CurrentSector;
				
				if (nextOptions.Count == 0) {
					// Initial load
					m_CurrentSector = gameObject;
				} else {
					m_CurrentSector = Instantiate(m_Sectors[Random.Range(0, m_Sectors.Count)], m_CurrentSector.transform.position, Quaternion.Euler(-90, 0, 0));
					int childPick = Random.Range(0, m_CurrentSector.transform.childCount);
				
					Transform selectedConnector = m_CurrentSector.transform.GetChild(childPick);
					selectedConnector.tag = "Connector";
					m_CurrentSector.transform.rotation = Quaternion.Euler(-90, 0, m_CurrentConnector.eulerAngles.z - 180);
					Vector3 connectorDist = m_CurrentConnector.position - (/*m_CurrentSector.transform.position +*/ selectedConnector.position);
					connectorDist.y = 0;
					print(connectorDist);
					m_CurrentSector.transform.position = connectorDist;
				}
				
//				LoopThroughOptions(m_PrevSector, true); // Grabs nextOptions
//				LoopThroughOptions(m_CurrentSector, false); // Grabs current connector
				
				m_CurrentOption = nextOptions[Random.Range(0, nextOptions.Count)]; // Current connector from option
				
				float qz = 0;

				qz = m_CurrentConnector.localEulerAngles.z - m_CurrentOption.transform.localEulerAngles.z;
					
				print(qz);
					
				//m_CurrentSector.transform.rotation = Quaternion.Euler(-90, 0, qz);

				float x = (m_CurrentSector.transform.position.x - m_CurrentConnector.position.x) - (m_PrevSector.transform.position.x - m_CurrentOption.transform.position.x);
				float z = (m_CurrentSector.transform.position.z - m_CurrentConnector.position.z) - (m_PrevSector.transform.position.z - m_CurrentOption.transform.position.z);


				//m_CurrentSector.transform.position = connectorDist; //new Vector3(x, 0, z);




				/*m_PrevSector = m_CurrentSector;
				
				if (m_Connectors.Count >= 1) {
					m_PrevConnectors = m_Connectors;
					m_PrevConnectorsOptions = m_ConnectorsOptions;
					
					m_CurrentSector = Instantiate(m_Sectors[Random.Range(0, m_Sectors.Count)], m_CurrentSector.transform.position, Quaternion.Euler(-90, 0, 0));

					Transform selectedCurrPos = null;
					
					for (int i = 0; i < m_CurrentSector.transform.childCount; i++) {
						Transform child = m_CurrentSector.transform.GetChild(i);
						if (child.CompareTag("Connector")) {
							selectedCurrPos = child;
						} else if (child.CompareTag("ConnectorOption")) {
							m_ConnectorsOptions.Add(child.gameObject);
						}
					}

					GameObject selectedPrevPos = m_PrevConnectorsOptions[Random.Range(0, m_PrevConnectorsOptions.Count)];
					
				
					print(selectedPrevPos.transform.localEulerAngles.z);
					
					float qz = 0;

					if (selectedPrevPos.transform.localEulerAngles.z == -90) {
						qz = 90;
					}else if (selectedPrevPos.transform.localEulerAngles.z == 90) {
						qz = -90f;
					} else if (selectedPrevPos.transform.localEulerAngles.z == 270) {
						qz = -270f;
					} else {
						qz = 270f;
					}
					
					print(qz);
					
					m_CurrentSector.transform.rotation = Quaternion.Euler(-90, 0, qz);

					if (selectedCurrPos != null) {
						float x = selectedPrevPos.transform.position.x - selectedCurrPos.transform.position.x;
						float z = selectedPrevPos.transform.position.z - selectedCurrPos.transform.position.z;

					
						m_CurrentSector.transform.position = new Vector3(m_CurrentSector.transform.position.x + x, 0, m_CurrentSector.transform.position.z + z);
					}

					//Quaternion.Euler(-90, 0, 0)
					
					
					


				} else {
					// initial
					m_CurrentSector = Instantiate(m_Sectors[Random.Range(0, m_Sectors.Count)], gameObject.transform.position, Quaternion.Euler(-90, 0, 0));
					for (int i = 0; i < m_CurrentSector.transform.childCount; i++) {
						Transform child = m_CurrentSector.transform.GetChild(i);
						if (child.CompareTag("Connector")) {
							
						} else if (child.CompareTag("ConnectorOption")) {
							m_ConnectorsOptions.Add(child.gameObject);
						}
					}
				}*/
			}

			m_InitialSpawn = false;
		} 
	}
}
