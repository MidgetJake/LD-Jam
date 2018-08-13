using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game {
	public class SceneChanger : MonoBehaviour {
		[SerializeField] private Setup m_Setup;
		[SerializeField] private Animator m_Animator;
		[SerializeField] private GameObject m_MenuCamera;
		[SerializeField] private GameObject m_MenuUI;
		
		public void StartLevel() {
			m_Animator.SetTrigger("isLoading");
			StartCoroutine(LoadLevel());
		}
		
		private IEnumerator LoadLevel() {
			yield return new WaitForSeconds(0.3f);
			print("Init level load");
			AsyncOperation sceneToLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
			sceneToLoad.allowSceneActivation = false;
			print("Loading!");
			while (!sceneToLoad.isDone) {
				print(sceneToLoad.progress);
				if (sceneToLoad.isDone || sceneToLoad.progress >= 0.9f) {
					print("Loaded!");
					sceneToLoad.allowSceneActivation = true;
					yield return new WaitForSeconds(1f);
				}

				yield return null;
			}
			
			print("Done");
			
			//m_Setup.ReGenerate();
			m_MenuCamera.SetActive(false);
			m_MenuUI.SetActive(false);
			GameObject.FindGameObjectWithTag("Player").SetActive(true);
			m_Animator.SetTrigger("doneLoading");
		}

		private IEnumerator LevelTransition(Action transitionJob) {
			m_Animator.SetTrigger("isLoading");
			yield return new WaitForSeconds(0.15f);
			transitionJob();
			m_Animator.SetTrigger("doneLoading");
			yield return new WaitForSeconds(0.15f);
		}
	}
}
