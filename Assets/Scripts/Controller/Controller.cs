using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public class Controller : MonoBehaviour {
    [SerializeField] private bool m_IsWalking;
    [SerializeField] private float m_WalkSpeed = 6.0f;
    [SerializeField] private float m_RunSpeed = 8.0f;
    [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten = 0.8f;
    [SerializeField] private float m_JumpSpeed = 4f;
    [SerializeField] private float m_StickToGroundForce = 1f;
    [SerializeField] private float m_GravityMultiplier = 1f;
    [SerializeField] private float m_StepInterval = 5f;
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private float XSensitivity = 2.0f;
    [SerializeField] private float YSensitivity = 2.0f;
    [SerializeField] private Transform CameraAnchor;
    [SerializeField] private bool lockCursor = true;

    private CharacterController m_CharacterController;
    private AudioSource m_AudioSource;
    private bool m_Jump;
    private bool m_Jumping = false;
    private bool m_Moving = false;
    private bool m_PreviouslyGrounded;
    private float m_StepCycle;
    private float m_NextStep;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private Vector3 m_AirMoveCache;
    private CollisionFlags m_CollisionFlags;
    private Quaternion CameraRotation;
    private bool m_CursorIsLocked = true;

    [SerializeField] private Vector2 RotationRange = new Vector2(90, 361);
    private Vector3 TargetAngle;
    private Quaternion CapturedRotation;

    private void Start () {
        m_CharacterController = GetComponent<CharacterController>();
        m_AudioSource = GetComponent<AudioSource>();
        CameraRotation = CameraAnchor.localRotation;
    }
	
	// Update is called once per frame
	void Update () {
        if (!m_Jump) {
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded) {
            PlayLandingSound();
            m_MoveDir.y = 0f;
            m_Jumping = false;
        }
        if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded) {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_CharacterController.isGrounded;
        RotateView();
    }

    private void FixedUpdate() {
        float speed;
        GetInput(out speed);
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = m_Jumping ? m_AirMoveCache : CameraAnchor.forward * m_Input.y + CameraAnchor.right * m_Input.x;

        m_MoveDir.x = desiredMove.x * speed;
        m_MoveDir.z = desiredMove.z * speed;

        if (m_CharacterController.isGrounded) {
            m_MoveDir.y = -m_StickToGroundForce;

            if (m_Jump) {
                m_MoveDir.y = m_JumpSpeed;
                m_AirMoveCache = desiredMove;
                PlayJumpSound();
                m_Jump = false;
                m_Jumping = true;
            }
        } else {
            m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
        }
        m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

        ProgressStepCycle(speed);
    }

    private void PlayLandingSound() {
        m_AudioSource.clip = m_LandSound;
        m_AudioSource.Play();
        m_NextStep = m_StepCycle + .5f;
    }

    private void PlayJumpSound() {
        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.Play();
    }

    private void PlayFootStepAudio() {
        if (!m_CharacterController.isGrounded) {
            return;
        }

        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);

        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }

    private void ProgressStepCycle(float speed) {
        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0)) {
            m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                         Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep)) {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepAudio();
    }

    private void GetInput(out float speed) {
        // Read input
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        bool waswalking = m_IsWalking;

        // set the desired speed to be walking or running
        speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
        m_Input = new Vector2(horizontal, vertical);
        if(m_Input.x + m_Input.y > 0 || m_Input.x + m_Input.y < 0) {
            m_Moving = true;
        } else {
            m_Moving = false;
        }

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1) {
            m_Input.Normalize();
        }
    }

    private void RotateView() {
        float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
        float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

        if(TargetAngle.y > 180) {
            TargetAngle.y -= 360;
        }
        if(TargetAngle.x > 180) {
            TargetAngle.x -= 360;
        }
        if (TargetAngle.y < -180) {
            TargetAngle.y += 360;
        }
        if (TargetAngle.x < -180) {
            TargetAngle.x += 360;
        }

        TargetAngle.y += yRot;
        TargetAngle.x += xRot;

        TargetAngle.y = Mathf.Clamp(TargetAngle.y, -RotationRange.y * 0.5f, RotationRange.y * 0.5f);
        TargetAngle.x = Mathf.Clamp(TargetAngle.x, -RotationRange.x * 0.5f, RotationRange.x * 0.5f);

        if (m_Moving) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, TargetAngle.y, 0), 5 *Time.deltaTime);
        }
        CameraAnchor.rotation = Quaternion.Euler(-TargetAngle.x, TargetAngle.y, 0);

        UpdateCursorLock();
    }
    


    private void InternalLockUpdate() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            m_CursorIsLocked = false;
        } else if (Input.GetMouseButtonUp(0)) {
            m_CursorIsLocked = true;
        }

        if (m_CursorIsLocked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else if (!m_CursorIsLocked) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock() {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below) {
            return;
        }

        if (body == null || body.isKinematic) {
            return;
        }
        body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }
}