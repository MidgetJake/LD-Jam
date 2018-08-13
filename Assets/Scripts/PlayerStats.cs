using System;
using UnityEngine;
using Player;
using System.Collections.Generic;
using Environment;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {
	public float maxHealth = 100f;
	public float maxOxygen = 100f;
	public float playerHealth = 100f;
	public float playerOxygen = 100f;
	public float oxygenRegen = 10f;
	public float healthRegen = 5f;
	public bool canRegenOxygen;
	public bool canRegenHealth;
	public int currentSuctionPathNum;
	public List<EnvironmentControl> suckerList = new List<EnvironmentControl>();
	public EnvironmentControl closestSucker;
	public float closestDist = 100000f;
	
	public bool outOfArea = false;
	public bool enteredSector = false;
	
	private float m_TickTime;
	private bool m_Outside;
	private Vector3 m_FloatingDirection;
	private float m_FloatingSpeed;

	[SerializeField] private GameObject m_Player;
	[SerializeField] private GameObject DeadScreen;
	[SerializeField] private GameObject BlockSaved;
	[SerializeField] private GameObject TimerText;

	private void Update() {
		if (closestSucker) {
			RaycastHit hit;
			Vector3 startPoint = closestSucker.transform.position;
			startPoint.y -= 3f;
			if (Physics.Raycast(startPoint, (transform.position - startPoint).normalized, out hit)) {
				if (!hit.collider.CompareTag("Player")) {
					closestSucker = null;
				}
			}
		}

		foreach (EnvironmentControl sucker in suckerList) {
			float newDist = Vector3.Distance(sucker.transform.position, transform.position);
			if ( newDist < closestDist) {
				closestDist = newDist;
				closestSucker = sucker;
			}
		}
		suckerList = new List<EnvironmentControl>();
		m_TickTime += Time.deltaTime;
		if (m_TickTime >= 1f) {
			Regen();
			m_TickTime = 0f;
		}
		
		if (outOfArea && !enteredSector) {
			canRegenHealth = false;
			canRegenOxygen = false;
			SetGravity(0);
			ReduceOxygen(0.5f);
		} else {
			enteredSector = false;
			canRegenOxygen = playerOxygen < 100;
			canRegenHealth = playerOxygen >= 100 && playerHealth < 100;
			SetGravity(1);
		}
	}

	private void FixedUpdate() {
		if (m_Outside) {
			transform.position = Vector3.MoveTowards(transform.position, transform.position + (m_FloatingDirection * 10), m_FloatingSpeed * Time.deltaTime);
		}
		
		if (closestSucker && !m_Outside) {
			float speed = Mathf.Clamp(300 / Vector3.Distance(closestSucker.transform.position, transform.position), 3, 300 );
			transform.position = Vector3.MoveTowards(transform.position, closestSucker.transform.position, speed * Time.deltaTime);
			if (Vector3.Distance(transform.position, closestSucker.transform.position) <= 2.5 && !m_Outside) {
				m_FloatingDirection = (closestSucker.transform.position - transform.position).normalized;
				m_FloatingSpeed = speed;
				m_Outside = true;
			}
		}
	}

	public void SetGravity(int val) {
		// Player Gravity state
		// 1 = Normal gravity
		// 0 = No gravity
		m_Player.GetComponent<Controller>().gravityMultiplier = val;

		if (val == 1) {
			m_Player.GetComponent<Controller>().canMove = true;
		} else {
			m_Player.GetComponent<Controller>().canMove = false;
		}
	}

	public void ReduceHealth(float val) {
		playerHealth -= val;
		Mathf.Clamp(playerHealth, 0f, maxHealth);
	}

	public void ReduceOxygen(float val) {
		if (playerOxygen <= 0) {
			if (playerHealth <= 0) {
				EnvironmentSettings.ActiveGame = false;
				DeadScreen.active = true;
				BlockSaved.transform.GetComponent<Text>().text = ""+EnvironmentSettings.safeBoxCount + " Boxes.";
				TimerText.GetComponent<Text>().text = ""+EnvironmentSettings.OveralTimer + " seconds.";
				m_Player.GetComponent<Controller>().m_CursorIsLocked = false;
				Cursor.visible = true;
			} else {
				ReduceHealth(0.5f);	
			}
		} else {
			playerOxygen -= val;	
		}
	}

	public void Regen() {
		if (canRegenHealth) {
			playerHealth = Mathf.Clamp(playerHealth + healthRegen, 0f, maxHealth);
		}
		if (canRegenOxygen) {
			playerOxygen = Mathf.Clamp(playerOxygen + oxygenRegen, 0f, maxOxygen);
		}
	}
}