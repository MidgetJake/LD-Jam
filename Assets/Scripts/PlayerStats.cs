using UnityEngine;

public class PlayerStats : MonoBehaviour {
	public float maxHealth = 100f;
	public float maxOxygen = 100f;
	public float playerHealth = 100f;
	public float playerOxygen = 100f;
	public float oxygenRegen = 10f;
	public float healthRegen = 5f;
	public bool canRegenOxygen;
	public bool canRegenHealth;
	
	public bool outOfArea = false;
	public bool enteredSector = false;
	
	private float m_TickTime;
	

	[SerializeField] private GameObject m_Player;

	private void Update() {
		m_TickTime += Time.deltaTime;
		if (m_TickTime >= 1f) {
			Regen();
			m_TickTime = 0f;
		}
		
		if (outOfArea && !enteredSector) {
			canRegenHealth = false;
			canRegenOxygen = false;
			SetGravity(0);
			ReduceOxygen(0.25f);
		} else {
			canRegenOxygen = playerOxygen < 100;
			canRegenHealth = playerOxygen >= 100 && playerHealth < 100;
			SetGravity(1);
		}
	}
	
	public void SetGravity(float val) {
		// Player Gravity state
		// 1 = Normal gravity
		// 0 = No gravity
		m_Player.GetComponent<Controller.Controller>().gravityMultiplier = val;
	}

	public void ReduceHealth(float val) {
		playerHealth -= val;
		Mathf.Clamp(playerHealth, 0f, maxHealth);
	}

	public void ReduceOxygen(float val) {
		if (playerOxygen <= 0) {
			ReduceHealth(0.5f);
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