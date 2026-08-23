using UnityEngine;

public class rotator : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        // Rotate the object around Z-axis
        transform.Rotate(new Vector3(0, 0, 30)* Time.deltaTime);
    }
}
