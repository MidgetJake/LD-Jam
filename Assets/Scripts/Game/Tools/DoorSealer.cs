using Environment;
using Player;
using UnityEngine;
using System.Collections;

namespace Game.Tools {
	public class DoorSealer : Tool {
		private SectorDoor m_SectorActive;
		public GameObject door;
		[SerializeField] private Sector m_Sector;
		
		public override void UseItem() {
			if (canUse) {
				m_SectorActive.SealDoor();
				print("Used!");
			} else {
				print("Can't use!");
			}
		}
		
		public void ToggleDoor(bool isClosed) {
			if (!m_Sector.isSafe) return;
			door.SetActive(isClosed);
			if (!isClosed) {
				RaycastHit hit;
				Vector3 selfPos = new Vector3(transform.position.x, m_Sector.transform.position.y, transform.position.y);
				Vector3 dir = (selfPos - m_Sector.transform.position).normalized;
				if (Physics.Raycast(m_Sector.transform.position, dir, out hit)) {
					Sector sect = hit.collider.GetComponent<Sector>();
					if (!sect.isSafe) {
						m_Sector.InitWarning();
						StartCoroutine(DestroySect());
					}
				} else {
					m_Sector.InitWarning();
					StartCoroutine(DestroySect());
				}
			}
		}

		private IEnumerator DestroySect() {
			yield return new WaitForSeconds(4f);
			StartCoroutine(m_Sector.DestroySector());
		}

		public override void SetUsable(bool usable, string type, GameObject door) {
			if (type == "Seal") {
				canUse = usable;
				m_SectorActive = usable ? door.GetComponent<SectorDoor>() : null;
			}
		}

		private void OnTriggerEnter(Collider other) {
			if (other.CompareTag("Player")) {
				other.GetComponent<Controller>().currDoor = this;
			}
		}
		
		private void OnTriggerStay(Collider other) {
			if (other.CompareTag("Player")) {
				other.GetComponent<Controller>().currDoor = this;
			}
		}
		
		private void OnTriggerExit(Collider other) {
			if (other.CompareTag("Player")) {
				other.GetComponent<Controller>().currDoor = null;
			}
		}
	}
}
