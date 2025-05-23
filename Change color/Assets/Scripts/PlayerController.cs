using Unity.Netcode;
using UnityEngine;

public class PlayerController2D : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;

    public NetworkVariable<Color> playerColor = new NetworkVariable<Color>(
        Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public float moveSpeed = 5f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        playerColor.OnValueChanged += OnColorChanged;

        // So o servidor pode asignar cor ao conectar
        if (IsServer)
        {
            Color newColor = ColorManager.Instance.GetRandomAvailableColor();
            playerColor.Value = newColor;
        }

        // Asegurar que se actualiza a cor local
        OnColorChanged(Color.white, playerColor.Value);
    }

    void Update()
    {
        if (!IsOwner) return;

        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestColorChangeServerRpc(playerColor.Value);
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(moveX, moveY, 0).normalized;

        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }

    [ServerRpc]
    void RequestColorChangeServerRpc(Color currentColor)
    {
        // Liberar a cor anterior
        ColorManager.Instance.ReleaseColor(currentColor);

        // Obter nova cor dispoñible
        Color newColor = ColorManager.Instance.GetNewColorExcluding(currentColor);

        // Actualizar valor en rede
        playerColor.Value = newColor;
    }

    void OnColorChanged(Color oldColor, Color newColor)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = newColor;
    }

    private new void OnDestroy()
    {
        playerColor.OnValueChanged -= OnColorChanged;

        // Só o servidor debe liberar a cor
        if (IsServer && ColorManager.Instance != null)
            ColorManager.Instance.ReleaseColor(playerColor.Value);
    }
}
