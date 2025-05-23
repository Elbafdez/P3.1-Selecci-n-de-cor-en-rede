using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager2D : MonoBehaviour
{
    public GameObject playerPrefab;
    public Button changeColorButton;

    private const int MaxPlayers = 6;
    private List<ulong> connectedPlayers = new List<ulong>();

    void Start()
    {
        // Engadir eventos
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

        // Comezar como host
        NetworkManager.Singleton.StartHost();

        // Contar o host como xogador
        ulong hostId = NetworkManager.Singleton.LocalClientId;
        connectedPlayers.Add(hostId);

        var player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(hostId);
    }

    void HandleClientConnected(ulong clientId)
    {
        if (connectedPlayers.Count >= MaxPlayers)
        {
            Debug.LogWarning("Número máximo de xogadores alcanzado. Rechazando conexión.");
            NetworkManager.Singleton.DisconnectClient(clientId);
            return;
        }

        // Só crear o obxecto para o cliente local
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            var player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }

        if (!connectedPlayers.Contains(clientId))
        {
            connectedPlayers.Add(clientId);
            Debug.Log($"Cliente conectado: {clientId}. Total xogadores: {connectedPlayers.Count}");
        }
    }

    void HandleClientDisconnected(ulong clientId)
    {
        if (connectedPlayers.Contains(clientId))
        {
            connectedPlayers.Remove(clientId);
            Debug.Log($"Cliente desconectado: {clientId}. Total xogadores: {connectedPlayers.Count}");
        }
    }
    
}
