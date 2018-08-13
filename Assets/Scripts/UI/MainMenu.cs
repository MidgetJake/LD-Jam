using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
	[SerializeField] private GameObject m_Tutorial;
	[SerializeField] private GameObject m_TutorialFirst;
	[SerializeField] private GameObject m_MainMenu;

	private void Start() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void OpenTutorial() {
		m_Tutorial.SetActive(true);
		foreach (Transform child in m_Tutorial.transform) {
			child.gameObject.SetActive(false);
		}
		m_TutorialFirst.SetActive(true);
		m_MainMenu.SetActive(false);
	}

	public void Quit() {
		Application.Quit();
	}
}
