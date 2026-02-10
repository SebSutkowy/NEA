using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TheDungeonGame;

enum MessageType : ushort
{
    Sync = 0,
    IdClient = 1,
    ClientJoin = 2,
    ClientDisconnect = 3,
    SendMessage = 4,
    SendPrivateMessage = 5,
    ListClients = 6, 
    SpawnPlayer = 7,
    UpdatePlayerPos = 8,
    PlayerDeath = 9,
    PlayerAttack = 10,
    RequestLogin = 11,
    LoginSuccess = 12,
    LoginFail = 13,
    AccountRegister = 14,
    AccountCreationSuccess = 15,
    AccountCreationFail = 16,
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
        string passedMessage, username, password, salt; 
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

            case MessageType.IdClient:
                id = int.Parse(splitMessage[1]);
                Network.SetLocalId(id);
                break;

            case MessageType.ClientJoin:
                id = int.Parse(splitMessage[1]);
                username = splitMessage[2];
                Network.OnClientJoin(id, username);
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
                Network.AddMessage($"[{Network.GetConnections[id]}] {passedMessage}");
                break;

            case MessageType.SendPrivateMessage:
                id = int.Parse(splitMessage[1]);
                targetId = int.Parse(splitMessage[2]);
                passedMessage = string.Join(" ", splitMessage, 3, splitMessage.Count() - 3); 
                if (Network.LocalId == targetId)
                {
                    Network.AddMessage($"[{Network.GetConnections[id]}->{Network.GetConnections[targetId]}] {passedMessage}");
                }
                else if (Network.GetMode() == ConnectionType.Host)
                {
                        Network.SendMessage(message, targetId); 
                }
                break;

            case MessageType.ListClients:
                id = int.Parse(splitMessage[1]);
                username = splitMessage[2];
                Network.AddClient(id, username);
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

            case MessageType.RequestLogin:
                // client --> server: sends login info to server
                id = int.Parse(splitMessage[1]);
                username = splitMessage[2];
                password = splitMessage[3];
                Network.VerifyLogin(id, username, password);
                break;

            case MessageType.LoginSuccess:
                // server --> client: client can join
                SceneManager.SwitchScene(SceneName.Lobby);
                username = splitMessage[1];
                Network.AddClient(Network.LocalId, username);
                break;

            case MessageType.LoginFail:
                // server --> client: client has to try again 
                Debug.WriteLine("Incorrect password or username, try again :(");
                break;

            case MessageType.AccountRegister:
                // client --> server: sends info to become registered
                id = int.Parse(splitMessage[1]);
                username = splitMessage[2];
                password = splitMessage[3];
                salt = splitMessage[4];
                Network.CreatePlayer(id, username, password, salt);
                break;

            case MessageType.AccountCreationSuccess:
                // server --> client: client can now log in
                Debug.WriteLine("Account created successfully");
                break;

            case MessageType.AccountCreationFail:
                // server --> client: username already taken
                Debug.WriteLine("Account failed to be created");
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

    public static string CreateClientIdMessage(int id) => $"{(ushort)MessageType.IdClient} {id}";

    public static string CreateClientJoinMessage(int id, string username) => $"{(ushort)MessageType.ClientJoin} {id} {username}";

    public static string CreateClientDisconnectMessage(int id) => $"{(ushort)MessageType.ClientDisconnect} {id}";

    public static string CreateSendMessage(int id, string message = "") => $"{(ushort)MessageType.SendMessage} {id} {message}";

    public static string CreateSendPrivateMessage(int id, int recipientId, string message = "") => $"{(ushort)MessageType.SendPrivateMessage} {id} {recipientId} {message}";

    public static string CreateListClientsMessage(int id, string username) => $"{(ushort)MessageType.ListClients} {id} {username}";

    public static string CreateSpawnPlayerMessage(int id) => $"{(ushort)MessageType.SpawnPlayer} {id}";

    public static string CreateUpdatePlayerPosMessage(int id, float x, float y, float rotation) => $"{(ushort)MessageType.UpdatePlayerPos} {id} {x} {y} {rotation}";

    public static string CreatePlayerDeathMessage(int id) => $"{(ushort)MessageType.PlayerDeath} {id}";

    public static string CreatePlayerAttackMessage(int id) => $"{(ushort)MessageType.PlayerAttack} {id}";

    public static string CreateRequestLoginMessage(int id, string username, string password) => $"{(ushort)MessageType.RequestLogin} {id} {username} {password}";

    public static string CreateLoginSuccessMessage(string username) => $"{(ushort)MessageType.LoginSuccess} {username}";

    public static string CreateLoginFailMessage() => $"{(ushort)MessageType.LoginFail}";

    public static string CreateRegisterMessage(int id, string username, string password, string salt) => $"{(ushort)MessageType.AccountRegister} {id} {username} {password} {salt}";

    public static string CreateAccountCreationSuccess() => $"{(ushort)MessageType.AccountCreationSuccess}";

    public static string CreateAccountCreationFail() => $"{(ushort)MessageType.AccountCreationFail}";

    //public static string CreateInteractionMessage(int id, int tick, TilemapName tilemap, Point tilemapPos) => $"{(ushort)MessageType.Interaction} {id} {tick} {(int)tilemap} {tilemapPos.X} {tilemapPos.Y}";
    //public static string CreateInteractionConfirmationMessage(int tick, TilemapName tilemap, Point tilemapPos, int seed) => $"{(ushort)MessageType.InteractionConfirmation} {tick} {(int)tilemap} {tilemapPos.X} {tilemapPos.Y} {seed}";

    //public static string CreatePlayerHealthChangeMessage(int id, int newHealth) => $"{(ushort)MessageType.PlayerHealthChange} {id} {newHealth}";

    //public static string CreateTrapToggleMessage(TilemapName tilemap, Point tilemapPos, TrapActivationStatus ActivatedStatus) => $"{(ushort)MessageType.TrapActivation} {(int)tilemap} {tilemapPos.X} {tilemapPos.Y} {(ushort)ActivatedStatus}";

    //public static string CreateEnteredBossRoomMessage() => $"{(ushort)MessageType.EnteredBossRoom}";

    //public static string CreateChangeTilemapMessage(TilemapName tilemap) => $"{(ushort)MessageType.ChangeTilemap} {(int)tilemap}";

    //public static string CreateUpdateEnemyMessage(Enemy enemy) => $"{(ushort)MessageType.UpdateEnemy} {enemy.Id} {(int)enemy.Type} {enemy.X} {enemy.Y} {enemy.Health} {enemy.MaxHealth}";

    //public static string CreatePlayerAttackMessage(int damage, int enemyId) => $"{(ushort)MessageType.PlayerAttacking} {damage} {enemyId}";
}
