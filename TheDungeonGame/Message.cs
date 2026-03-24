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
    GenerateWorld = 17,
    CompletedPuzzle = 18,
    CompleteDungeon = 19
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


        int tick, id, targetId, tilemapId;
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
                tilemapId = int.Parse(splitMessage[2]);
                PlayerManager.AddPlayer(id, tilemapId);
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
                tilemapId = int.Parse(splitMessage[5]);
                PlayerManager.UpdatePos(id, new Vector2(x, y), rotation, tilemapId);
                if (Network.GetMode() == ConnectionType.Host)
                {
                    HashSet<int> excluded = new HashSet<int>() { id };
                    Network.SendExclusiveMessage(message, excluded);
                }
                Network.SetTranslatedMessage($"updating {id} to {x}, {y}, {rotation} in {tilemapId}");
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

            case MessageType.GenerateWorld:
                int size = int.Parse(splitMessage[1]);
                int seed = int.Parse(splitMessage[2]);
                Dungeon.GenerateMap(size, seed);
                SceneManager.SwitchScene(SceneName.Game);
                break;

            case MessageType.CompletedPuzzle:
                string name = splitMessage[1];
                tilemapId = int.Parse(splitMessage[2]);
                Dungeon.CompletedPuzzle(name, tilemapId, false);
                if(Network.GetMode() == ConnectionType.Host)
                {
                    Network.SendMessage(message);
                }
                break;
            case MessageType.CompleteDungeon:
                Dungeon.CompleteDungeon();
                break;
        }
    }

    public static string CreateSyncMessage(int tick) => $"{(ushort)MessageType.Sync} {tick}";

    public static string CreateClientIdMessage(int id) => $"{(ushort)MessageType.IdClient} {id}";

    public static string CreateClientJoinMessage(int id, string username) => $"{(ushort)MessageType.ClientJoin} {id} {username}";

    public static string CreateClientDisconnectMessage(int id) => $"{(ushort)MessageType.ClientDisconnect} {id}";

    public static string CreateSendMessage(int id, string message = "") => $"{(ushort)MessageType.SendMessage} {id} {message}";

    public static string CreateSendPrivateMessage(int id, int recipientId, string message = "") => $"{(ushort)MessageType.SendPrivateMessage} {id} {recipientId} {message}";

    public static string CreateListClientsMessage(int id, string username) => $"{(ushort)MessageType.ListClients} {id} {username}";

    public static string CreateSpawnPlayerMessage(int id, int tilemapId) => $"{(ushort)MessageType.SpawnPlayer} {id} {tilemapId}";

    public static string CreateUpdatePlayerPosMessage(int id, float x, float y, float rotation, int tilemap) => $"{(ushort)MessageType.UpdatePlayerPos} {id} {x} {y} {rotation} {tilemap}";

    public static string CreatePlayerDeathMessage(int id) => $"{(ushort)MessageType.PlayerDeath} {id}";

    public static string CreatePlayerAttackMessage(int id) => $"{(ushort)MessageType.PlayerAttack} {id}";

    public static string CreateRequestLoginMessage(int id, string username, string password) => $"{(ushort)MessageType.RequestLogin} {id} {username} {password}";

    public static string CreateLoginSuccessMessage(string username) => $"{(ushort)MessageType.LoginSuccess} {username}";

    public static string CreateLoginFailMessage() => $"{(ushort)MessageType.LoginFail}";

    public static string CreateRegisterMessage(int id, string username, string password, string salt) => $"{(ushort)MessageType.AccountRegister} {id} {username} {password} {salt}";

    public static string CreateAccountCreationSuccess() => $"{(ushort)MessageType.AccountCreationSuccess}";

    public static string CreateAccountCreationFail() => $"{(ushort)MessageType.AccountCreationFail}";

    public static string CreateGenerateWorldMessage(int size, int seed) => $"{(ushort)MessageType.GenerateWorld} {size} {seed}";

    public static string CreatePuzzleCompletionMessage(string playerName, int tilemapId) => $"{(ushort)MessageType.CompletedPuzzle} {playerName} {tilemapId}";

    public static string CreateDungeonCompletionMessage() => $"{(ushort)MessageType.CompleteDungeon}";
    //public static string CreateInteractionMessage(int id, int tick, TilemapName tilemap, Point tilemapPos) => $"{(ushort)MessageType.Interaction} {id} {tick} {(int)tilemap} {tilemapPos.X} {tilemapPos.Y}";
    //public static string CreateInteractionConfirmationMessage(int tick, TilemapName tilemap, Point tilemapPos, int seed) => $"{(ushort)MessageType.InteractionConfirmation} {tick} {(int)tilemap} {tilemapPos.X} {tilemapPos.Y} {seed}";

    //public static string CreatePlayerHealthChangeMessage(int id, int newHealth) => $"{(ushort)MessageType.PlayerHealthChange} {id} {newHealth}";

    //public static string CreateTrapToggleMessage(TilemapName tilemap, Point tilemapPos, TrapActivationStatus ActivatedStatus) => $"{(ushort)MessageType.TrapActivation} {(int)tilemap} {tilemapPos.X} {tilemapPos.Y} {(ushort)ActivatedStatus}";

    //public static string CreateEnteredBossRoomMessage() => $"{(ushort)MessageType.EnteredBossRoom}";

    //public static string CreateChangeTilemapMessage(TilemapName tilemap) => $"{(ushort)MessageType.ChangeTilemap} {(int)tilemap}";

    //public static string CreateUpdateEnemyMessage(Enemy enemy) => $"{(ushort)MessageType.UpdateEnemy} {enemy.Id} {(int)enemy.Type} {enemy.X} {enemy.Y} {enemy.Health} {enemy.MaxHealth}";

    //public static string CreatePlayerAttackMessage(int damage, int enemyId) => $"{(ushort)MessageType.PlayerAttacking} {damage} {enemyId}";
} 