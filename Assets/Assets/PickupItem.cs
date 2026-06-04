using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public bool destroyOnPickup = true;

    public void Pick(ThirdPersonPlayerController player)
    {
        if (player != null)
        {
            player.PlayPickUp();
        }

        if (destroyOnPickup)
        {
            Destroy(gameObject, 0.5f);
        }
    }
}