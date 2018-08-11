using UnityEngine;

namespace Environment {
	public class Sector : MonoBehaviour {
		[SerializeField] private SectorDoor m_SectorDoorNorth;
		[SerializeField] private SectorDoor m_SectorDoorEast;
		[SerializeField] private SectorDoor m_SectorDoorSouth;
		[SerializeField] private SectorDoor m_SectorDoorWest;
		[SerializeField] private EnvironmentControl m_SectorEnvironment;

		public void BreakSector() {
			m_SectorEnvironment.isSafe = false;
		}
	}
}
