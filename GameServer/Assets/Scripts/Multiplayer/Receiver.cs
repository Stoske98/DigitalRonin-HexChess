﻿using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Receiver
{
    public void Subscibe()
    {
        NetworkManager.S_ON_KEEP_ALIVE_REQUEST += OnKeepAliveRequest;
        NetworkManager.S_ON_AUTH_REQUEST += OnAuthenticationRequest;
        NetworkManager.S_ON_SYNC_REQUEST += OnSyncRequest;
        NetworkManager.S_ON_SYNC_LOST_FRAGMENT_REQUEST += OnSyncLostFragmentRequest;
        NetworkManager.S_ON_ATTACK_REQUEST += OnAttackRequest;
        NetworkManager.S_ON_MOVE_REQUEST += OnMoveRequest;
        NetworkManager.S_ON_SINGLE_TARGET_ABILITY_REQUEST += OnSingleTargetableAbilityRequest;
        NetworkManager.S_ON_MULTIPLE_TARGETS_ABILITY_REQUEST += OnMultipleTargetableAbilityRequest;
        NetworkManager.S_ON_INSTANT_ABILITY_REQUEST += OnInstantAbilityRequest;
        NetworkManager.S_ON_UPGRADE_CLASS_REQUEST += OnUpgradeClassRequest;
    }


    public void UnSubscibe()
    {
        NetworkManager.S_ON_KEEP_ALIVE_REQUEST -= OnKeepAliveRequest;
        NetworkManager.S_ON_AUTH_REQUEST -= OnAuthenticationRequest;
        NetworkManager.S_ON_SYNC_REQUEST -= OnSyncRequest;
        NetworkManager.S_ON_SYNC_LOST_FRAGMENT_REQUEST -= OnSyncLostFragmentRequest;
        NetworkManager.S_ON_ATTACK_REQUEST -= OnAttackRequest;
        NetworkManager.S_ON_MOVE_REQUEST -= OnMoveRequest;
        NetworkManager.S_ON_SINGLE_TARGET_ABILITY_REQUEST -= OnSingleTargetableAbilityRequest;
        NetworkManager.S_ON_MULTIPLE_TARGETS_ABILITY_REQUEST -= OnMultipleTargetableAbilityRequest;
        NetworkManager.S_ON_INSTANT_ABILITY_REQUEST -= OnInstantAbilityRequest;
        NetworkManager.S_ON_UPGRADE_CLASS_REQUEST -= OnUpgradeClassRequest;
    }

    private void OnKeepAliveRequest(NetMessage message, Connection connection)
    {
    }
    private async void OnAuthenticationRequest(NetMessage message, Connection connection)
    {
        NetAuthentication request = message as NetAuthentication;

        PlayerData _player_data = await Database.AuthenticatePlayerAsync(request.device_id);

        if (_player_data != null)
        {
            Player player = NetworkManager.Instance.players[connection.Id];
            player.device_id = request.device_id;
            player.data = _player_data;

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
            if(!game.object_manager.IsObjectsWorking() && NetworkManager.Instance.players.TryGetValue(connection.Id, out Player player))
            {
                player.match_id = request.match_id;
                game.players.Add(player);

                if(game.players.Count == 2)
                    game.SendMessageToPlayers(new NetReconnect());

                string _json_game = NetworkManager.Serialize(game);

                if (!string.IsNullOrEmpty(_json_game))
                    SendGameToClientInFragments(connection, _json_game);
                else
                    UnityEngine.Debug.Log("OnSyncRequest: _jsong_game is empty");

            }           
        }
    }

    private void OnSyncLostFragmentRequest(NetMessage message, Connection connection)
    {
        NetSyncLostFragment request = message as NetSyncLostFragment;

        if (NetworkManager.Instance.games.TryGetValue(request.match_id, out Game game))
        {
            if (NetworkManager.Instance.players.TryGetValue(connection.Id, out Player player))
            {

                string _json_game = NetworkManager.Serialize(game);

                if (!string.IsNullOrEmpty(_json_game))
                    SendToClientFromLostFragment(connection, _json_game, request.fragment_index);
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
                    AttackBehaviour attack_behaviour = _attacker.GetBehaviour<AttackBehaviour>();
                    if(attack_behaviour != null && attack_behaviour.GetAttackMoves(_attacker_hex).Contains(_target_hex))
                    {
                        _attacker.Attack(_target);
                        game.SendMessageToPlayers(request);

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
                    MovementBehaviour movement_behaviour = _unit.GetBehaviour<MovementBehaviour>();
                    if (movement_behaviour != null && movement_behaviour.GetAvailableMoves(_hex).Contains(_desired_hex)) 
                    {
                        _unit.Move(_hex, _desired_hex);
                        game.SendMessageToPlayers(request);

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
                    Ability ability = _unit.GetBehaviour<Ability>(request.key_code);
                    if (ability != null && ability is TargetableAbility targetable_ability && ability.HasCooldownExpired() && targetable_ability.GetAbilityMoves(_hex).Count > 0 && targetable_ability.GetAbilityMoves(_hex).Contains(_desired_hex) && targetable_ability is ITargetableSingleHex)
                    {
                        _unit.UseSingleTargetableAbility(targetable_ability, _desired_hex);
                        game.SendMessageToPlayers(request);

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
                    Ability ability = _unit.GetBehaviour<Ability>(request.key_code);
                    if (ability != null && ability is TargetableAbility targetable_ability && ability.HasCooldownExpired() && targetable_ability is ITargetMultipleHexes)
                    {
                        bool is_valid_ability = true;

                        foreach (var desiredHex in _desired_hexes)
                        {
                            if (!targetable_ability.GetAbilityMoves(_hex).Contains(desiredHex) || targetable_ability.GetAbilityMoves(_hex).Count == 0)
                            {
                                is_valid_ability = false;
                                break;
                            }
                        }

                        if (is_valid_ability)
                        {
                            _unit.UseMultipleTargetableAbility(targetable_ability, _desired_hexes);
                            game.SendMessageToPlayers(request);
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
                    Ability ability = _unit.GetBehaviour<Ability>(request.key_code);
                    if (ability != null && ability is InstantleAbility instant_ability && ability.HasCooldownExpired())
                    {
                        _unit.UseInstantAbility(instant_ability);
                        game.SendMessageToPlayers(request);

                        game.action_done = true;
                    }
                }
            }
        }
    }

    private void OnUpgradeClassRequest(NetMessage message, Connection connection)
    {
        NetUpgradeClass request = message as NetUpgradeClass;

        if (NetworkManager.Instance.players.TryGetValue(connection.Id, out Player player))
        {
            if (NetworkManager.Instance.games.TryGetValue(player.match_id, out Game game))
            {
                ChallengeRoyaleGame ch_game = game as ChallengeRoyaleGame;
                if (request.class_type == ch_game.class_on_turn && !ch_game.object_manager.IsObjectsWorking() && ch_game.shard_controller.CanClassBeUpgraded(ch_game.class_on_turn, request.unit_type_to_upgrade))
                {
                    ch_game.shard_controller.UpgradeClass(ch_game.class_on_turn, request.unit_type_to_upgrade, ch_game.object_manager.objects.OfType<Unit>().ToList());
                    game.SendMessageToPlayers(request);
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
                fragment_data = fragment_data,
                game_data_lenght = gameData.Length
            };
            Sender.SendToClient_Reliable(connection.Id, responess);
        }
    }

    private void SendToClientFromLostFragment(Connection connection, string json_game, int last_fragment)
    {
        byte[] gameData = System.Text.Encoding.UTF8.GetBytes(json_game);

        int MaxFragmentSize = 1200;

        // Calculate the total number of fragments needed
        int totalFragments = Mathf.CeilToInt((float)gameData.Length / MaxFragmentSize);
        // Split the data into fragments and send the
        Debug.Log("START SENDING FROM: " + last_fragment);
        for (int fragmentIndex = last_fragment; fragmentIndex < totalFragments; fragmentIndex++)
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
                fragment_data = fragment_data
            };
            Sender.SendToClient_Reliable(connection.Id, responess);
        }
    }
}
