using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using System.Collections;

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviourPunCallbacks
    {
        [SerializeField] private CinemachineVirtualCamera playerFollowCamera;
        [SerializeField] private GameObject overlayCanvas;

        [SerializeField] private float normalSensitivity;
        [SerializeField] private float aimSensitivity;
        [SerializeField] private float normalAimRange = 40f;
        [SerializeField] private float distanceRay = 999f;
        [SerializeField] private int currentWeaponIndex => weaponToggles.IndexOf(weaponToggles.First(toggle => toggle.isOn));
        [SerializeField] private Weapon currentWeapon => weapons[weaponToggles[currentWeaponIndex]];
        private ThirdPersonController thirdPersonController;

        [Header("Weapons")]
        [SerializeField] private Transform weaponRoot;
        private List<Toggle> weaponToggles;
        private Dictionary<Toggle, Weapon> weapons = new Dictionary<Toggle, Weapon>();

        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public float scrollWeaponInput;
        public bool jump;
        public bool sprint;
        public bool crouch;
        public bool jumpStop;
        public bool prone;
        public bool aim;
        public bool shoot;
        public bool reload;
        public bool inGameMenu;
        [SerializeField] protected UnityEvent<int> OnWeaponSwitch = new UnityEvent<int>();

        protected Vector2 windowCenter => new Vector2(Screen.width / 2f, Screen.height / 2f);
        [SerializeField] protected float turningRate = 0.5f;
        //private CinemachineBrain _mainCameraBrain;
        //private CinemachineBrain mainCameraBrain => _mainCameraBrain is null ? _mainCameraBrain = Camera.main.GetComponent<CinemachineBrain>() : _mainCameraBrain;

        //private GameObject _mainCameraTargetVirtualCamera;
        //private GameObject mainCameraTarget => mainCameraBrain.ActiveVirtualCamera.VirtualCameraGameObject;

        private Camera _mainCamera;
        private Camera mainCamera => _mainCamera is null ? _mainCamera = Camera.main : _mainCamera;
        private GameObject inGameMenuCanvas;

        [Header("Movement Settings")]
        public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;
#endif

        [Header("PhotonSettings")]
        PhotonView PV;

        public CharacterController characterController;
        public Animator animator;
        [SerializeField] private Image[] weaponIcon;
        [SerializeField] private Image weaponEnable;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator.SetBool("MainWeapon", true);
            PV = GetComponent<PhotonView>();
            animator = GetComponent<Animator>();
            thirdPersonController = GetComponent<ThirdPersonController>();
            OnWeaponSwitch.AddListener((int value) => SwitchWeapon(value));

            weaponToggles = weaponRoot.GetComponentsInChildren<Toggle>(true).ToList();

            foreach (var toggleWithWeapon in weaponToggles)
            {
                Weapon weaponOnToggle = toggleWithWeapon.gameObject.GetComponentInChildren<Weapon>();
                if (!weaponOnToggle)
                {
                    Debug.LogError($"Toggle \"{toggleWithWeapon.name}\" has no {nameof(Weapon)}", toggleWithWeapon);
                    continue;
                }
                weapons.Add(toggleWithWeapon, weaponOnToggle);
            }
            Cursor.lockState = CursorLockMode.Locked;
        }
        protected void Start()
        {
            if (!PV.IsMine)
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
                Destroy(GetComponentInChildren<CinemachineVirtualCamera>().gameObject);
                Destroy(overlayCanvas);

            }
            OnWeaponSwitch.Invoke(0);
            inGameMenuCanvas = overlayCanvas.transform.Find("InGameMenu").gameObject;
        }

        private void Update()
        {
            if (!PV.IsMine)
                return;
            //check if delta rotate is enough
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, mainCamera.transform.rotation.eulerAngles.y, 0)), 1 / turningRate * Time.deltaTime);
            Vector3 mouseWorldPosition = Vector3.zero;

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = mainCamera.ScreenPointToRay(screenCenterPoint);
            Transform hitTransform = null;
            if (Physics.Raycast(ray, out RaycastHit raycastHit, distanceRay))
            {
                mouseWorldPosition = raycastHit.point;
                hitTransform = raycastHit.transform;

            }

            if (aim)
            {
                if (currentWeapon.TryGetComponent<RangedWeapon>(out var rangedWeapon))
                {
                    rangedWeapon.Aim(playerFollowCamera);
                    thirdPersonController.SetSensitivity(aimSensitivity);
                    thirdPersonController.SetRotateOnMove(false);
                }
            }
            else
            {
                playerFollowCamera.m_Lens.FieldOfView = normalAimRange;
                thirdPersonController.SetSensitivity(normalSensitivity);
                thirdPersonController.SetRotateOnMove(true);
            }

            if (shoot)
            {
                animator.SetBool("KnifeAttack", true);
                animator.SetBool("Grenade", true);
                Debug.Log("Input shoot");
                currentWeapon.Attack();
                if (currentWeapon is Grenades) weapons.First(pair => pair.Value == currentWeapon).Key.interactable = false;
                if (currentWeapon.TryGetComponent<Weapon>(out var weapon))
                {
                    var toggle = weapons.FirstOrDefault(pair => pair.Value == currentWeapon).Key;
                    if (toggle is null) throw new NullReferenceException(nameof(toggle));
                    
                    if (!toggle.interactable)
                    {
                        var firstAvailableWeaponIndex = weapons.Keys.ToList().IndexOf(weapons.FirstOrDefault(pair => pair.Key.interactable).Key);
                        if (firstAvailableWeaponIndex == -1) throw new Exception("No weapon available");
                        OnWeaponSwitch.Invoke(firstAvailableWeaponIndex);
                    }
                }
            }
            else
            {
                animator.SetBool("KnifeAttack", false);
                animator.SetBool("Grenade", false);
            }
            if (inGameMenuCanvas.active)
            {
                Cursor.lockState = CursorLockMode.Confined;
                inGameMenu = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                inGameMenu = false;
            }
            if (reload)
            {
                Debug.Log("Input R");
                if (currentWeapon.TryGetComponent<RangedWeapon>(out var rangedWeapon))
                {
                    animator.SetTrigger("Reload");
                    rangedWeapon.Reload();
                }

            }
        }

        protected void SwitchWeapon(int value)
        {
            if (PV.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("currentWeaponIndex", currentWeaponIndex);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
            weaponToggles[value].isOn = true;
            switch (value)
            {
                case 0:
                    animator.Play("GunMove");
                    break;
                case 1:
                    animator.Play("PistolMove");
                    break;
                case 2:
                    animator.Play("IdleKnife");
                    break;
                case 3:
                    animator.Play("GrenadeIdle");
                    break;
            }
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) //test
        {
            if (!PV.IsMine && targetPlayer == PV.Owner)
            {
                SwitchWeapon((int)changedProps["currentWeaponIndex"]);
            }
        }

        public void OnCrouch(InputValue value)
        {
            CrouchInput(value.isPressed);
            if (value.isPressed)
            {
                characterController.height = 1.3f;
                characterController.center = new Vector3(0, 0.77f, 0);
            }
            else
            {
                characterController.height = 1.65f;
                characterController.center = new Vector3(0, 0.85f, 0);
            }
        }
        public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());
        public void OnLook(InputValue value)
        {
            if (!cursorInputForLook) return;
            LookInput(value.Get<Vector2>());
        }
        public void OnJump(InputValue value) => JumpInput(value.isPressed);
        public void OnSprint(InputValue value) => SprintInput(value.isPressed);
        public void OnAim(InputValue value) => AimInput(value.isPressed);
        public void OnInGameMenu(InputValue value)
        {
            inGameMenu = !inGameMenu;
            if (inGameMenu)
            {
                inGameMenuCanvas.SetActive(true);
            }
            else
            {
                inGameMenuCanvas.SetActive(false);
            }
            
        }
        public void ExitGame()
        {
            Application.Quit();
        }

        public void OnShoot(InputValue value) => ShootInput(value.isPressed);
        public void OnReload(InputValue value) => ReloadInput(value.isPressed);

        #region On#Weapon Input

        public void OnFirstWeapon(InputValue value)
        {
            if (!value.isPressed) return;
            animator.Play("GunMove");
            animator.SetBool("MainWeapon", true);
            animator.SetBool("SecondWeapon", false);
            weaponEnable.transform.position = weaponIcon[0].transform.position;
            OnWeaponSwitch.Invoke(0);
        }
        public void OnSecondWeapon(InputValue value)
        {
            if (!value.isPressed) return;
            animator.Play("PistolMove");
            animator.SetBool("SecondWeapon", true);
            animator.SetBool("MainWeapon", false);
            weaponEnable.transform.position = weaponIcon[1].transform.position;
            OnWeaponSwitch.Invoke(1);
        }
        public void OnThirdWeapon(InputValue value)
        {
            if (!value.isPressed) return;
            animator.Play("IdleKnife");
            weaponEnable.transform.position = weaponIcon[2].transform.position;
            OnWeaponSwitch.Invoke(2);
        }
        public void OnForthWeapon(InputValue value)
        {
            if (!value.isPressed || !weapons.First(pair => pair.Value == weapons.Values.First(val => val is Grenades)).Key.interactable) return;
            animator.Play("GrenadeIdle");
            weaponEnable.transform.position = weaponIcon[3].transform.position;
            OnWeaponSwitch.Invoke(3);
        }

        #endregion

        public void OnScrollWeapon(InputValue value) => ScrollWeaponInput(value.Get<float>());

        public void ProneInput(bool newProneState) => prone = newProneState;
        public void MoveInput(Vector2 newMoveDirection) => move = newMoveDirection;
        public void LookInput(Vector2 newLookDirection) => look = newLookDirection;
        public void CrouchInput(bool newCrouchState) => crouch = newCrouchState;
        public void JumpInput(bool newJumpState) => jump = newJumpState;
        public void SprintInput(bool newSprintState) => sprint = newSprintState;
        public void AimInput(bool newAimState) => aim = newAimState;
        public void InGameMenu(bool newInGameMenuState) => inGameMenu = newInGameMenuState;
        public void ShootInput(bool newShootState) => shoot = newShootState;
        public void ReloadInput(bool newReloadState) => reload = newReloadState;
        public void ScrollWeaponInput(float value)
        {
            if (value == 0) return;
            OnWeaponSwitch.Invoke
            (
                value > 0 ?
                currentWeaponIndex + 1 == weapons.Count ? 0 : currentWeaponIndex + 1 :
                currentWeaponIndex - 1 < 0 ? weapons.Count - 1 : currentWeaponIndex - 1
            );
        }
        private void FixedUpdate()
        {

            if (!PV.IsMine)
                return;
        }


#if !UNITY_IOS || !UNITY_ANDROID

        private void OnApplicationFocus(bool hasFocus) => SetCursorState(cursorLocked);
        private void SetCursorState(bool newState) => Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;

#endif
    }

}