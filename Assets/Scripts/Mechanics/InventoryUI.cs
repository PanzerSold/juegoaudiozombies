using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerShoot playerShoot;
    [SerializeField] private Button[] slotButtons;

    private TextMeshProUGUI[] slotLabels;

    private void Awake()
    {
        slotLabels = new TextMeshProUGUI[slotButtons.Length];
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slot = i;
            slotLabels[i] = slotButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            slotButtons[i].onClick.AddListener(() => playerShoot.EquipFromSlot(slot));
        }
    }

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
            Inventory.Item item = inventory.GetSlot(i);

            if (item.item == Inventory.itemType.Empty) slotLabels[i].text = "";
            else if (item.item == Inventory.itemType.Weapon) slotLabels[i].text = item.index.ToString();
            else slotLabels[i].text = item.index + "\nx" + item.amount;
        }
    }
}