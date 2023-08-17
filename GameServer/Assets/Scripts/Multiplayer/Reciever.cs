using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Reciever
{
    public void Subscibe()
    {
        NetworkManager.S_ON_AUTH_REQUEST += OnAuthenticationRequest;
        NetworkManager.S_ON_SYNC_REQUEST += OnSyncRequest;
        NetworkManager.S_ON_ATTACK_REQUEST += OnAttackRequest;
        NetworkManager.S_ON_MOVE_REQUEST += OnMoveRequest;
        NetworkManager.S_ON_SINGLE_TARGET_ABILITY_REQUEST += OnSingleTargetableAbilityRequest;
        NetworkManager.S_ON_MULTIPLE_TARGETS_ABILITY_REQUEST += OnMultipleTargetableAbilityRequest;
        NetworkManager.S_ON_INSTANT_ABILITY_REQUEST += OnInstantAbilityRequest;
    }


    public void UnSubscibe()
    {
        NetworkManager.S_ON_AUTH_REQUEST -= OnAuthenticationRequest;
        NetworkManager.S_ON_SYNC_REQUEST -= OnSyncRequest;
        NetworkManager.S_ON_ATTACK_REQUEST -= OnAttackRequest;
        NetworkManager.S_ON_MOVE_REQUEST -= OnMoveRequest;
        NetworkManager.S_ON_SINGLE_TARGET_ABILITY_REQUEST -= OnSingleTargetableAbilityRequest;
        NetworkManager.S_ON_MULTIPLE_TARGETS_ABILITY_REQUEST -= OnMultipleTargetableAbilityRequest;
        NetworkManager.S_ON_INSTANT_ABILITY_REQUEST -= OnInstantAbilityRequest;
    }
    private async void OnAuthenticationRequest(NetMessage message, Connection connection)
    {
        NetAuthentication request = message as NetAuthentication;

        PlayerData _player_data = await Database.AuthenticatePlayerAsync(request.device_id);

        if (_player_data != null)
        {
            Player player = NetworkManager.Instance.players[connection.Id];
            player.device_id = request.device_id;
            player.player_data = _player_data;

            NetAuthentication responess = new NetAuthentication()
            {
                player_data = _player_data,
            };

            Sender.SendToClient_Reliable(connection.Id, responess);
        }
        else
            NetworkManager.Instance.DisconnectPlayer(connection);
    }
    private void OnSyncRequest(NetMessage message, Connection connection)
    {
        NetSync request = message as NetSync;

        if (NetworkManager.Instance.games.TryGetValue(request.match_id, out Game game))
        {
            if(NetworkManager.Instance.players.TryGetValue(connection.Id, out Player player))
            {
                player.match_id = request.match_id;
                game.players.Add(player);

                string _json_game = NetworkManager.Serialize(game);
                File.WriteAllText("ChallengeRoyaleGame.json", _json_game);

                if (!string.IsNullOrEmpty(_json_game))
                    SendGameToClientInFragments(connection, _json_game);
                else
                    UnityEngine.Debug.Log("OnSyncRequest: _jsong_game is empty");

            }           
        }
    }
    private void OnAttackRequest(NetMessage message, Connection connection)
    {
        NetAttack request = message as NetAttack;

        if (NetworkManager.Instance.players.TryGetValue(connection.Id, out Player player))
        {
            if (NetworkManager.Instance.games.TryGetValue(player.match_id, out Game game))
            {
                Hex _attacker_hex = game.map.GetHex(request.attacker_col, request.attacker_row);
                Hex _target_hex  = game.map.GetHex(request.target_col, request.target_row);

                Unit _attacker = _attacker_hex?.GetUnit();
                Unit _target = _target_hex?.GetUnit();

                if (_attacker != null && !Stun.IsStuned(_attacker) && !Disarm.IsDissarmed(_attacker) && _attacker.id == request.attacker_id &&
                    _target != null && _target.id == request.target_id)
                {
                    AttackBehaviour attack_behaviour = _attacker.GetBehaviour<AttackBehaviour>() as AttackBehaviour;
                    if(attack_behaviour != null && attack_behaviour.GetAttackMoves(_attacker_hex).Contains(_target_hex))
                    {
                        _attacker.Attack(_target);

                        Sender.SendToClient_Reliable(connection.Id, request);
                        game.action_done = true;
                    }
                }
            }
        }
    }
    private void OnMoveRequest(NetMessage message, Connection connection)
    {
        NetMove request = message as NetMove;

        if (NetworkManager.Instance.players.TryGetValue(connection.Id, out Player player))
        {
            if (NetworkManager.Instance.games.TryGetValue(player.match_id, out Game game))
            {
                Hex _hex = game.map.GetHex(request.col, request.row);
                Hex _desired_hex = game.map.GetHex(request.desired_col, request.desired_row);

                Unit _unit = _hex?.GetUnit();

                if (_unit != null && !Stun.IsStuned(_unit) && _unit.id == request.unit_id && _desired_hex != null && _desired_hex.IsWalkable())
                {
                    MovementBehaviour movement_behaviour = _unit.GetBehaviour<MovementBehaviour>() as MovementBehaviour;
                    if (movement_behaviour != null && movement_behaviour.GetAvailableMoves(_hex).Contains(_desired_hex)) 
                    {
                        _unit.Move(_hex, _desired_hex);

                        Sender.SendToClient_Reliable(connection.Id, request);
                        game.action_done = true;
                    }                 
                }
            }
        }
    }
    private void OnSingleTargetableAbilityRequest(NetMessage message, Connection connection)
    {
        NetSingleTargetAbilility request = message as NetSingleTargetAbilility;

        if (NetworkManager.Instance.players.TryGetValue(connection.Id, out Player player))
        {
            if (NetworkManager.Instance.games.TryGetValue(player.match_id, out Game game))
            {
                Hex _hex = game.map.GetHex(request.col, request.row);
                Hex _desired_hex = game.map.GetHex(request.desired_col, request.desired_row);

                Unit _unit = _hex?.GetUnit();
                if (_unit != null && _unit.id == request.unit_id && _desired_hex != null)
                {
                    Ability ability = _unit.GetBehaviour<Ability>(request.key_code) as Ability;
                    if (ability != null && ability is TargetableAbility targetable_ability && ability.HasCooldownExpired() && targetable_ability.GetAbilityMoves(_hex).Contains(_desired_hex) && targetable_ability is ITargetableSingleHex)
                    {
                        _unit.UseSingleTargetableAbility(targetable_ability, _desired_hex);

                        Sender.SendToClient_Reliable(connection.Id, request);
                        game.action_done = true;
                    }
                }
            }
        }
    }
    private void OnMultipleTargetableAbilityRequest(NetMessage message, Connection connection)
    {
        NetMultipeTargetsAbilility request = message as NetMultipeTargetsAbilility;

        if (NetworkManager.Instance.players.TryGetValue(connection.Id, out Player player))
        {
            if (NetworkManager.Instance.games.TryGetValue(player.match_id, out Game game))
            {
                Hex _hex = game.map.GetHex(request.col, request.row);
                Unit _unit = _hex?.GetUnit();

                List<Hex> _desired_hexes = new List<Hex>();
                foreach (var coordinate in request.hexes_coordiantes)
                    _desired_hexes.Add(game.map.GetHex(coordinate.x,coordinate.y));

                if (_unit != null && _unit.id == request.unit_id && _desired_hexes.Count > 0)
                {
                    Ability ability = _unit.GetBehaviour<Ability>(request.key_code) as Ability;
                    if (ability != null && ability is TargetableAbility targetable_ability && ability.HasCooldownExpired() && targetable_ability is ITargetMultipleHexes)
                    {
                        bool is_valid_ability = true;

                        foreach (var desiredHex in _desired_hexes)
                        {
                            if (!targetable_ability.GetAbilityMoves(_hex).Contains(desiredHex))
                            {
                                is_valid_ability = false;
                                break;
                            }
                        }

                        if (is_valid_ability)
                        {
                            _unit.UseMultipleTargetableAbility(targetable_ability, _desired_hexes);
                            Sender.SendToClient_Reliable(connection.Id, request);
                            game.action_done = true;
                        }
                        else
                        {
                            // Handle the case where the ability is not valid
                        }
                    }
                }



            }
        }
    }
    private void OnInstantAbilityRequest(NetMessage message, Connection connection)
    {
        NetInstantAbility request = message as NetInstantAbility;

        if (NetworkManager.Instance.players.TryGetValue(connection.Id, out Player player))
        {
            if (NetworkManager.Instance.games.TryGetValue(player.match_id, out Game game))
            {
                Hex _hex = game.map.GetHex(request.col, request.row);

                Unit _unit = _hex?.GetUnit();

                if (_unit != null && _unit.id == request.unit_id)
                {
                    Ability ability = _unit.GetBehaviour<Ability>(request.key_code) as Ability;
                    if (ability != null && ability is InstantleAbility instant_ability && ability.HasCooldownExpired())
                    {
                        _unit.UseInstantAbility(instant_ability);

                        Sender.SendToClient_Reliable(connection.Id, request);
                        game.action_done = true;
                    }
                }
            }
        }
    }

    private void SendGameToClientInFragments(Connection connection, string json_game)
    {
        byte[] gameData = System.Text.Encoding.UTF8.GetBytes(json_game);

        int MaxFragmentSize = 1200;

        // Calculate the total number of fragments needed
        int totalFragments = Mathf.CeilToInt((float)gameData.Length / MaxFragmentSize);

        // Split the data into fragments and send the
        for (int fragmentIndex = 0; fragmentIndex < totalFragments; fragmentIndex++)
        {
            int offset = fragmentIndex * MaxFragmentSize;
            int size = Mathf.Min(gameData.Length - offset, MaxFragmentSize);

            byte[] fragment_data = new byte[size];
            Array.Copy(gameData, offset, fragment_data, 0, size);

            // Send the fragment as a string using the SendStringMessage method
            NetSync responess = new NetSync()
            {
                fragment_index = fragmentIndex,
                total_fragments = totalFragments - 1,
                game_data = fragment_data
            };
            Sender.SendToClient_Reliable(connection.Id, responess);
        }
    }
}
