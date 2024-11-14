using UnityEngine;

public class MomentumVisualizer : MonoBehaviour
{
    public CustomRB rb;
    public State state;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<CustomRB>();
    }

    // Update is called once per frame
    void Update()
    {
        state = rb.state;
        Debug.DrawLine(rb.cube.position, state.velocity, Color.green);
    }
}