using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
public class PhotonAuth : MonoBehaviourPunCallbacks
{
    public void AuthenticateWithPhoton(string sessionTicket)
    {
        PhotonNetwork.AuthValues = new AuthenticationValues
        {
            AuthType = CustomAuthenticationType.Custom,
            UserId = sessionTicket // Use PlayFab SessionTicket as UserId
        };
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Connected Successfully!");
        // Proceed to join a lobby or room
    }

    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.LogError($"Photon Authentication Failed: {debugMessage}");
    }
}
