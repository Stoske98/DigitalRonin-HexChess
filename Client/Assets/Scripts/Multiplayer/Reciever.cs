using System.IO;
using UnityEngine;
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
        NetworkManager.C_ON_TARGETABLE_ABILITY_RESPONESS += OnTargetableAbilityResponess;
        NetworkManager.C_ON_INSTANT_ABILITY_RESPONESS += OnInstantAbilityResponess;
    }

    public void UnSubscibe()
    {
        NetworkManager.C_ON_KEEP_ALIVE_RESPONESS -= OnKeepAliveResponess;
        NetworkManager.C_ON_WELCOME_RESPONESS -= OnWelcomeResponess;
        NetworkManager.C_ON_AUTH_RESPONESS -= OnAuthenticationResponess;
        NetworkManager.C_ON_SYNC_RESPONESS -= OnSyncResponess;
        NetworkManager.C_ON_MOVE_RESPONESS -= OnMoveResponess;
        NetworkManager.C_ON_ATTACK_RESPONESS -= OnAttackResponess;
        NetworkManager.C_ON_TARGETABLE_ABILITY_RESPONESS -= OnTargetableAbilityResponess;
        NetworkManager.C_ON_INSTANT_ABILITY_RESPONESS -= OnInstantAbilityResponess;
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
        NetworkManager.Instance.player.player_date = responess.player_data;

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

        Hex unit_hex = game.GetHex(responess.col,responess.row);
        Hex desired_hex = game.GetHex(responess.desired_col, responess.desired_row);

        Unit unit = unit_hex?.GetUnit();

        if (unit != null && unit.id == responess.unit_id && desired_hex != null)
        {
            MovementBehaviour movement_behaviour = unit.GetBehaviour<MovementBehaviour>() as MovementBehaviour;

            if (movement_behaviour != null && movement_behaviour.GetAvailableMoves(unit_hex).Contains(desired_hex))
                unit.Move(unit_hex, desired_hex);
        }

        //remove 
        game.EndTurn();
    }    
    private void OnAttackResponess(NetMessage message)
    {
        NetAttack responess = message as NetAttack;

        Game game = GameManager.Instance.game;

        Hex attacker_hex = game.GetHex(responess.attacker_col, responess.attacker_row);
        Hex target_hex = game.GetHex(responess.target_col, responess.target_row);

        Unit attacker = attacker_hex?.GetUnit();
        Unit target = target_hex?.GetUnit();

        if (attacker != null && attacker.id == responess.attacker_id && target != null && target.id == responess.target_id)
        {
            AttackBehaviour attack_behaviour = attacker.GetBehaviour<AttackBehaviour>() as AttackBehaviour;

            if(attack_behaviour != null && attack_behaviour.GetAttackMoves(attacker_hex).Contains(target_hex))
                attacker.Attack(target);
        }

        //remove 
        game.EndTurn();
    }
    private void OnTargetableAbilityResponess(NetMessage message)
    {
        NetTargetableAbilility responess = message as NetTargetableAbilility;

        Game game = GameManager.Instance.game;

        Hex unit_hex = game.GetHex(responess.col, responess.row);
        Hex desired_hex = game.GetHex(responess.desired_col, responess.desired_row);

        Unit unit = unit_hex?.GetUnit();

        if (unit != null && unit.id == responess.unit_id && desired_hex != null)
        {
            Ability ability = unit.GetBehaviour<Ability>(responess.key_code) as Ability;
            if (ability != null && ability is TargetableAbility targetable_ability && targetable_ability.GetAbilityMoves(unit_hex).Contains(desired_hex)) //check cd also when is it develop
                unit.UseAbility(targetable_ability, desired_hex);
        }

        //remove 
        game.EndTurn();
    }
    private void OnInstantAbilityResponess(NetMessage message)
    {
        NetInstantAbility responess = message as NetInstantAbility;

        Game game = GameManager.Instance.game;

        Hex unit_hex = game.GetHex(responess.col, responess.row);
        Unit unit = unit_hex?.GetUnit();
        if (unit != null && unit.id == responess.unit_id)
        {
            Ability ability = unit.GetBehaviour<Ability>(responess.key_code) as Ability;
            if (ability != null && ability is InstantleAbility instant_ability)//check cd also when is it develop
                unit.UseAbility(instant_ability, null);

        }

        //remove 
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
            //TO:DO receive 30 more message after fragment_index == total_fragments - true, i dont know why 
        }
    }
    private void CreateGameFromJson(string _json)
    {
        Game game = NetworkManager.Deserialize<ChallengeRoyaleGame>(_json);

        if (game != null)
        {
            foreach (var obj in game.objects)
                if (obj is Unit unit)
                {
                    game.units.Add(unit);
                    unit.RegisterEvents();
                    Spawner.CreateUnitGameObjects(unit);
                }

            foreach (Hex hex in game.map.hexes)
            {
                hex.SetNeighbors(game.map);
                hex.hex_mesh = hex.game_object.GetComponent<MeshRenderer>();
                hex.SetMaterial(GameManager.Instance.map_controller.field_material);
            }

            GameManager.Instance.game = game;
            GameManager.Instance.map_controller.SetMap(game.map);

            string json = NetworkManager.Serialize(game);
        }
    }


}
