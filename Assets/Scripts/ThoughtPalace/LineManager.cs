using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviourSingleton<LineManager>
{
    [SerializeField] private List<LineController> _lineControllers = new();

    public void addLineController(LineController controller)
    {
        _lineControllers.Add(controller);
    }
    public void removeLineController(LineController controller)
    {
            _lineControllers.Remove(controller);
    }
    public bool CheckIfLineExist(LineController controller)
    {
        foreach (LineController lineController in _lineControllers)
        {
            if (lineController.connectionGuids.Equals(controller.connectionGuids))
            {
                return true;
            }
        }
        return false;
    }
}
