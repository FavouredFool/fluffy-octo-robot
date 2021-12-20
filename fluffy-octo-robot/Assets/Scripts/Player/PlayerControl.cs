using Unity.Netcode;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    [SerializeField]
    private float walkSpeed = 0.05f;

    [SerializeField]
    private Vector2 defaultPositionRange = new Vector2(-4, 4);

    [SerializeField]
    private NetworkVariable<float> forwardBackPosition = new NetworkVariable<float>();

    [SerializeField]
    private NetworkVariable<float> leftRightPosition = new NetworkVariable<float>();

    [HideInInspector]
    public HexCell activeCell = null;

    [HideInInspector]
    public int maxWalkHeight;

    private BattleSystem battleSystem;

    // client caching
    private float oldForwardBackPosition;
    private float oldLeftRightPosition;

    private void Awake()
    {
        maxWalkHeight = 1;
    }

    private void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
        transform.position = new Vector3(
            Random.Range(defaultPositionRange.x, defaultPositionRange.y),
            0,
            Random.Range(defaultPositionRange.x, defaultPositionRange.y)
        );
    }

    private void Update()
    {
        if (IsServer && battleSystem.GetState().Equals(GameState.PLAYERTURN))
        {
            UpdateServer();
        }

        if (IsClient && battleSystem.GetState().Equals(GameState.PLAYERTURN))
        {
            UpdateClient();
        }
    }

    private void UpdateServer()
    {
        transform.position = new Vector3(
            transform.position.x + leftRightPosition.Value,
            transform.position.y,
            transform.position.z + forwardBackPosition.Value
        );
    }

    private void UpdateClient()
    {
        float forwardBackward = 0;
        float leftRight = 0;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            forwardBackward += walkSpeed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            leftRight -= walkSpeed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            forwardBackward -= walkSpeed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            leftRight += walkSpeed;
        }

        if (oldForwardBackPosition != forwardBackward || oldLeftRightPosition != leftRight)
        {
            oldForwardBackPosition = forwardBackward;
            oldLeftRightPosition = leftRight;

            UpdateClientPositionServerRpc(forwardBackward, leftRight);
        }
    }

    [ServerRpc]
    private void UpdateClientPositionServerRpc(float forwardBackward, float leftRight)
    {
        forwardBackPosition.Value = forwardBackward;
        leftRightPosition.Value = leftRight;
    }
}