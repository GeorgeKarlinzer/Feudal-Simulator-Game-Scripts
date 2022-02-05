using UnityEngine;
using UnityEngine.Events;

public interface IStorage
{
    void AddAmount(string name, int value);

    int GetAmount(string name);

    int GetMaxAmount(string name);

    void AddListenerFindNewStorage(UnityAction<string[]> action);

    void RemoveListenerFindNewStorage(UnityAction<string[]> action);

    bool IsLocal { get; set; }

    Transform Destination { get; }
}
