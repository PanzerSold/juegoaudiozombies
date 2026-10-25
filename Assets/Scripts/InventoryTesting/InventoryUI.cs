using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI1 : MonoBehaviour
{
    [SerializeField] private Inventory1 inventory;
    [SerializeField] private PlayerShoot1 playerShoot;
    [SerializeField] private Button[] slotButtons;   // los 9, en orden (0 a 8)

    private TextMeshProUGUI[] slotLabels;

    // Corre una sola vez, la primera vez que se abre el panel
    private void Awake()
    {
        slotLabels = new TextMeshProUGUI[slotButtons.Length];
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slot = i; // copia, para que cada boton recuerde su propio numero
            slotLabels[i] = slotButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            slotButtons[i].onClick.AddListener(() => playerShoot.EquipFromSlot(slot));
        }
    }

    // Solo escucha al inventario mientras el panel esta abierto
    private void OnEnable()
    {
        inventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        inventory.OnInventoryChanged -= Refresh;
    }

    private void Refresh()
    {
        for (int i = 0; i < slotLabels.Length; i++)
        {
            Inventory1.Item item = inventory.GetSlot(i);

            if (item.item == Inventory1.itemType.Empty) slotLabels[i].text = "";
            else if (item.item == Inventory1.itemType.Weapon) slotLabels[i].text = item.index.ToString();
            else slotLabels[i].text = item.index + "\nx" + item.amount;
        }
    }
}