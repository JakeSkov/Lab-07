using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Lab02a_PlayerControl : NetworkBehaviour 
{
    struct PlayerState
    {
        public float posX, posY, posZ;
        public float rotX, rotY, rotZ;
    }

    [SyncVar]
    PlayerState state;

    void InitState()
    {
        state = new PlayerState
        {
            posX = -119f,
            posY = 165.08f,
            posZ = -924f,
            rotX = 0f,
            rotY = 0f,
            rotZ = 0f
        };
    }

    void SyncState()
    {
        transform.position = new Vector3(state.posX, state.posY, state.posZ);
        transform.rotation = Quaternion.Euler(state.rotX, state.rotY, state.rotZ);
    }

	// Use this for initialization
	void Start () 
    {
        InitState();
        SyncState();
	}

    PlayerState Move(PlayerState prev, KeyCode newKey)
    {
        float deltaX = 0, deltaY = 0, deltaZ = 0;
        float deltaRotationY = 0;
        
        switch (newKey)
        {
            case KeyCode.Q:
                deltaX = -0.5f;
                break;
            case KeyCode.S:
                deltaZ = -0.5f;
                break;
            case KeyCode.E:
                deltaX = 0.5f;
                break;
            case KeyCode.W:
                deltaZ = 0.5f;
                break;
            case KeyCode.A:
                deltaRotationY = -1f;
                break;
            case KeyCode.D:
                deltaRotationY = 1f;
                break;
        }

        return new PlayerState
        {
            posX = deltaX + prev.posX,
            posY = deltaY + prev.posY,
            posZ = deltaZ + prev.posZ,
            rotX = prev.rotX,
            rotY = deltaRotationY + prev.rotY,
            rotZ = prev.rotZ
        };
    }

	// Update is called once per frame
	void Update () 
    {
        if (isLocalPlayer)
        {
            KeyCode[] possibleKeys = { KeyCode.A, KeyCode.S, KeyCode.W, KeyCode.D, KeyCode.Q, KeyCode.E, KeyCode.Space };

            foreach (KeyCode possibleKey in possibleKeys)
            {
                if (!Input.GetKey(possibleKey))
                {
                    continue;
                }

                CmdMoveOnServer(possibleKey);
            }
        }

        SyncState();
	}

    [Command]
    void CmdMoveOnServer(KeyCode pressedKey)
    {
        state = Move(state, pressedKey);
    }
}
