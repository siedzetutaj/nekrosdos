using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviourSingleton<LineManager>
{
    [SerializeField] public List<LineController> lineControllers = new();

    public void addLineController(LineController controller)
    {
        lineControllers.Add(controller);
    }
    public void removeLineController(LineController controller)
    {
            lineControllers.Remove(controller);
    }
    public bool CheckIfLineExist(LineController controller)
    {
        foreach (LineController lineController in lineControllers)
        {
            if (lineController.connectionGuids.Equals(controller.connectionGuids))
            {
                return true;
            }
        }
        return false;
    }
}
