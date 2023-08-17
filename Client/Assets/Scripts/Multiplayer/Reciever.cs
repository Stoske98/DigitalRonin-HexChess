using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class Reciever
{
    private MemoryStream received_game_data_stream = new MemoryStream();
    public void Subscibe()
    {
        NetworkManager.C_ON_KEEP_ALIVE_RESPONESS += OnKeepAliveResponess;
        NetworkManager.C_ON_WELCOME_RESPONESS += OnWelcomeResponess;
        NetworkManager.C_ON_AUTH_RESPONESS += OnAuthenticationResponess;
        NetworkManager.C_ON_SYNC_RESPONESS += OnSyncResponess;
        NetworkManager.C_ON_MOVE_RESPONESS += OnMoveResponess;
        NetworkManager.C_ON_ATTACK_RESPONESS += OnAttackResponess;
        NetworkManager.C_ON_SINGLE_TARGET_ABILITY_RESPONESS += OnSingleTargetableAbilityResponess;
        NetworkManager.C_ON_MULTIPLE_TARGETS_ABILITY_RESPONESS += OnMultipleTargetableAbilityResponess;
        NetworkManager.C_ON_INSTANT_ABILITY_RESPONESS += OnInstantAbilityResponess;
        NetworkManager.C_ON_END_TURN_RESPONESS += OnEndTurnResponess;
    }

    public void UnSubscibe()
    {
        NetworkManager.C_ON_KEEP_ALIVE_RESPONESS -= OnKeepAliveResponess;
        NetworkManager.C_ON_WELCOME_RESPONESS -= OnWelcomeResponess;
        NetworkManager.C_ON_AUTH_RESPONESS -= OnAuthenticationResponess;
        NetworkManager.C_ON_SYNC_RESPONESS -= OnSyncResponess;
        NetworkManager.C_ON_MOVE_RESPONESS -= OnMoveResponess;
        NetworkManager.C_ON_ATTACK_RESPONESS -= OnAttackResponess;
        NetworkManager.C_ON_SINGLE_TARGET_ABILITY_RESPONESS -= OnSingleTargetableAbilityResponess;
        NetworkManager.C_ON_MULTIPLE_TARGETS_ABILITY_RESPONESS -= OnMultipleTargetableAbilityResponess;
        NetworkManager.C_ON_INSTANT_ABILITY_RESPONESS -= OnInstantAbilityResponess;
        NetworkManager.C_ON_END_TURN_RESPONESS -= OnEndTurnResponess;
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
        NetworkManager.Instance.player.player_data = responess.player_data;

        received_game_data_stream?.Dispose();
        received_game_data_stream = null;

        NetworkManager.Instance.StartSyncGameData();
    }
    private void OnSyncResponess(NetMessage message)
    {
        NetworkManager.Instance.StopSyncGameData();
        NetSync responess = message as NetSync;
        OnReceiveByteArrayFragment(responess);
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
            MovementBehaviour movement_behaviour = unit.GetBehaviour<MovementBehaviour>() as MovementBehaviour;

            if (movement_behaviour != null && movement_behaviour.GetAvailableMoves(unit_hex).Contains(desired_hex))
                unit.Move(unit_hex, desired_hex);
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
            AttackBehaviour attack_behaviour = attacker.GetBehaviour<AttackBehaviour>() as AttackBehaviour;

            if(attack_behaviour != null && attack_behaviour.GetAttackMoves(attacker_hex).Contains(target_hex))
                attacker.Attack(target);
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
            Ability ability = unit.GetBehaviour<Ability>(responess.key_code) as Ability;
            if (ability != null && ability is TargetableAbility targetable_ability && targetable_ability.GetAbilityMoves(unit_hex).Contains(desired_hex) && targetable_ability is ITargetableSingleHex)
            {
                unit.UseSingleTargetableAbility(targetable_ability, desired_hex);
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
            Ability ability = unit.GetBehaviour<Ability>(responess.key_code) as Ability;
            if (ability != null && ability is TargetableAbility targetable_ability && targetable_ability is ITargetMultipleHexes)
            {
                unit.UseMultipleTargetableAbility(targetable_ability, _desired_hexes);
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
            Ability ability = unit.GetBehaviour<Ability>(responess.key_code) as Ability;
            if (ability != null && ability is InstantleAbility instant_ability)
                unit.UseInstantAbility(instant_ability);

        }

        game.class_on_turn = ClassType.None;
    }

    private void OnEndTurnResponess(NetMessage message)
    {
        NetEndTurn responess = message as NetEndTurn;

        Game game = GameManager.Instance.game;
        game.class_on_turn = responess.class_on_turn;
        game.EndTurn();    
    }

    private void OnReceiveByteArrayFragment(NetSync responess)
    {
        int fragment_index = responess.fragment_index;
        int total_fragments = responess.total_fragments;

        if (received_game_data_stream == null)
            received_game_data_stream = new MemoryStream();

        received_game_data_stream.Write(responess.game_data, 0, responess.game_data.Length);

        if (fragment_index == total_fragments)
        {
            byte[] originalMessage = received_game_data_stream.ToArray();
            string json = System.Text.Encoding.UTF8.GetString(originalMessage);

            CreateGameFromJson(json);
            received_game_data_stream.Dispose();
            received_game_data_stream = null;
        }
    }
    private void CreateGameFromJson(string _json)
    {
        Game game = NetworkManager.Deserialize<ChallengeRoyaleGame>(_json);

        if (game != null)
        {
            GameManager.Instance.game = game;
            game.object_manager.Init();

            foreach (var obj in game.object_manager.objects)
            {
                if (obj is ISubscribe subscriber)
                    subscriber.RegisterEvents();

                IObject.ObjectVisibility(obj,obj.visibility);
            }

            foreach (Hex hex in game.map.hexes)
            {
                hex.hex_mesh = hex.game_object.GetComponent<MeshRenderer>();
                hex.SetMaterial(GameManager.Instance.map_controller.field_material);
            }

            GameManager.Instance.game = game;
            GameManager.Instance.map_controller.SetMap(game.map);

            string json = NetworkManager.Serialize(game);

            File.WriteAllText("ChallengeRoyaleGame.json", json);
        }
    }


}
