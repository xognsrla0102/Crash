using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class LoginManager : MonoBehaviour
{
    [Header("버튼 및 토글")]
    [SerializeField] private Button registerBtn;
    [SerializeField] private Button loginBtn;
    [SerializeField] private Button logoutBtn;

    [SerializeField] private Toggle registerToggle;
    [SerializeField] private Toggle loginToggle;

    [Header("회원가입 UI 요소")]
    [SerializeField] private InputField registerUserNameInputField;
    [SerializeField] private InputField registerPwInputField;
    [SerializeField] private InputField registerPwCheckInputField;

    [Header("로그인 UI 요소")]
    [SerializeField] private InputField loginUserNameInputField;
    [SerializeField] private InputField loginPwInputField;

    [Header("UI 오브젝트")]
    [SerializeField] private GameObject tabUI;
    [SerializeField] private GameObject registerUI;
    [SerializeField] private GameObject loginUI;
    [SerializeField] private GameObject titleUI;

    private bool isAutoLogin;

    private void Start()
    {
        // 회원가입/로그인 탭 활성화
        titleUI.SetActive(false);
        tabUI.SetActive(true);

        // 리스너들 등록
        registerBtn.onClick.AddListener(OnClickRegisterBtn);
        loginBtn.onClick.AddListener(OnClickLoginBtn);
        logoutBtn.onClick.AddListener(OnClickLogoutBtn);

        registerToggle.onValueChanged.AddListener(OnClickRegisterToggle);
        loginToggle.onValueChanged.AddListener(OnClickLoginToggle);

        // 이전에 로그인 한 적 있으면 자동 로그인
        isAutoLogin = EncryptPlayerPrefs.GetBool(PrefsKeys.IS_AUTO_LOGIN);
        if (isAutoLogin)
        {
            AutoLogin();
        }
    }

    private void AutoLogin()
    {
        print("자동 로그인 시도");

        // 로그인 탭 활성화
        loginToggle.isOn = true;

        // 이전에 입력한 계정 정보 받음
        loginUserNameInputField.text = EncryptPlayerPrefs.GetString(PrefsKeys.USER_NAME);
        loginPwInputField.text = EncryptPlayerPrefs.GetString(PrefsKeys.PW);

        // 로그인 시도
        OnClickLoginBtn();
    }

    private void OnDestroy()
    {
        // 리스너들 삭제
        registerBtn.onClick.RemoveAllListeners();
        loginBtn.onClick.RemoveAllListeners();
        logoutBtn.onClick.RemoveAllListeners();

        registerToggle.onValueChanged.RemoveAllListeners();
        loginToggle.onValueChanged.RemoveAllListeners();
    }

    private void Update()
    {
        InputKeyboard();
    }

    private void InputKeyboard()
    {
        // 팝업 활성화된 상태라면 입력 무시
        if (FindObjectOfType<Popup>() != null)
        {
            return;
        }

        // Tab 키로 현재 인풋필드에서 다음 인풋필드로 넘어가도록 설정
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                Selectable nowSelectedObj = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
                if (nowSelectedObj != null)
                {
                    Selectable nextSelectedObj = nowSelectedObj.FindSelectableOnDown();
                    if (nextSelectedObj != null)
                    {
                        nextSelectedObj.Select();
                    }
                }
            }
        }

        // Enter 키로 회원가입이나 로그인 버튼 작동하도록 설정
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (registerUI.activeInHierarchy)
            {
                OnClickRegisterBtn();
            }
            else if (loginUI.activeInHierarchy)
            {
                OnClickLoginBtn();
            }
            else
            {
                Debug.Assert(false);
            }
        }
    }

    private void OnClickRegisterToggle(bool isOn)
    {
        if (isOn == false)
        {
            return;
        }

        // 회원가입 탭 활성화
        registerUI.SetActive(true);
        loginUI.SetActive(false);
    }

    private void OnClickLoginToggle(bool isOn)
    {
        if (isOn == false)
        {
            return;
        }
        
        // 로그인 탭 활성화
        registerUI.SetActive(false);
        loginUI.SetActive(true);
    }

    private bool CheckRegisterInputField()
    {
        if (string.IsNullOrEmpty(registerUserNameInputField.text) || string.IsNullOrWhiteSpace(registerUserNameInputField.text))
        {
            Popup.CreateInfoPopup("Register Failed", "Please Input Username[ID].");
            return false;
        }

        if (string.IsNullOrEmpty(registerPwInputField.text) || string.IsNullOrWhiteSpace(registerPwInputField.text))
        {
            Popup.CreateInfoPopup("Register Failed", "Please Input Password.");
            return false;
        }

        if (string.IsNullOrEmpty(registerPwCheckInputField.text) || string.IsNullOrWhiteSpace(registerPwCheckInputField.text))
        {
            Popup.CreateInfoPopup("Register Failed", "Please Input Password Check.");
            return false;
        }

        if (registerPwInputField.text.Equals(registerPwCheckInputField.text) == false)
        {
            Popup.CreateInfoPopup("Register Failed", "It's not match Password Check.");
            return false;
        }

        return true;
    }

    private void OnClickRegisterBtn()
    {
        if (registerBtn.interactable == false)
        {
            return;
        }

        // 인풋필드 유효성 검사
        if (CheckRegisterInputField() == false)
        {
            print("회원가입 인풋필드 유효성 검사 실패");
            // 현재 활성화된 인풋 모듈(버튼, 인풋 필드) 비활성화
            EventSystem.current.currentInputModule.DeactivateModule();
            return;
        }

        registerBtn.interactable = false;

        // 닉네임(ID으로 쓰임), 비번만으로 회원가입 요청
        var request = new RegisterPlayFabUserRequest { Username = registerUserNameInputField.text, Password = registerPwInputField.text, RequireBothUsernameAndEmail = false };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnSuccessRegister, OnFailedRegister);
    }

    private bool CheckLoginInputField()
    {
        if (string.IsNullOrEmpty(loginUserNameInputField.text) || string.IsNullOrWhiteSpace(loginUserNameInputField.text))
        {
            Popup.CreateInfoPopup("Login Failed", "Please Input Username[ID].");
            return false;
        }

        if (string.IsNullOrEmpty(loginPwInputField.text) || string.IsNullOrWhiteSpace(loginPwInputField.text))
        {
            Popup.CreateInfoPopup("Login Failed", "Please Input Password.");
            return false;
        }

        return true;
    }

    private void OnClickLoginBtn()
    {
        if (loginBtn.interactable == false)
        {
            return;
        }

        // 인풋필드 유효성 검사
        if (CheckLoginInputField() == false)
        {
            print("로그인 인풋필드 유효성 검사 실패");
            // 현재 활성화된 인풋 모듈(버튼, 인풋 필드) 비활성화
            EventSystem.current.currentInputModule.DeactivateModule();
            return;
        }

        loginBtn.interactable = false;

        var request = new LoginWithPlayFabRequest { Username = loginUserNameInputField.text, Password = loginPwInputField.text };
        PlayFabClientAPI.LoginWithPlayFab(request, OnSuccessLogin, OnFailedLogin);
    }

    private void OnSuccessRegister(RegisterPlayFabUserResult result)
    {
        registerBtn.interactable = true;

        print($"회원가입 성공! : {result}");

        loginUserNameInputField.text = registerUserNameInputField.text;
        loginPwInputField.text = registerPwInputField.text;

        // 로그인 탭 활성화
        loginToggle.isOn = true;

        OnClickLoginBtn();
    }

    private void OnFailedRegister(PlayFabError error)
    {
        registerBtn.interactable = true;
        Popup.CreateInfoPopup("Register Failed", error);
        print($"회원가입 실패 이유 : {error}");
    }

    private void OnSuccessLogin(LoginResult result)
    {
        print($"로그인 성공! : {result}");
        loginBtn.interactable = true;

        if (isAutoLogin == false)
        {
            EncryptPlayerPrefs.SetBool(PrefsKeys.IS_AUTO_LOGIN, true);
            isAutoLogin = true;
            print("자동 로그인 세팅");
        }

        EncryptPlayerPrefs.SetString(PrefsKeys.USER_NAME, loginUserNameInputField.text);
        EncryptPlayerPrefs.SetString(PrefsKeys.PW, loginPwInputField.text);

        // 타이틀 UI 활성화
        tabUI.SetActive(false);
        titleUI.SetActive(true);
    }
    
    private void OnFailedLogin(PlayFabError error)
    {
        loginBtn.interactable = true;

        Popup.CreateInfoPopup("Login Failed", error);
        print($"로그인 실패 이유 : {error}");

        // 자동 로그인 키가 활성화되어서 자동로그인 했지만 실패한 경우, 계정 정보가 바뀌었다는 뜻이므로 취소
        if (isAutoLogin)
        {
            EncryptPlayerPrefs.DeleteKey(PrefsKeys.IS_AUTO_LOGIN);
            isAutoLogin = false;
            print("자동 로그인 실패했으므로 키 삭제");
        }
    }

    private void OnClickLogoutBtn()
    {
        // 로그인 API 사용을 허용하는 클라이언트 세션 토큰 삭제
        PlayFabClientAPI.ForgetAllCredentials();

        // 로그인 UI 활성화
        titleUI.SetActive(false);
        tabUI.SetActive(true);

        // 로그아웃 시에는 자동 로그인 해제
        EncryptPlayerPrefs.DeleteKey(PrefsKeys.IS_AUTO_LOGIN);
        isAutoLogin = false;
        print("로그아웃으로 인한 자동 로그인 키 삭제");
    }
}
