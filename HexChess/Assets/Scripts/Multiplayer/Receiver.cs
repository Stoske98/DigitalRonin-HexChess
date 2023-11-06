using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Riptide;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Receiver
{
    private MemoryStream received_game_data_stream = new MemoryStream();
    private Timer poll_ticket_timer;
    private bool is_polling = false;
    private Timer sync_timer;
    private bool is_sync = false;
    private Timer end_turn_timer;

    private int last_fragment = - 1;
    private bool stop_receiving_data = false;
    public void SubscribeMainMenu()
    {
        NetworkManager.C_ON_KEEP_ALIVE_RESPONESS += OnKeepAliveResponess;
        NetworkManager.C_ON_WELCOME_RESPONESS += OnWelcomeResponess;
        NetworkManager.C_ON_AUTH_RESPONESS += OnAuthenticationResponess;
        NetworkManager.C_ON_GAME_EXIST_RESPONESS += OnGameExistResponess;
        NetworkManager.C_ON_LOGIN_RESPONESS += OnLoginResponess;
        NetworkManager.C_ON_CHANGE_NICKNAME_RESPONESS += OnChangeNicknameResponess;
        NetworkManager.C_ON_CREATE_TICKET_RESPONESS += OnCreateTicketResponess;
        NetworkManager.C_ON_FIND_MATCH_RESPONESS += OnFindMatchResponess;
        NetworkManager.C_ON_STOP_MATCH_FINDING_RESPONESS += OnStopMatchFindingResponess;
        NetworkManager.C_ON_ACCEPT_MATCH_RESPONESS += OnAcceptMatchResponess;
        NetworkManager.C_ON_DECLINE_MATCH_RESPONESS += OnDeclineMatchResponess;
        NetworkManager.C_ON_MATCH_CREATED_RESPONESS += OnMatchCreatedResponess;
        NetworkManager.C_ON_CREATE_LOBY_RESPONESS += OnCreateLobyResponess;
        NetworkManager.C_ON_JOIN_LOBY_RESPONESS += OnJoinLobyResponess;

    }

    public void UnSubscribeMainMenu()
    {
        NetworkManager.C_ON_KEEP_ALIVE_RESPONESS -= OnKeepAliveResponess;
        NetworkManager.C_ON_WELCOME_RESPONESS -= OnWelcomeResponess;
        NetworkManager.C_ON_AUTH_RESPONESS -= OnAuthenticationResponess;
        NetworkManager.C_ON_GAME_EXIST_RESPONESS -= OnGameExistResponess;
        NetworkManager.C_ON_LOGIN_RESPONESS -= OnLoginResponess;
        NetworkManager.C_ON_CHANGE_NICKNAME_RESPONESS -= OnChangeNicknameResponess;
        NetworkManager.C_ON_CREATE_TICKET_RESPONESS -= OnCreateTicketResponess;
        NetworkManager.C_ON_FIND_MATCH_RESPONESS -= OnFindMatchResponess;
        NetworkManager.C_ON_STOP_MATCH_FINDING_RESPONESS -= OnStopMatchFindingResponess;
        NetworkManager.C_ON_ACCEPT_MATCH_RESPONESS -= OnAcceptMatchResponess;
        NetworkManager.C_ON_DECLINE_MATCH_RESPONESS -= OnDeclineMatchResponess;
        NetworkManager.C_ON_MATCH_CREATED_RESPONESS -= OnMatchCreatedResponess;
        NetworkManager.C_ON_CREATE_LOBY_RESPONESS -= OnCreateLobyResponess;
        NetworkManager.C_ON_JOIN_LOBY_RESPONESS -= OnJoinLobyResponess;
    }

    public void SubscibeGame()
    {
        NetworkManager.C_ON_KEEP_ALIVE_RESPONESS += OnKeepAliveResponess;
        NetworkManager.C_ON_WELCOME_RESPONESS += OnWelcomeResponess;
        NetworkManager.C_ON_AUTH_RESPONESS += OnAuthenticationResponess;
        NetworkManager.C_ON_MOVE_RESPONESS += OnMoveResponess;
        NetworkManager.C_ON_ATTACK_RESPONESS += OnAttackResponess;
        NetworkManager.C_ON_SINGLE_TARGET_ABILITY_RESPONESS += OnSingleTargetableAbilityResponess;
        NetworkManager.C_ON_MULTIPLE_TARGETS_ABILITY_RESPONESS += OnMultipleTargetableAbilityResponess;
        NetworkManager.C_ON_INSTANT_ABILITY_RESPONESS += OnInstantAbilityResponess;
        NetworkManager.C_ON_END_TURN_RESPONESS += OnEndTurnResponess;
        NetworkManager.C_ON_UPGRADE_CLASS_RESPONESS += OnUpgradeClassResponess;
        NetworkManager.C_ON_RECONNECT_RESPONESS += OnInGameReconnectResponess;
        NetworkManager.C_ON_DISCONNECT_RESPONESS += OnInGameDisconnectResponess;
        NetworkManager.C_ON_END_GAME_RESPONESS += OnEndGameResponess;
    }

    public void UnSubscibeGame()
    {
        NetworkManager.C_ON_KEEP_ALIVE_RESPONESS -= OnKeepAliveResponess;
        NetworkManager.C_ON_WELCOME_RESPONESS -= OnWelcomeResponess;
        NetworkManager.C_ON_AUTH_RESPONESS -= OnAuthenticationResponess;
        NetworkManager.C_ON_MOVE_RESPONESS -= OnMoveResponess;
        NetworkManager.C_ON_ATTACK_RESPONESS -= OnAttackResponess;
        NetworkManager.C_ON_SINGLE_TARGET_ABILITY_RESPONESS -= OnSingleTargetableAbilityResponess;
        NetworkManager.C_ON_MULTIPLE_TARGETS_ABILITY_RESPONESS -= OnMultipleTargetableAbilityResponess;
        NetworkManager.C_ON_INSTANT_ABILITY_RESPONESS -= OnInstantAbilityResponess;
        NetworkManager.C_ON_END_TURN_RESPONESS -= OnEndTurnResponess;
        NetworkManager.C_ON_UPGRADE_CLASS_RESPONESS -= OnUpgradeClassResponess;
        NetworkManager.C_ON_RECONNECT_RESPONESS -= OnInGameReconnectResponess;
        NetworkManager.C_ON_DISCONNECT_RESPONESS -= OnInGameDisconnectResponess;
        NetworkManager.C_ON_END_GAME_RESPONESS += OnEndGameResponess;
    }

    private async void OnEndGameResponess(NetMessage message)
    {
        NetEndGame responess = message as NetEndGame;
        GameUI.Instance.SetAWinner(responess.winner);
        Client client = NetworkManager.Instance.Client;
        client.Disconnect();
        NetworkManager.Instance.Reciever.UnSubscibeGame();
        await Task.Delay(3000);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnInGameDisconnectResponess(NetMessage message)
    {
        GameUI.Instance.player_disconnect_from_game.SetActive(true);
    }

    private void OnInGameReconnectResponess(NetMessage message)
    {
        GameUI.Instance.player_disconnect_from_game.SetActive(false);
    }

    private void OnKeepAliveResponess(NetMessage message)
    {
        Sender.SendToServer_Reliable(message);
    }
    private void OnWelcomeResponess(NetMessage message)
    {
        NetAuthentication request = new NetAuthentication();
        request.device_id = NetworkManager.Instance.player.device_id;

        Sender.SendToServer_Reliable(request);
    }
    private void OnAuthenticationResponess(NetMessage message)
    {
        NetAuthentication responess = message as NetAuthentication;
        NetworkManager.Instance.player.data = responess.player_data;

        received_game_data_stream?.Dispose();
        received_game_data_stream = null;
        if (!is_sync)
        {
            NetworkManager.C_ON_SYNC_RESPONESS += OnSyncResponess;
            is_sync = true;

            sync_timer = new Timer(5000);
            sync_timer.Elapsed += SyncGame;
            sync_timer.AutoReset = true;
            sync_timer.Start();
        }

    }
    private void SyncGame(object sender, ElapsedEventArgs e)
    {
        NetSync request = new NetSync()
        {
            match_id = NetworkManager.Instance.player.match_id,
        };
        Sender.SendToServer_Reliable(request);
    }
    private void GameStartSync()
    {
        is_sync = false;
        if (sync_timer != null)
        {
            sync_timer.Stop();
            sync_timer.Dispose();
        }
    }
    private async  void OnGameExistResponess(NetMessage message)
    {
        NetGameExist responess = message as NetGameExist;

        NetworkManager.Instance.player.match_id = responess.match_id;
        Client client = NetworkManager.Instance.Client;
        client.Disconnect();
        NetworkManager.Instance.Reciever.UnSubscribeMainMenu();
        await Task.Delay(3000);
        NetworkManager.Instance.Reciever.SubscibeGame();
        client.Connect($"162.19.241.52:{responess.port}");
        //client.Connect($"{responess.ip_address}:{responess.port}");
    }
    private async void OnMatchCreatedResponess(NetMessage message)
    {
        NetMatchCreated responess = message as NetMatchCreated;

        NetworkManager.Instance.player.match_id = responess.match_id;
        Client client = NetworkManager.Instance.Client;
        client.Disconnect();
        MainMenuUIManager.Instance.BothPlayerAcceptMatch();
        NetworkManager.Instance.Reciever.UnSubscribeMainMenu();
        await Task.Delay(3000);
        NetworkManager.Instance.Reciever.SubscibeGame();
        client.Connect($"162.19.241.52:{responess.port}");
        //client.Connect($"{responess.ip_address}:{responess.port}");
    }//162.19.241.52
    private void OnLoginResponess(NetMessage message)
    {
        NetLogin responess = message as NetLogin;
        NetworkManager.Instance.player.data = responess.player_data;
        MainMenuUIManager.Instance.SetPlayerInfo(NetworkManager.Instance.player);
        MainMenuUIManager.Instance.connecting_panel.SetActive(false);
    }
    private void OnChangeNicknameResponess(NetMessage message)
    {
        NetChangeNickname responess = message as NetChangeNickname;
        MainMenuUIManager.Instance.ChangeNickname(responess.nickname);
    }
    private void OnCreateTicketResponess(NetMessage message)
    {
        NetCreateTicket responess = message as NetCreateTicket;
        NetworkManager.Instance.player.data.class_type = responess.class_type;
        MainMenuUIManager.Instance.StartFindingMatchResponess(responess.class_type);

        if (!is_polling)
        {
            is_polling = true;

            poll_ticket_timer = new Timer(6000);
            poll_ticket_timer.Elapsed += PollTicket;
            poll_ticket_timer.AutoReset = true;
            poll_ticket_timer.Start();
        }
    }
    private void OnCreateLobyResponess(NetMessage message)
    {
        NetCreateLoby responess = message as NetCreateLoby;
        NetworkManager.Instance.player.data.class_type = responess.class_type;
        Debug.Log(responess.loby_id);
        MainMenuUIManager.Instance.OnCreateLobyResponess(responess.loby_id);
    }
    private void OnJoinLobyResponess(NetMessage message)
    {
        throw new NotImplementedException();
    }
    private void OnFindMatchResponess(NetMessage message)
    {
        StopPolling();
        MainMenuUIManager.Instance.StopFindingMatchResponess();

        NetFindMatch responess = message as NetFindMatch;
        PlayerData opponent_data = responess.enemy_data;

        MainMenuUIManager.Instance.ShowMatchFound(opponent_data);
    }

    private void OnDeclineMatchResponess(NetMessage message)
    {
        NetDeclineMatch responess = message as NetDeclineMatch;
        MainMenuUIManager.Instance.OnDeclineMatchResponess();
    }

    private void OnAcceptMatchResponess(NetMessage message)
    {
        NetAcceptMatch responess = message as NetAcceptMatch;
        MainMenuUIManager.Instance.OnAcceptMatchResponess(responess.class_type);

    }

    private void OnStopMatchFindingResponess(NetMessage message)
    {
        StopPolling();
        MainMenuUIManager.Instance.StopFindingMatchResponess();
    }
    private void PollTicket(object sender, ElapsedEventArgs e)
    {
        NetFindMatch request = new NetFindMatch();
        Sender.SendToServer_Reliable(request);
    }

    private void StopPolling()
    {
        is_polling = false;
        if (poll_ticket_timer != null)
        {
            poll_ticket_timer.Stop();
            poll_ticket_timer.Dispose();
        }
    }
    private void OnSyncResponess(NetMessage message)
    {
        if (is_sync)
        {
            stop_receiving_data = false;
            last_fragment = -1;
            stop_receiving_data = false;
            GameStartSync();
        }
        if(!stop_receiving_data)
        {
            NetSync responess = message as NetSync;
            OnReceiveByteArrayFragment(responess);

        }
        /* Debug.Log("CURRENT FRAGMENT: " + responess.fragment_index);
         Debug.Log("LAST FRAGMENT: " + last_fragment);*/
        /* if ((responess.fragment_index - last_fragment) == 1)
         {
           //  Debug.Log("RECEIVE FRAGMENT: " + responess.fragment_index);
             waiting_for_next_fragment = false;
             OnReceiveByteArrayFragment(responess);
         }
         else
         {
             if(!waiting_for_next_fragment)
             {
                 waiting_for_next_fragment = true;
                 Debug.Log("LOST FRAGMENT: " + (last_fragment + 1));
                 NetSyncLostFragment request = new NetSyncLostFragment()
                 {
                     match_id = NetworkManager.Instance.player.match_id,
                     fragment_index = last_fragment + 1,
                 };

                 Sender.SendToServer_Reliable(request);

             }
         }*/
    }
    private void OnMoveResponess(NetMessage message)
    {
        NetMove responess = message as NetMove;

        Game game = GameManager.Instance.game;

        Hex unit_hex = game.map.GetHex(responess.col,responess.row);
        Hex desired_hex = game.map.GetHex(responess.desired_col, responess.desired_row);

        Unit unit = unit_hex?.GetUnit();

        if (unit != null && unit.id == responess.unit_id && desired_hex != null)
        {
            MovementBehaviour movement_behaviour = unit.GetBehaviour<MovementBehaviour>();

            if (movement_behaviour != null && movement_behaviour.GetAvailableMoves(unit_hex).Contains(desired_hex))
            {
                unit.Move(unit_hex, desired_hex);
                GameUI.Instance.log_bar.action_bar.SetMovementnBar(unit);
            }
        }

        game.class_on_turn = ClassType.None;
    }    
    private void OnAttackResponess(NetMessage message)
    {
        NetAttack responess = message as NetAttack;

        Game game = GameManager.Instance.game;

        Hex attacker_hex = game.map.GetHex(responess.attacker_col, responess.attacker_row);
        Hex target_hex = game.map.GetHex(responess.target_col, responess.target_row);

        Unit attacker = attacker_hex?.GetUnit();
        Unit target = target_hex?.GetUnit();

        if (attacker != null && attacker.id == responess.attacker_id && target != null && target.id == responess.target_id)
        {
            AttackBehaviour attack_behaviour = attacker.GetBehaviour<AttackBehaviour>();

            if(attack_behaviour != null && attack_behaviour.GetAttackMoves(attacker_hex).Contains(target_hex))
            {
                attacker.Attack(target);
                GameUI.Instance.log_bar.action_bar.SetAttacknBar(attacker,target);
            }
        }

        game.class_on_turn = ClassType.None;
    }
    private void OnSingleTargetableAbilityResponess(NetMessage message)
    {
        NetSingleTargetAbilility responess = message as NetSingleTargetAbilility;

        Game game = GameManager.Instance.game;

        Hex unit_hex = game.map.GetHex(responess.col, responess.row);
        Hex desired_hex = game.map.GetHex(responess.desired_col, responess.desired_row);

        Unit unit = unit_hex?.GetUnit();

        if (unit != null && unit.id == responess.unit_id && desired_hex != null)
        {
            Ability ability = unit.GetBehaviour<Ability>(responess.key_code);
            if (ability != null && ability is TargetableAbility targetable_ability && targetable_ability.GetAbilityMoves(unit_hex).Count > 0 && targetable_ability.GetAbilityMoves(unit_hex).Contains(desired_hex) && targetable_ability is ITargetableSingleHex)
            {
                unit.UseSingleTargetableAbility(targetable_ability, desired_hex);

                Unit target_unit = desired_hex.GetUnit();
                if (target_unit != null)
                    GameUI.Instance.log_bar.action_bar.SetSpellBar(unit, targetable_ability, target_unit);
                else
                    GameUI.Instance.log_bar.action_bar.SetSpellBar(unit, targetable_ability);
            }    
        }

        game.class_on_turn = ClassType.None;
    }
    private void OnMultipleTargetableAbilityResponess(NetMessage message)
    {
        NetMultipeTargetsAbilility responess = message as NetMultipeTargetsAbilility;

        Game game = GameManager.Instance.game;

        Hex unit_hex = game.map.GetHex(responess.col, responess.row);
        Unit unit = unit_hex?.GetUnit();

        List<Hex> _desired_hexes = new List<Hex>();
        foreach (var coordinate in responess.hexes_coordiantes)
            _desired_hexes.Add(game.map.GetHex(coordinate.x, coordinate.y));

        if (unit != null && unit.id == responess.unit_id && _desired_hexes.Count > 0)
        {
            Ability ability = unit.GetBehaviour<Ability>(responess.key_code);
            if (ability != null && ability is TargetableAbility targetable_ability && targetable_ability is ITargetMultipleHexes)
            {
                unit.UseMultipleTargetableAbility(targetable_ability, _desired_hexes);
                GameUI.Instance.log_bar.action_bar.SetSpellBar(unit, targetable_ability);
            }
        }

        game.class_on_turn = ClassType.None;
    }
    private void OnInstantAbilityResponess(NetMessage message)
    {
        NetInstantAbility responess = message as NetInstantAbility;

        Game game = GameManager.Instance.game;

        Hex unit_hex = game.map.GetHex(responess.col, responess.row);
        Unit unit = unit_hex?.GetUnit();
        if (unit != null && unit.id == responess.unit_id)
        {
            Ability ability = unit.GetBehaviour<Ability>(responess.key_code);
            if (ability != null && ability is InstantleAbility instant_ability)
            {
                unit.UseInstantAbility(instant_ability);
                GameUI.Instance.log_bar.action_bar.SetSpellBar(unit, instant_ability);
            }

        }

        game.class_on_turn = ClassType.None;
    }

    private void OnEndTurnResponess(NetMessage message)
    {
        NetEndTurn responess = message as NetEndTurn;

        Game game = GameManager.Instance.game;
        if(!game.object_manager.IsObjectsWorking())
        {
            game.class_on_turn = responess.class_on_turn;
            game.EndTurn();
        }else
            GameManager.Instance.gameObject.LeanDelayedCall(0.25f, CheckIsTurnEnded(game, responess));
    }
    private Action CheckIsTurnEnded(Game game, NetEndTurn responess)
    {
        return () =>
        {
            if (!game.object_manager.IsObjectsWorking())
            {
                game.class_on_turn = responess.class_on_turn;
                game.EndTurn();
            }else
                GameManager.Instance.gameObject.LeanDelayedCall(0.25f, CheckIsTurnEnded(game,responess));
        };
    }
    /* private void CheckIsTurnEnded(object sender, ElapsedEventArgs e)
{
    Game game = GameManager.Instance.game;
    if (!game.object_manager.IsObjectsWorking())
    {
        if (end_turn_timer != null)
        {
            game.class_on_turn = class_on_turn;
            game.EndTurn();
            end_turn_timer.Stop();
            end_turn_timer.Dispose();
            class_on_turn = ClassType.None;

            Debug.Log("TURN ENDED: " + Time.time);
        }
    }
}*/

    private void OnUpgradeClassResponess(NetMessage message)
    {
        NetUpgradeClass responess = message as NetUpgradeClass;
        ChallengeRoyaleGame ch_game = GameManager.Instance.game as ChallengeRoyaleGame;

        ClassLevelController  level_controller =  ch_game.shard_controller.UpgradeClass(responess.class_type, responess.unit_type_to_upgrade, ch_game.object_manager.objects.OfType<Unit>().ToList());
        ch_game.game_events.OnClassUpgraded_Global?.Invoke(level_controller, responess.class_type, responess.unit_type_to_upgrade);
    }
    private void OnReceiveByteArrayFragment(NetSync responess)
    {
        int fragment_index = responess.fragment_index;
        int total_fragments = responess.total_fragments;
        long position = (long)fragment_index * 1200;
        if(fragment_index - last_fragment != 1)
        {
            Debug.Log("ERROR ON " + fragment_index);
        }
        last_fragment = fragment_index;
        if (received_game_data_stream == null)
            received_game_data_stream = new MemoryStream();

        received_game_data_stream.Seek(position, SeekOrigin.Begin); 
        received_game_data_stream.Write(responess.fragment_data, 0, responess.fragment_data.Length);

        if (responess.game_data_lenght == received_game_data_stream.Length)
        {
            Debug.Log("GAME RECEIVED !!!");
            byte[] originalMessage = received_game_data_stream.ToArray();
            string json = System.Text.Encoding.UTF8.GetString(originalMessage);
            stop_receiving_data = true;
            CreateGameFromJson(json);
            received_game_data_stream.Dispose();
            received_game_data_stream = null;

            NetworkManager.C_ON_SYNC_RESPONESS -= OnSyncResponess;
        }
    }
    private void CreateGameFromJson(string _json)
    {
        Game game = NetworkManager.Deserialize<ChallengeRoyaleGame>(_json);

        if (game != null)
        {
            GameManager.Instance.game = game;
            game.object_manager.Init();
            game.RegisterEvents();

            foreach (var sub in game.object_manager.subscribes)
            {
                sub.RegisterEvents();
            }
            foreach (var obj in game.object_manager.objects)
            {
                /*if (obj is ISubscribe subscriber)
                    subscriber.RegisterEvents();*/

                if (obj is Unit unit && unit.ccs.Count > 0)
                    foreach (var cc in unit.ccs)
                        cc.Init();

                IObject.ObjectVisibility(obj,obj.visibility);
            }

            foreach (Hex hex in game.map.hexes)
                hex.Init();

            GameManager.Instance.map_controller.SetMap(game.map);
            GameUI.Instance.Subscribe(game);
            GameUI.Instance.SetPlayerTurn(game.class_on_turn);
            GameUI.Instance.log_bar.gralls_controller.SetCounter(ClassType.Light, GameManager.Instance.game.death_light);
            GameUI.Instance.log_bar.gralls_controller.SetCounter(ClassType.Dark, GameManager.Instance.game.death_dark);

            ChallengeRoyaleGame ch_game = game as ChallengeRoyaleGame;
            if(ch_game != null && ch_game.is_activated)
            {
                GameUI.Instance.SetChallengeRoyaleMove(ch_game.GetChalllengeRoyaleTurn());
                GameUI.Instance.ActiveChallengeRoyale(true);
            }

            GameManager.Instance.main_canvas.enabled = false;
            GameManager.Instance.game_canvas.enabled = true;

            string json = NetworkManager.Serialize(game);

           // File.WriteAllText("ChallengeRoyaleGame.json", json);
        }
    }


}
