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
		public bool isSealed = false;
		public float destructionTime;
		public List<GameObject> deadEnds = new List<GameObject>();
		
		public bool DIEDIEDIE = false;
		public bool goingToDie = false;
		public bool dead = false;	
		[SerializeField] private GameObject m_SuckerCube;	
		[SerializeField] private GameObject m_Explosion;	
		private GameObject TmpExpl;	
		private GameObject ExplParent;	

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

				if (destructionTime <= 1.5f && !dead) {
					m_GameController.Play_Destruction(gameObject);
					TmpExpl = Instantiate(m_Explosion, gameObject.transform.position, gameObject.transform.rotation);
					TmpExpl.transform.SetParent(GameObject.FindGameObjectWithTag("ExplosionControl").transform);
				}
				
				if (destructionTime <= 0 && !dead) {
					m_GasStats.isOpen = true;
					GetComponent<Renderer>().enabled = false;
					GetComponent<MeshCollider>().enabled = false;
					foreach (MeshRenderer sideLight in m_LightList) {
						sideLight.gameObject.SetActive(false);
					}

					foreach (GameObject end in deadEnds) {
						end.SetActive(false);
					}
                    
					StartCoroutine(DestroySector());
					dead = true;
				}
			}
		}

		public void InitiateCountDown() {
			foreach (MeshRenderer cornerLight in m_LightList) {
				cornerLight.GetComponent<Renderer>().material = m_WarningMaterial;
			}

			if (GameObject.FindGameObjectWithTag("ExplosionControl").transform.childCount > 0) {
				for (var i = 0; i < GameObject.FindGameObjectWithTag("ExplosionControl").transform.childCount; i++) {
					Destroy(GameObject.FindGameObjectWithTag("ExplosionControl").transform.GetChild(i).transform.gameObject);
				}
			}
			
			goingToDie = true;
			destructionTime = Random.Range(5, 5); // TODO - CHANGE THIS TO A HIGH NUMBER (TIME IT TAKES FOR IT TO BE DESTROYED)
			m_GameController.Play_DestructionWarning();
		}

		public void InitWarning() {
			foreach (MeshRenderer cornerLight in m_LightList) {
				cornerLight.GetComponent<Renderer>().material = m_WarningMaterial;
			}
		}
			
		public IEnumerator DestroySector() {
			EnvironmentSettings.deadCount++;
			foreach (MeshRenderer cornerLight in m_LightList) {
				cornerLight.GetComponent<Renderer>().material = m_DeadMaterial;
			}

			isSafe = false;
			m_GasStats.isSafe = false;
			m_GasStats.isSealed = false;
			List<Sector> destroyList = new List<Sector>();
			foreach (SectorData sect in attachedSectors) {
				print("==================");
				if (!sect.attachedSector.isSafe) continue;
				// Sector - TRANSFORM
				RaycastHit hit;
				Vector3 startPoint = transform.position;
				startPoint.y += 4;
				Vector3 direction = (sect.attachedSector.transform.position - transform.position).normalized;
				print(transform.position);
				print(sect.attachedSector.transform.position);
				Debug.DrawLine(startPoint, sect.attachedSector.transform.position, Color.red);
				
				if (Physics.Raycast(startPoint, direction, out hit)) {
					print(hit.collider.tag);
					if (hit.collider.CompareTag("DoorSeal")) {
						print("Safe");
					} else {
						print("Nope");
						if (sect.attachedSector.isSafe) {
							sect.attachedSector.InitWarning();
							destroyList.Add(sect.attachedSector);
						}

					}
				} 
			}
			
			yield return new WaitForSeconds(4f);
			foreach (Sector section in destroyList) {
				StartCoroutine(section.DestroySector());
			}
		}

		public void AddSection(SectorData newSector) {
			print("Got another!");
			print(newSector);
			attachedSectors.Add(newSector);
		}
	}
}
