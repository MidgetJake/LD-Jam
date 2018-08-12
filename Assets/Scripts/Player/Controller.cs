﻿using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;
using Game;

namespace Player {
    public class Controller : MonoBehaviour {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed = 6.0f;
        [SerializeField] private float m_RunSpeed = 8.0f;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten = 0.8f;
        [SerializeField] private float m_JumpSpeed = 4f;
        [SerializeField] private float m_StickToGroundForce = 1f;
        [SerializeField] private float m_StepInterval = 5f;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;
        [SerializeField] private Camera m_Camera;
        [SerializeField] private float m_XSensitivity = 2.0f;
        [SerializeField] private float m_YSensitivity = 2.0f;
        [SerializeField] private Transform m_CameraAnchor;
        [SerializeField] private bool m_LockCursor = true;
        [SerializeField] private Vector2 m_RotationRange = new Vector2(90, 361);
        [SerializeField] public bool m_CameraShake = false;
        
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
        private Quaternion m_CameraRotation;
        private bool m_CursorIsLocked = true;
        private Vector3 m_TargetAngle;
        private Quaternion m_CapturedRotation;
        
        public float gravityMultiplier = 1f;
        public List<Tool> inventory = new List<Tool>();
        public Tool activeTool;
        public float shakeAmount = 0.25f;
        public float shakeTime = 5;
        public float shakeTick = 5;
        public float deadSectorPlayerDistance;

        private void Start () {
            m_CharacterController = GetComponent<CharacterController>();
            m_AudioSource = GetComponent<AudioSource>();
            m_CameraRotation = m_CameraAnchor.localRotation;
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

            for (int i = 0; i < 10; ++i) {
                if (Input.GetKeyDown("" + i)) {
                    if (i == 0) {
                        SetActiveItem(9);
                    } else {
                        SetActiveItem(i - 1);
                    }
                }
            }
            
            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            if (CrossPlatformInputManager.GetButtonDown("Fire1") && activeTool != null) {
                activeTool.UseItem();
            }
            
            RotateView();
        }

        public void SetItemUsable(bool usable, string type, GameObject interactable) {
            if (activeTool != null) {
                activeTool.SetUsable(usable, type, interactable);
            }
        }
        
        private void SetActiveItem(int item) {
            if (activeTool != null) {
                activeTool.UnEquip();
            }

            if (item < inventory.Count) {
                activeTool = inventory[item];
                activeTool.Equip();
            }
        }
        
        private void FixedUpdate() {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = m_Jumping ? m_AirMoveCache : m_CameraAnchor.forward * m_Input.y + m_CameraAnchor.right * m_Input.x;

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
                m_MoveDir += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            
            if (m_CameraShake) {
                shakeAmount = (1-(deadSectorPlayerDistance / 100))-(shakeTick/10);

                if (shakeAmount > 0.45f) {
                    shakeAmount = 0.45f;
                }
                
                if (shakeTick >= shakeTime || shakeAmount <= 0) {
                    shakeAmount = 0;
                    m_CameraShake = false;
                    shakeTick = 0;
                }
                
                m_Camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                shakeTick += 1*Time.deltaTime;
            } else {
                m_Camera.transform.localPosition = Vector3.zero;
                shakeAmount = 0f;
            }
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
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * m_XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * m_YSensitivity;

            if(m_TargetAngle.y > 180) {
                m_TargetAngle.y -= 360;
            }
            if(m_TargetAngle.x > 180) {
                m_TargetAngle.x -= 360;
            }
            if (m_TargetAngle.y < -180) {
                m_TargetAngle.y += 360;
            }
            if (m_TargetAngle.x < -180) {
                m_TargetAngle.x += 360;
            }

            m_TargetAngle.y += yRot;
            m_TargetAngle.x += xRot;

            m_TargetAngle.y = Mathf.Clamp(m_TargetAngle.y, -m_RotationRange.y * 0.5f, m_RotationRange.y * 0.5f);
            m_TargetAngle.x = Mathf.Clamp(m_TargetAngle.x, -m_RotationRange.x * 0.5f, m_RotationRange.x * 0.5f);

            if (m_Moving) {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, m_TargetAngle.y, 0), 5 *Time.deltaTime);
            }
            m_CameraAnchor.rotation = Quaternion.Euler(-m_TargetAngle.x, m_TargetAngle.y, 0);

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
            if (m_LockCursor)
                InternalLockUpdate();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (hit.transform.CompareTag("Tool")) {
                Tool newTool = hit.transform.GetComponent<Tool>();
                newTool.Pickup(transform.position);
                hit.transform.SetParent(transform);
                inventory.Add(newTool);
            }
            
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
}