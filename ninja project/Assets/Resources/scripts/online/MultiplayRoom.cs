using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MultiplayRoom : MonoBehaviourPunCallbacks
{
    public AudioSource _bgm;
    public AudioClip _defaultclip;
    public AudioClip _runsetBGM;
    public GameObject fadein;
    private AudioSource audioSource;
    public AudioClip mese;
    public AudioClip aoooose;
    public InputField roomname;
    public InputField playername;
    public Animator matchanim;
    public Image[] matchfrogimg;
    public Text[] matchfrogname;
    public Text currentstatus;
    private TypedLobby sqlLobby = new TypedLobby("Normal", LobbyType.SqlLobby);
    private RoomOptions roomOptions;
    private string sql_roomtype = "Normal";

    public int isvictory = -1;//0敗北、1勝利
    public string roomID;
    public int[] matchplayer_frogid;
    public GameObject escapemacth;
    //生成関連
    public int selectstage = 0;
    [System.Serializable]
    public struct StageID
    {
        public GameObject stageall;
        public Transform[] masterstartpos;
        public Transform[] otherstartpos;
        public GameObject[] enemys;
        public GameObject[] gimmicks;
        public GameObject player;
        public GameObject currentplobj;
    }
    public StageID[] MultiStages;
    public GameObject missionstartUI;
    public GameObject fadeindestroy;
    public GameObject fadeout;
    private string tmpvarname = "";
    private string nextscene = "";
    public bool isruncheck = false;
    public int thismatchindex = 0;
    private void Start()
    {
        playername.text = PlayerPrefs.GetString("multiname", "");
        if (GetComponent<AudioSource>()) audioSource = GetComponent<AudioSource>();
        if (GManager.instance.isEnglish == 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "▼モード選択前に入力してね▼";
        }
        else if (GManager.instance.isEnglish != 0)
        {
            currentstatus.fontSize = 12;
            currentstatus.text = "▼Enter it before selecting the mode!▼";
        }
    }
    private void Update()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers && !GManager.instance.ismatch)
            {
                GManager.instance.ismatch = true;
                Invoke(nameof(MatchPlay), 0.3f);
            }
            else if (GManager.instance.ismatch && isvictory == -1 && PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                isvictory = 1;
            }
        }
    }
    public void NormalRoomMatch()//ノーマルモード
    {
        if (playername.text == "") playername.text = "Player";
        PhotonNetwork.NickName = playername.text;
        sql_roomtype = "Normal";
        sqlLobby = new TypedLobby(sql_roomtype, LobbyType.SqlLobby);

        if (audioSource && mese) audioSource.PlayOneShot(mese);
        if (GManager.instance.isEnglish == 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "対戦相手が見つかるを待っています。";
        }
        else if (GManager.instance.isEnglish != 0)
        {
            currentstatus.fontSize = 12;
            currentstatus.text = "We are waiting for you to find an opponent.";
        }
        matchfrogname[0].text = PhotonNetwork.NickName;
        matchfrogimg[0].sprite = GManager.instance.all_frog[GManager.instance.set_playerselect].frog_image;
        matchanim.SetInteger("trg", 1);

        // 未接続の場合は接続する
        PhotonNetwork.ConnectUsingSettings();
    }
    //public void Event1RoomMatch()//イベント1モード
    //{
    //    PhotonNetwork.NickName = playername.text;
    //sql_roomtype = "Event1";
    //sqlLobby = new TypedLobby(sql_roomtype, LobbyType.SqlLobby);
    //    // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
    //    PhotonNetwork.ConnectUsingSettings();
    //}
    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // ルームの参加人数を2人に設定する
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        if (roomname.text == "") PhotonNetwork.JoinRandomOrCreateRoom(null, 2, MatchmakingMode.FillRoom, sqlLobby, null, null, roomOptions);
        else PhotonNetwork.JoinOrCreateRoom(roomname.text, roomOptions, sqlLobby);
        //PhotonNetwork.GetCustomRoomList(sqlLobby, sql_roomtype);
    }
    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{
    //    if (roomList.Count == 0)
    //    {
    //        if (roomname.text != "") PhotonNetwork.JoinOrCreateRoom(roomname.text, roomOptions, sqlLobby);
    //        else PhotonNetwork.JoinRandomRoom();
    //    }
    //    else
    //    {
    //        foreach (RoomInfo roomInfo in roomList)
    //        {
    //            if (roomname.text == "")
    //            {
    //                PhotonNetwork.JoinRandomOrCreateRoom(null, 2, MatchmakingMode.FillRoom, sqlLobby, sql_roomtype, null, roomOptions);
    //            }
    //            else if (roomInfo.Name == roomname.text)
    //            {
    //                PhotonNetwork.JoinRoom(roomname.text);
    //            }
    //            else if(roomInfo.Name!= roomname.text)
    //            {
    //                PhotonNetwork.CreateRoom(roomname.text, roomOptions, sqlLobby);
    //            }
    //        }
    //    }
    //}
    // ランダムで参加できるルームが存在しないなら、新規でルームを作成する
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // ルームの参加人数を2人に設定する
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(null, roomOptions, sqlLobby);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        PlayerPrefs.SetString("multiname", playername.text);
        PlayerPrefs.Save();
        //自分の蛙情報を登録
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // ルームのID（名前）を取得
            roomID = PhotonNetwork.CurrentRoom.Name;
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                thismatchindex = PhotonNetwork.LocalPlayer.ActorNumber;
                SetRoomCustomProperty("MasterFrog", GManager.instance.set_playerselect.ToString());
                selectstage = UnityEngine.Random.Range(0, MultiStages.Length);
                SetRoomCustomProperty("MasterStage", selectstage.ToString());
            }
            else
            {
                thismatchindex = PhotonNetwork.LocalPlayer.ActorNumber;
                SetRoomCustomProperty(PhotonNetwork.LocalPlayer.ActorNumber.ToString()+ "Frog", GManager.instance.set_playerselect.ToString());
                object gameMode;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MasterStage", out gameMode))
                {
                    string tmpstage = gameMode.ToString();
                    selectstage = int.Parse(tmpstage);
                }
            }

            //ここからマッチとステージの準備・生成
            MultiStages[selectstage].stageall.SetActive(true);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                var tmpselectstage = UnityEngine.Random.Range(0, MultiStages[selectstage].masterstartpos.Length);
                Vector3 position = MultiStages[selectstage].masterstartpos[tmpselectstage].position;
                MultiStages[selectstage].currentplobj = PhotonNetwork.Instantiate(MultiStages[selectstage].player.name, position, MultiStages[selectstage].player.transform.rotation);
                MultiStages[selectstage].currentplobj.name = "Player";
            }
            else
            {
                var tmpselectstage = UnityEngine.Random.Range(0, MultiStages[selectstage].otherstartpos.Length);
                Vector3 position = MultiStages[selectstage].otherstartpos[tmpselectstage].position;
                MultiStages[selectstage].currentplobj = PhotonNetwork.Instantiate(MultiStages[selectstage].player.name, position, MultiStages[selectstage].player.transform.rotation);
                MultiStages[selectstage].currentplobj.name = "Player";
            }
            // ルームが満員になったら、以降そのルームへの参加を不許可にする
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                GManager.instance.ismatch = true;
                Invoke(nameof(MatchPlay), 0.3f);
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    //完全に人数(2人)揃ってマッチした時に対戦開始させる関数
    public void MatchPlay()
    {
        escapemacth.SetActive(false);
        if (audioSource && aoooose) audioSource.PlayOneShot(aoooose);
        if (GManager.instance.isEnglish == 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "対戦相手が見つかりました！";
        }
        else if (GManager.instance.isEnglish != 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "Opponent found!";
        }
        // 他のプレイヤーの一覧を取得
        Player[] otherPlayers = PhotonNetwork.PlayerList;
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            // 他のプレイヤーの名前をログに表示
            for (int q = 1; q < otherPlayers.Length;)
            {
                matchfrogname[q].text = otherPlayers[q].NickName;

                object gameMode;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(otherPlayers[q].ActorNumber.ToString() + "Frog", out gameMode))
                {
                    string tmpfrog = gameMode.ToString();
                    matchplayer_frogid[q] = int.Parse(tmpfrog);
                    matchfrogimg[q].sprite = GManager.instance.all_frog[matchplayer_frogid[q]].frog_image;
                }
                q++;
            }
        }
        else
        {
            object gameMode;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MasterFrog", out gameMode))
            {
                string tmpfrog = gameMode.ToString();
                matchplayer_frogid[0] = int.Parse(tmpfrog);
                matchfrogimg[0].sprite = GManager.instance.all_frog[matchplayer_frogid[0]].frog_image;
            }
        }
        StartCoroutine(CountStart());
    }
    IEnumerator CountStart()
    {
        yield return new WaitForSeconds(2f);
        if (GManager.instance.isEnglish == 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "5秒後に対戦が開始されます。声と心の準備を！";
        }
        else if (GManager.instance.isEnglish != 0)
        {
            currentstatus.fontSize = 12;
            currentstatus.text = "Match starts in 5 seconds. Prepare your voice and mind!";
        }
        yield return new WaitForSeconds(1f);
        if (GManager.instance.isEnglish == 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "4秒後に対戦が開始されます。声と心の準備を！";
        }
        else if (GManager.instance.isEnglish != 0)
        {
            currentstatus.fontSize = 12;
            currentstatus.text = "Match starts in 4 seconds. Prepare your voice and mind!";
        }
        yield return new WaitForSeconds(1f);
        if (GManager.instance.isEnglish == 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "3秒後に対戦が開始されます。声と心の準備を！";
        }
        else if (GManager.instance.isEnglish != 0)
        {
            currentstatus.fontSize = 12;
            currentstatus.text = "Match starts in 3 seconds. Prepare your voice and mind!";
        }
        yield return new WaitForSeconds(1f);
        if (GManager.instance.isEnglish == 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "2秒後に対戦が開始されます。声と心の準備を！";
        }
        else if (GManager.instance.isEnglish != 0)
        {
            currentstatus.fontSize = 12;
            currentstatus.text = "Match starts in 2 seconds. Prepare your voice and mind!";
        }
        yield return new WaitForSeconds(1f);
        if (GManager.instance.isEnglish == 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "1秒後に対戦が開始されます。声と心の準備を！";
        }
        else if (GManager.instance.isEnglish != 0)
        {
            currentstatus.fontSize = 12;
            currentstatus.text = "Match starts in 1 seconds. Prepare your voice and mind!";
        }
        yield return new WaitForSeconds(1f);
        if (GManager.instance.isEnglish == 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "対戦開始！";
        }
        else if (GManager.instance.isEnglish != 0)
        {
            currentstatus.fontSize = 14;
            currentstatus.text = "Match start!";
        }
        Instantiate(fadeindestroy, transform.position, transform.rotation);
        yield return new WaitForSeconds(1f);
        matchanim.SetInteger("trg", 2);
        _bgm.Stop();
        _bgm.clip = _defaultclip;
        _bgm.loop = true;
        _bgm.Play();
        Instantiate(fadeout, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.3f);
        GManager.instance.walktrg = true;
        yield return new WaitForSeconds(0.3f);
        Instantiate(missionstartUI, transform.position, transform.rotation);
    }
    //■マスサバから抜けつつタイトルに戻る
    public void OnExitedToMaster()
    {
        GManager.instance.multimode = false;
        GManager.instance.ismatch = false;
        nextscene = "title";
        if (PhotonNetwork.InRoom)
        {
            // ルームから抜ける
            PhotonNetwork.LeaveRoom();
        }
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        Instantiate(fadein, transform.position, transform.rotation);
        if (audioSource && mese) audioSource.PlayOneShot(mese);
        Invoke(nameof(TitleToChange), 1f);

    }
    private void TitleToChange()
    {
        SceneManager.LoadScene(nextscene);
    }
    public void OnExitedToMatch()
    {
        GManager.instance.ismatch = false;
        nextscene = "onlinebattle";
        if (PhotonNetwork.InRoom)
        {
            // ルームから抜ける
            PhotonNetwork.LeaveRoom();
        }
        Instantiate(fadeindestroy, transform.position, transform.rotation);
        if (audioSource && mese) audioSource.PlayOneShot(mese);
        Invoke(nameof(TitleToChange), 1f);

    }
    //意図時でマッチプレイ中終了した時完全に閉じられる前に敗北扱いにする
    private void OnApplicationQuit()
    {
        if (GManager.instance.ismatch)
        {
            GManager.instance.ismatch = false;
            PlayerPrefs.SetInt("MatchLose", 1);
            PlayerPrefs.Save();
        }
        PhotonNetwork.Disconnect();
        //後からタイトル画面に戻ってきたら負けたことを知らせるようにする。
    }

    // ルームのカスタムプロパティを設定する関数
    public void SetRoomCustomProperty(string key, object value)
    {
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties[key] = value;
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        }
    }
    // プレイヤーのカスタムプロパティを変更する関数
    public void SetPlayerCustomProperty(string key, object value)
    {
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties[key] = value;
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        }
    }
    public string GetOtherPlayerCustomProperty(string key)
    {
        // ローカルプレイヤー以外のプレイヤーを取得
        foreach (var player in PhotonNetwork.PlayerListOthers)
        {
            // カスタムプロパティを取得
            object customPropertyValue;
            if (player.CustomProperties.TryGetValue(key, out customPropertyValue))
            {
                return customPropertyValue.ToString();
            }
        }
        return "";
    }
    public string GetThisPlayerCustomProperty(string key)
    {
        object customPropertyValue = PhotonNetwork.LocalPlayer.CustomProperties[key];
        
        return customPropertyValue.ToString();
    }
    public string GetRoomCustomProperty(string key)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // ルームのカスタムプロパティを取得する例
            object gameMode;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out gameMode))
            {
                return gameMode.ToString();
            }
        }
        return "";
    }

    // 指定したActorNumberのプレイヤーのGameObjectを取得する関数
    public GameObject GetPlayerObjectByActorNumber(int actorNumber)
    {
        Player[] players = PhotonNetwork.PlayerList;

        foreach (Player player in players)
        {
            if (player.ActorNumber == actorNumber)
            {
                // 指定したActorNumberのプレイヤーが見つかった場合、そのプレイヤーのGameObjectを返す
                return GetPlayerGameObject(player);
            }
        }

        // 指定したActorNumberのプレイヤーが見つからなかった場合はnullを返す
        return null;
    }

    // PlayerからGameObjectを取得する関数
    private GameObject GetPlayerGameObject(Player player)
    {
        // PhotonViewがアタッチされたPrefabのインスタンスの場合は、PhotonViewを介してGameObjectを取得する
        PhotonView photonView = player.TagObject as PhotonView;
        if (photonView != null)
        {
            return photonView.gameObject;
        }

        // PhotonViewがアタッチされていない場合はnullを返す
        return null;
    }

    public int GetActorNumberByPlayerObject(GameObject playerObject)
    {
        PhotonView photonView = playerObject.GetComponent<PhotonView>();

        if (photonView != null)
        {
            Player player = GetPlayerByPhotonView(photonView);
            if (player != null)
            {
                // 指定したGameObjectと同じオブジェクトなPlayerが見つかった場合、そのPlayerのActorNumberを返す
                return player.ActorNumber;
            }
        }

        // 指定したGameObjectと同じオブジェクトなPlayerが見つからなかった場合は-1を返す
        return -1;
    }

    // PhotonViewからPlayerを取得する関数
    private Player GetPlayerByPhotonView(PhotonView photonView)
    {
        // PhotonViewがアタッチされたPrefabのインスタンスの場合は、ownerを介してPlayerを取得する
        if (photonView.IsMine)
        {
            return PhotonNetwork.LocalPlayer;
        }
        else
        {
            return photonView.Owner;
        }
    }
}