using UnityEngine;

namespace Environment {
	public class BoxSpawner : MonoBehaviour {
		[SerializeField] private GameObject m_Box;
		
		// Use this for initialization
		void Start () {
			if(Random.Range(0, 100) > 49) {
				Instantiate(m_Box, transform.position, transform.rotation);
				EnvironmentSettings.boxCount++;
//				EnvironmentSettings.safeBoxCount++;
			}
		}
	}
}
