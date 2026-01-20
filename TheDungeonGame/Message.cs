using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TheDungeonGame;

enum MessageType : ushort
{
    Sync = 0,
    ClientJoin = 1,
    ClientDisconnect = 2,
    SendMessage = 3,
    SendPrivateMessage = 4,
    ListClients = 5, 
    SpawnPlayer=6,
    UpdatePlayerPos=7,
    PlayerDeath=8,
    PlayerAttack=9,
}

static class Message
{
    
    public static void Decode(string message)
    {
        Debug.WriteLine($"Received: {message}");
        string[] splitMessage = message.Split(' ');
        ushort typeValue = (ushort)int.Parse(splitMessage[0]);
        bool isDefined = Enum.IsDefined(typeof(MessageType), typeValue);
        if (!isDefined)
            return;
        MessageType type = (MessageType)typeValue;


        int tick, id, targetId;
        string passedMessage; 
        float x, y, rotation;
        //Point tilemapPos = new Point();
        //string seed;
        //TilemapChange Change;
        //TilemapName tilemap;
        //EntityType entityType;

        switch (type)
        {
            case MessageType.Sync:
                Network.SetTick(int.Parse(splitMessage[1]));
                Network.AddMessage("Received Tick Sync");
                Network.SetTranslatedMessage("Tick sync");
                break;

            case MessageType.ClientJoin:
                id = int.Parse(splitMessage[1]);
                tick = int.Parse(splitMessage[2]);
                Network.OnClientJoin(id);
                Network.AddMessage($"{id} Joined");
                Network.SetTranslatedMessage("client joined");
                break;

            case MessageType.ClientDisconnect:
                id = int.Parse(splitMessage[1]);
                Network.OnClientDisconnect(id);
                Network.AddMessage($"{id} Disconnected");
                Network.SetTranslatedMessage("client disconnected");
                break;

            case MessageType.SendMessage:
                id = int.Parse(splitMessage[1]);
                if (Network.GetMode() == ConnectionType.Host)
                {
                    HashSet<int> excludedPeers = new HashSet<int> { id };
                    Network.SendExclusiveMessage(message, excludedPeers);
                }
                if (Network.IsMuted(id)) return;
                passedMessage = string.Join(" ", splitMessage, 2, splitMessage.Count() - 2);
                Network.AddMessage($"[{id}] {passedMessage}");
                break;

            case MessageType.SendPrivateMessage:
                id = int.Parse(splitMessage[1]);
                targetId = int.Parse(splitMessage[2]);
                passedMessage = string.Join(" ", splitMessage, 3, splitMessage.Count() - 3); 
                if (Network.LocalId == targetId)
                {
                    Network.AddMessage($"[{id}->{targetId}] {passedMessage}");
                }
                else if (Network.GetMode() == ConnectionType.Host)
                {
                        Network.SendMessage(message, targetId); 
                }
                break;

            case MessageType.ListClients:
                id = int.Parse(splitMessage[1]);
                Network.AddClient(id);
                Network.SetTranslatedMessage("client listing");
                break;
            case MessageType.SpawnPlayer:
                id = int.Parse(splitMessage[1]);
                PlayerManager.AddPlayer(id);
                if (Network.GetMode() == ConnectionType.Host)
                {
                    Network.SendMessage(message);
                }
                Network.SetTranslatedMessage("spawning player");
                break;
            case MessageType.UpdatePlayerPos:
                id = int.Parse(splitMessage[1]);
                x = float.Parse(splitMessage[2]);
                y = float.Parse(splitMessage[3]);
                rotation = float.Parse(splitMessage[4]);
                PlayerManager.UpdatePos(id, new Vector2(x, y), rotation);
                if (Network.GetMode() == ConnectionType.Host)
                {
                    HashSet<int> excluded = new HashSet<int>() { id };
                    Network.SendExclusiveMessage(message, excluded);
                }
                Network.SetTranslatedMessage($"updating {id} to {x}, {y}, {rotation}");
                break;
            case MessageType.PlayerDeath:
                id = int.Parse(splitMessage[1]);
                PlayerManager.RemovePlayer(id);
                Network.SetTranslatedMessage($"{id} died");
                break;
            case MessageType.PlayerAttack:
                id = int.Parse(splitMessage[1]);
                PlayerManager.PlayerAttacked(id);
                if (Network.GetMode() == ConnectionType.Host)
                {
                    HashSet<int> excluded = new HashSet<int>() { id };
                    Network.SendExclusiveMessage(message, excluded);
                }
                break;
                

                //    case MessageType.SpawnPlayer:
                //    case MessageType.PlayerState:
                //        id = int.Parse(splitMessage[1]);
                //        tick = int.Parse(splitMessage[2]);
                //        X = float.Parse(splitMessage[3]);
                //        Y = float.Parse(splitMessage[4]);
                //        StatePayload state = new StatePayload
                //        {
                //            Tick = tick,
                //            Position = new Vector2(X, Y)
                //        };
                //        playerManager.SetPlayerState(id, state);
                //        Client.Write($"Received player state: {{Tick: {tick} X: {X} Y: {Y}}}");
                //        break;

                //    case MessageType.PlayerSpawnRequest:
                //        id = int.Parse(splitMessage[1]);
                //        playerManager.CreatePlayer(id);
                //        Server.Write($"Received player spawn request for client {id}");
                //        break;

                //    case MessageType.PlayerInput:
                //        id = int.Parse(splitMessage[1]);
                //        tick = int.Parse(splitMessage[2]);
                //        X = float.Parse(splitMessage[3]);
                //        Y = float.Parse(splitMessage[4]);
                //        InputPayload input = new InputPayload
                //        {
                //            Tick = tick,
                //            Input = new Vector2(X, Y)
                //        };
                //        Server.Write($"Received an input of {tick} {X} {Y} from client {id}");
                //        playerManager.AddInput(id, input);
                //        break;

                //    case MessageType.Interaction:
                //        id = int.Parse(splitMessage[1]);
                //        tick = int.Parse(splitMessage[2]);
                //        tilemap = (TilemapName)int.Parse(splitMessage[3]);
                //        tilemapPos.X = int.Parse(splitMessage[4]);
                //        tilemapPos.Y = int.Parse(splitMessage[5]);
                //        Change = new TilemapChange()
                //        {
                //            Tick = tick,
                //            Position = tilemapPos
                //        };
                //        Dungeon.AddChanges(tilemap, Change, Mode.Server);
                //        Server.Write($"Received Interaction message position: {tilemapPos} in {tilemap}");
                //        break;

                //    case MessageType.InteractionConfirmation:
                //        tick = int.Parse(splitMessage[1]);
                //        tilemap = (TilemapName)int.Parse(splitMessage[2]);
                //        tilemapPos.X = int.Parse(splitMessage[3]);
                //        tilemapPos.Y = int.Parse(splitMessage[4]);
                //        seed = splitMessage[5];
                //        Change = new TilemapChange()
                //        {
                //            Tick = tick,
                //            Position = tilemapPos
                //        };
                //        Dungeon.AddChanges(tilemap, Change, Mode.Client);
                //        Client.Write($"Received Interaction confirmation position: {tilemapPos}");
                //        break;

                //    case MessageType.PlayerHealthChange:
                //        id = int.Parse(splitMessage[1]);
                //        int newHealth = int.Parse(splitMessage[2]);
                //        Client.PlayerManager.SetPlayerHealth(id, newHealth);
                //        Client.Write($"Received new health message: {newHealth}");
                //        break;

                //    case MessageType.TrapActivation:
                //        tilemap = (TilemapName)int.Parse(splitMessage[1]);
                //        tilemapPos.X = int.Parse(splitMessage[2]);
                //        tilemapPos.Y = int.Parse(splitMessage[3]);
                //        TrapActivationStatus status = (TrapActivationStatus) ushort.Parse(splitMessage[4]);
                //        if (status == TrapActivationStatus.Inactive)
                //            Dungeon.AddTile(tilemap, tilemapPos, TileType.Trap);
                //        else
                //            Dungeon.AddTile(tilemap, tilemapPos, TileType.ActiveTrap);
                //        Client.Write($"Received new Trap toggle message: {tilemapPos}");
                //        break;

                //    case MessageType.EnteredBossRoom:
                //        break;

                //    case MessageType.ChangeTilemap:
                //        tilemap = (TilemapName)int.Parse(splitMessage[1]);
                //        Client.PlayerManager.ResetPositions();
                //        Client.Write($"Received tilemap change: {Dungeon.ActiveTilemapName} -> {tilemap}");
                //        Dungeon.ChangeTilemap(tilemap);
                //        break;

                //    case MessageType.UpdateEnemy:
                //        id = int.Parse(splitMessage[1]);
                //        entityType = (EntityType)int.Parse(splitMessage[2]);
                //        X = float.Parse(splitMessage[3]);
                //        Y = float.Parse(splitMessage[4]);
                //        health = int.Parse(splitMessage[5]);
                //        int maxHealth = int.Parse(splitMessage[6]);
                //        Client.Write($"Received {entityType.ToString()} Update message");

                //        if (Dungeon.Enemies.ContainsKey(id))
                //        {
                //            Dungeon.Enemies[id].Position = new Vector2(X, Y);
                //            Dungeon.Enemies[id].SetHealth(health);
                //            break;
                //        }
                //        Dungeon.Enemies[id] = entityType switch
                //        {
                //            EntityType.Boss => new Boss1(Camera.EntityAssets[entityType], X, Y, Camera.EntityAssets[entityType].Width, Camera.EntityAssets[entityType].Height, 0, maxHealth),
                //            EntityType.Totem => new Totem(Camera.EntityAssets[entityType], X, Y, Camera.EntityAssets[entityType].Width, Camera.EntityAssets[entityType].Height, maxHealth)
                //        };
                //        break;

                //    case MessageType.PlayerAttacking:
                //        int damage = int.Parse(splitMessage[1]);
                //        int enemyId = int.Parse(splitMessage[2]);
                //        if (Dungeon.Enemies.ContainsKey(enemyId))
                //            Dungeon.Enemies[enemyId].TakeDamage(damage);
                //        Server.Write($"Received Player Attack: -{damage}HP -> {Dungeon.Enemies[enemyId].Type}");
                //        break;

        }
    }

