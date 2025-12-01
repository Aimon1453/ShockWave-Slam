using UnityEngine;

public interface IUnlockable
{
    bool TryUnlock(int keyID);
}