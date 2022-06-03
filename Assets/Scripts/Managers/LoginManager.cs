using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;


public class LoginManager : MonoBehaviour
{
    [Header("버튼 및 토글")]
    [SerializeField] private Button registerBtn;
    [SerializeField] private Button loginBtn;

    [SerializeField] private Toggle registerToggle;
    [SerializeField] private Toggle loginToggle;

    [Header("회원가입 UI 요소")]
    [SerializeField] private TMP_InputField registerNickNameInputField;
    [SerializeField] private TMP_InputField registerPW_InputField;
    [SerializeField] private TMP_InputField registerPW_CheckInputField;

    [Header("로그인 UI 요소")]
    [SerializeField] private TMP_InputField loginNickNameInputField;
    [SerializeField] private TMP_InputField loginPW_InputField;

    [Header("UI 오브젝트")]
    [SerializeField] private GameObject registerUI;
    [SerializeField] private GameObject loginUI;

    private bool isAutoLogin;

    private void Start()
    {
        // 리스너들 등록
        registerBtn.onClick.AddListener(OnClickRegisterBtn);
        loginBtn.onClick.AddListener(OnClickLoginBtn);

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
        loginNickNameInputField.text = EncryptPlayerPrefs.GetString(PrefsKeys.NICK_NAME);
        loginPW_InputField.text = EncryptPlayerPrefs.GetString(PrefsKeys.PW);

        // 로그인 시도
        OnClickLoginBtn();
    }

    private void OnDestroy()
    {
        // 리스너들 삭제
        registerBtn.onClick.RemoveAllListeners();
        loginBtn.onClick.RemoveAllListeners();

        registerToggle.onValueChanged.RemoveAllListeners();
        loginToggle.onValueChanged.RemoveAllListeners();
    }

    private void Update()
    {
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
        if (string.IsNullOrEmpty(registerNickNameInputField.text) || string.IsNullOrWhiteSpace(registerNickNameInputField.text))
        {
            print("닉네임 인풋필드 유효성 검사 실패");
            return false;
        }

        if (string.IsNullOrEmpty(registerPW_InputField.text) || string.IsNullOrWhiteSpace(registerPW_InputField.text))
        {
            print("비밀번호 인풋필드 유효성 검사 실패");
            return false;
        }

        if (string.IsNullOrEmpty(registerPW_CheckInputField.text) || string.IsNullOrWhiteSpace(registerPW_CheckInputField.text))
        {
            print("비밀번호 확인 인풋필드 유효성 검사 실패");
            return false;
        }

        if (registerPW_InputField.text.Equals(registerPW_CheckInputField.text) == false)
        {
            print("비밀번호가 다릅니다.");
            return false;
        }

        return true;
    }

    private void OnClickRegisterBtn()
    {
        // 인풋필드 유효성 검사
        if (CheckRegisterInputField() == false)
        {
            return;
        }

        // 닉네임, 비번만으로 회원가입 요청
        var request = new RegisterPlayFabUserRequest { Username = registerNickNameInputField.text, Password = registerPW_InputField.text, RequireBothUsernameAndEmail = false };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnSuccessRegister, (error) => print($"회원가입 실패 이유 : {error}"));
    }

    private bool CheckLoginInputField()
    {
        if (string.IsNullOrEmpty(loginNickNameInputField.text) || string.IsNullOrWhiteSpace(loginNickNameInputField.text))
        {
            print("닉네임 인풋필드 유효성 검사 실패");
            return false;
        }

        if (string.IsNullOrEmpty(loginPW_InputField.text) || string.IsNullOrWhiteSpace(loginPW_InputField.text))
        {
            print("비밀번호 인풋필드 유효성 검사 실패");
            return false;
        }

        return true;
    }

    private void OnClickLoginBtn()
    {
        // 인풋필드 유효성 검사
        if (CheckLoginInputField() == false)
        {
            return;
        }

        var request = new LoginWithPlayFabRequest { Username = loginNickNameInputField.text, Password = loginPW_InputField.text };
        PlayFabClientAPI.LoginWithPlayFab(request, OnSuccessLogin, OnFailedLogin);
    }

    private void OnSuccessRegister(RegisterPlayFabUserResult result)
    {
        print($"회원가입 성공! : {result}");

        loginNickNameInputField.text = registerNickNameInputField.text;
        loginPW_InputField.text = registerPW_InputField.text;

        // 로그인 탭 활성화
        loginToggle.isOn = true;

        OnClickLoginBtn();
    }

    private void OnSuccessLogin(LoginResult result)
    {
        print($"로그인 성공! : {result}");

        if (isAutoLogin == false)
        {
            EncryptPlayerPrefs.SetBool(PrefsKeys.IS_AUTO_LOGIN, true);
            isAutoLogin = true;
            print("자동 로그인 세팅");
        }

        EncryptPlayerPrefs.SetString(PrefsKeys.NICK_NAME, loginNickNameInputField.text);
        EncryptPlayerPrefs.SetString(PrefsKeys.PW, loginPW_InputField.text);

        // 타이틀 UI 활성화
    }
    
    private void OnFailedLogin(PlayFabError error)
    {
        print($"로그인 실패 이유 : {error}");

        // 자동 로그인 키가 활성화되어서 자동로그인 했지만 실패한 경우, 계정 정보가 바뀌었다는 뜻이므로 취소
        if (isAutoLogin)
        {
            EncryptPlayerPrefs.DeleteKey(PrefsKeys.IS_AUTO_LOGIN);
            isAutoLogin = false;
            print("자동 로그인 실패했으므로 키 삭제");
        }
    }
}