    public static string CreateSyncMessage(int tick) => $"{(ushort)MessageType.Sync} {tick}";

    public static string CreateClientJoinMessage(int id, int tick) => $"{(ushort)MessageType.ClientJoin} {id} {tick}";

    public static string CreateClientDisconnectMessage(int id) => $"{(ushort)MessageType.ClientDisconnect} {id}";

    public static string CreateSendMessage(int id, string message = "") => $"{(ushort)MessageType.SendMessage} {id} {message}";

    public static string CreateSendPrivateMessage(int id, int recipientId, string message = "") => $"{(ushort)MessageType.SendPrivateMessage} {id} {recipientId} {message}";

    public static string CreateListClientsMessage(int id) => $"{(ushort)MessageType.ListClients} {id}";

    public static string CreateSpawnPlayerMessage(int id) => $"{(ushort)MessageType.SpawnPlayer} {id}";

    public static string CreateUpdatePlayerPosMessage(int id, float x, float y, float rotation) => $"{(ushort)MessageType.UpdatePlayerPos} {id} {x} {y} {rotation}";

    public static string CreatePlayerDeathMessage(int id) => $"{(ushort)MessageType.PlayerDeath} {id}";

    public static string CreatePlayerAttackMessage(int id) => $"{(ushort)MessageType.PlayerAttack} {id}";

