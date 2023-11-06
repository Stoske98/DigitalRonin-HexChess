using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    #region MainMenuUIManager Singleton
    private static MainMenuUIManager _instance;

    public static MainMenuUIManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }
    #endregion

    private Animator animator;
    public GameObject connecting_panel;
    public Chat chat;

    public Button find_match_light_btn;
    public Button find_match_dark_btn;

    public GameObject finding_match_light;
    public GameObject finding_match_dark;

    public Button deselect_dark_btn;
    public Button deselect_light_btn;

    [Header("Player Info")]
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI rank;
    public TMP_InputField input_nickname;

    [Header("MatchFound")]
    public GameObject match_found_gameobject;
    public TextMeshProUGUI enemy_nickname;
    public TextMeshProUGUI enemy_rank;
    public TextMeshProUGUI status;
    public GameObject buttons;

    [Header("Settings")]
    public GameObject settings_gameobject;

    [Header("MatchFinding")]
    public Toggle light_check_box;
    public TMP_InputField light_code_input_field;
    public Toggle dark_check_box;
    public TMP_InputField dark_code_input_field;

    [Header("Loby")]
    public GameObject loby_go;
    public TextMeshProUGUI loby_status;
    public TextMeshProUGUI loby_id_txt;
    public TextMeshProUGUI code_txt;
    //public TextMeshProUGUI code_txt;
    public GameObject decline_loby_button;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void SelectLight()
    {
        animator.Play("SelectLight");
    }

    public void DeselectLight()
    {
        animator.Play("DeselectLight");
    }
    public void SelectDark()
    {
        animator.Play("SelectDark");
    }

    public void DeselectDark()
    {
        animator.Play("DeselectDark");
    }
    public void ChangeNickname()
    {
        NetChangeNickname request = new NetChangeNickname();
        request.nickname = input_nickname.text;
        Sender.SendToServer_Reliable(request);
    }
    public void ChangeNickname(string name)
    {
        nickname.text = name;
        input_nickname.gameObject.SetActive(false);
    }

    public void ActivateInputNickname()
    {
        input_nickname.text = nickname.text;
        input_nickname.gameObject.SetActive(true);
    }
    public void SetPlayerInfo(Player player)
    {
        nickname.text = player.data.nickname;
        rank.text = player.data.rank.ToString();
    }
    public void StartFindingMatchRequest(int class_type)
    {
        if(light_check_box.isOn)
        {
            if(!string.IsNullOrWhiteSpace(light_code_input_field.text))
            {
                NetJoinLoby request_join_loby = new NetJoinLoby();
                request_join_loby.loby_id = light_code_input_field.text;

                Sender.SendToServer_Reliable(request_join_loby);
            }
            else
            {
                NetCreateLoby request_code = new NetCreateLoby();
                request_code.class_type = (ClassType)class_type;

                Sender.SendToServer_Reliable(request_code);
            }

            return;
        }
        if (dark_check_box.isOn)
        {
            if (!string.IsNullOrWhiteSpace(dark_code_input_field.text))
            {
                NetJoinLoby request_join_loby = new NetJoinLoby();
                request_join_loby.loby_id = dark_code_input_field.text;

                Sender.SendToServer_Reliable(request_join_loby);
            }
            else
            {
                NetCreateLoby request_code = new NetCreateLoby();
                request_code.class_type = (ClassType)class_type;

                Sender.SendToServer_Reliable(request_code);
            }
            
            return;
        }

        NetCreateTicket request = new NetCreateTicket();
        request.class_type = (ClassType)class_type;

        Sender.SendToServer_Reliable(request);
    }
    public void StartFindingMatchResponess(ClassType class_type)
    {
        if (class_type == ClassType.Light)
        {
            deselect_light_btn.interactable = false;
            finding_match_light.SetActive(true);
            find_match_light_btn.interactable = false;

        }
        else
        {
            deselect_dark_btn.interactable = false;
            finding_match_dark.SetActive(true);
            find_match_dark_btn.interactable = false;
        }
    }

    public void StopFindingMatchResponess()
    {
        if (NetworkManager.Instance.player.data.class_type == ClassType.Light)
        {
            deselect_light_btn.interactable = true;
            finding_match_light.SetActive(false);
            find_match_light_btn.interactable = true;

        }
        else
        {
            deselect_dark_btn.interactable = true;
            finding_match_dark.SetActive(false);
            find_match_dark_btn.interactable = true;
        }
    }
    public void ShowMatchFound(PlayerData oppenent)
    {
        enemy_nickname.text = oppenent.nickname;
        enemy_rank.text = oppenent.rank.ToString();
        buttons.SetActive(true);
        match_found_gameobject.SetActive(true);
    }
    public void StopFindingMatchRequest()
    {
        NetStopMatchFinding request = new NetStopMatchFinding();
        Sender.SendToServer_Reliable(request);
    }

    public void AcceptMatchRequest()
    {
        NetAcceptMatch request = new NetAcceptMatch();
        Sender.SendToServer_Reliable(request);
    }
    public void OnAcceptMatchResponess(ClassType class_type)
    {
        status.color = Color.green;
        status.text = "Match has been ACCEPTED";
        if (class_type == NetworkManager.Instance.player.data.class_type)
            buttons.SetActive(false);
    }

    public void OnCreateLobyResponess(string code)
    {
        code_txt.gameObject.SetActive(true);
        loby_status.text = "";
        loby_id_txt.text = code;
        decline_loby_button.SetActive(true);
        loby_go.SetActive(true);

    }

    public void DeclineMatchRequest()
    {
        NetDeclineMatch request = new NetDeclineMatch();
        Sender.SendToServer_Reliable(request);
    }
    public void OnDeclineMatchResponess()
    {
        buttons.SetActive(false);
        decline_loby_button.SetActive(false);
        code_txt.gameObject.SetActive(false);
        loby_id_txt.text = "";
        loby_status.text = "Match has been DECLINED";
        light_code_input_field.text = "";
        dark_code_input_field.text = "";
        status.color = Color.red;
        status.text = "Match has been DECLINED";
        Invoke("ResetMatchBox", 2);

    }
    private void ResetMatchBox()
    {
        match_found_gameobject.SetActive(false);
        loby_go.SetActive(false);
        buttons.SetActive(true);
        status.text = "";
    }
    public void BothPlayerAcceptMatch()
    {
        status.color = Color.green;
        status.text = "Both player ACCEPTED the match";
        Invoke("Connecting", 2);
        Invoke("ResetMatchBox", 2);
        animator.Play("Idle");
    }
    public void Connecting()
    {
        connecting_panel.SetActive(true);
    }
    public void CopyText(TextMeshProUGUI text)
    {
        GUIUtility.systemCopyBuffer = text.text;
    }
    /* public void StartFindingMatchRequest(int class_type)
     {
         GameManager.Instance.player.data.class_type = (ClassType)class_type;

         NetCreateTicket request = new NetCreateTicket();
         request.class_type = (ClassType)class_type;
         Sender.TCP_SendToServer(request);

         AudioManager.Instance.OnClick();
     }
     public void StopFindingMatchRequest()
     {
         NetStopMatchFinding request = new NetStopMatchFinding();
         Sender.TCP_SendToServer(request);

         AudioManager.Instance.OnClick();
     }

     public void AcceptMatchRequest()
     {
         NetAcceptMatch request = new NetAcceptMatch();

         Data.Game data = new Data.Game();
         if (GameManager.Instance.player.data.class_type == ClassType.Light)
             GameManager.Instance.CreateTeam1(data, ClassType.Light);
         else
             GameManager.Instance.CreateTeam2(data, ClassType.Dark);

         request.xml_team_structure = Data.Serialize<Data.Game>(data);
         Sender.TCP_SendToServer(request);

         AudioManager.Instance.OnClick();
     }

     public void DeclineMatchRequest()
     {
         NetDeclineMatch request = new NetDeclineMatch();
         Sender.TCP_SendToServer(request);

         AudioManager.Instance.OnClick();
     }
     public void OnDeclineMatchResponess()
     {
             buttons.SetActive(false);
             status.color = Color.red;
             status.text = "Match has been DECLINED";
             Invoke("ResetMatchBox",2);

     }
     public void ShowMatchFound(Data.Player enemy)
     {
         enemy_nickname.text = enemy.nickname;
         enemy_rank.text = enemy.rank.ToString();
         buttons.SetActive(true);
         match_found_gameobject.SetActive(true);
         AudioManager.Instance.Play("MatchIsReady");
     }

     public void BothPlayerAcceptMatch()
     {
         status.color = Color.green;
         status.text = "Both player ACCEPTED the match"; 
         Invoke("ResetMatchBox", 2);
         animator.Play("Idle");
     }
     private void ResetMatchBox()
     {
         match_found_gameobject.SetActive(false);
         buttons.SetActive(true);
         status.text = "";
     }
     public void OnAcceptMatchResponess(ClassType class_type)
     {
         status.color = Color.green;
         status.text = "Match has been ACCEPTED";
         if (class_type == GameManager.Instance.player.data.class_type)
             buttons.SetActive(false);
     }
     public void StartFindingMatchResponess()
     {
         if (GameManager.Instance.player.data.class_type == ClassType.Light)
         {
             deselect_light_btn.interactable = false;
             finding_match_light.SetActive(true);
             find_match_light_btn.interactable = false;

         }
         else
         {
             deselect_dark_btn.interactable = false;
             finding_match_dark.SetActive(true);
             find_match_dark_btn.interactable = false;
         }
     }
     public void StopFindingMatchResponess()
     {
         if (GameManager.Instance.player.data.class_type == ClassType.Light)
         {
             deselect_light_btn.interactable = true;
             finding_match_light.SetActive(false);
             find_match_light_btn.interactable = true;

         }
         else
         {
             deselect_dark_btn.interactable = true;
             finding_match_dark.SetActive(false);
             find_match_dark_btn.interactable = true;
         }
     }

     public void ChangeNickname()
     {
         NetChangeNickname request = new NetChangeNickname();
         request.nickname = input_nickname.text;
         Sender.TCP_SendToServer(request);
     }

     public void ChangeNickname(string name)
     {
         nickname.text = name;
         input_nickname.gameObject.SetActive(false);
     }

     public void ActivateInputNickname()
     {
         input_nickname.text = nickname.text;
         input_nickname.gameObject.SetActive(true);

         AudioManager.Instance.OnClick();
     }
     public void SelectLight()
     {
         animator.Play("SelectLight");
         AudioManager.Instance.OnClick();
     }

     public void DeselectLight()
     {
         animator.Play("DeselectLight");
         AudioManager.Instance.OnClick();
     }
     public void SelectDark()
     {
         animator.Play("SelectDark");
         AudioManager.Instance.OnClick();
     }

     public void DeselectDark()
     {
         animator.Play("DeselectDark");
         AudioManager.Instance.OnClick();
     }

     public void OpenSettings()
     {
         settings_gameobject.SetActive(true);
         AudioManager.Instance.OnClick();
     }
     public void CloseSettings()
     {
         settings_gameobject.SetActive(false);
         AudioManager.Instance.OnClick();
     }
     public void SetPlayerInfo(Player player)
     {
         nickname.text = player.data.nickname;
         rank.text = player.data.rank.ToString();
     }
     public void OnConnected()
     {
         connecting_panel.SetActive(false);
     }

     public void Quit()
     {
         //RealtimeNetworking.Disconnected();
         Receiver.Instance.UnSubscibeOnLeaveMainMenu();
         Receiver.Instance.UnSubscribeOnLeaveMatch();
         Application.Quit();
     }*/
}
