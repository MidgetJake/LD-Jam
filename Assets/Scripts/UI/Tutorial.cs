using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {
	[SerializeField] private GameObject m_Next;
	[SerializeField] private GameObject m_MainMenu;
	[SerializeField] private GameObject m_Tutorial;

	public void Next() {
		m_Next.SetActive(true);
		gameObject.SetActive(false);
	}

	public void Finish() {
		m_MainMenu.SetActive(true);
		m_Tutorial.SetActive(false);
	}
}