    ////public static string CreatePlayerSpawnMessage(int id, int tick, Vector2 position) => $"{(ushort)MessageType.SpawnPlayer} {id} {tick} {position.X} {position.Y}";

    //public static string CreatePlayerSpawnRequestMessage(int id) => $"{(ushort)MessageType.PlayerSpawnRequest} {id}";

    //public static string CreatePlayerStateMessage(int id, StatePayload state) => $"{(ushort)MessageType.PlayerState} {id} {state.Tick} {state.Position.X} {state.Position.Y}";

    //public static string CreatePlayerInputMessage(int id, InputPayload input) => $"{(ushort)MessageType.PlayerInput} {id} {input.Tick} {input.Input.X} {input.Input.Y}";

    //public static string CreateInteractionMessage(int id, int tick, TilemapName tilemap, Point tilemapPos) => $"{(ushort)MessageType.Interaction} {id} {tick} {(int)tilemap} {tilemapPos.X} {tilemapPos.Y}";
    //public static string CreateInteractionConfirmationMessage(int tick, TilemapName tilemap, Point tilemapPos, int seed) => $"{(ushort)MessageType.InteractionConfirmation} {tick} {(int)tilemap} {tilemapPos.X} {tilemapPos.Y} {seed}";

    //public static string CreatePlayerHealthChangeMessage(int id, int newHealth) => $"{(ushort)MessageType.PlayerHealthChange} {id} {newHealth}";

    //public static string CreateTrapToggleMessage(TilemapName tilemap, Point tilemapPos, TrapActivationStatus ActivatedStatus) => $"{(ushort)MessageType.TrapActivation} {(int)tilemap} {tilemapPos.X} {tilemapPos.Y} {(ushort)ActivatedStatus}";

    //public static string CreateEnteredBossRoomMessage() => $"{(ushort)MessageType.EnteredBossRoom}";

    //public static string CreateChangeTilemapMessage(TilemapName tilemap) => $"{(ushort)MessageType.ChangeTilemap} {(int)tilemap}";

    //public static string CreateUpdateEnemyMessage(Enemy enemy) => $"{(ushort)MessageType.UpdateEnemy} {enemy.Id} {(int)enemy.Type} {enemy.X} {enemy.Y} {enemy.Health} {enemy.MaxHealth}";

    //public static string CreatePlayerAttackMessage(int damage, int enemyId) => $"{(ushort)MessageType.PlayerAttacking} {damage} {enemyId}";
}
