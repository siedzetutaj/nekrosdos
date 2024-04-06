using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class TPInformationNode : TPNode
{
    public override void Initialize(string nodeName, TPGraphView dsGraphView, Vector2 position)
    {
        base.Initialize(nodeName, dsGraphView, position);

        ThoughtType = TPThoughtType.Information;

    }

    public override void Draw()
    {
        base.Draw();

        /* OUTPUT CONTAINER */

        Port outputPort = this.CreatePort("Next", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);

        outputPort.userData = NextThoughts;

        outputContainer.Add(outputPort);

        defaultBackgroundColor = Color.cyan;
        mainContainer.style.backgroundColor = defaultBackgroundColor;

        RefreshExpandedState();
    }
}
