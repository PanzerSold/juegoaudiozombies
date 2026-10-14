using UnityEngine.Events;

public static class UnityEvents
{
   public static UnityEvent<int> onHealthChanged = new UnityEvent<int>();
   public static UnityEvent<bool> onCooldownChanged = new UnityEvent<bool>();
}
