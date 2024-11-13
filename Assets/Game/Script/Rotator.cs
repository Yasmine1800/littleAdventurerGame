using UnityEngine;

public class Rotator : MonoBehaviour
{
    
    public float Speed = 80f;

    void Update()
    {
        transform.Rotate(new Vector3(0f, Speed * Time.deltaTime, 0f), Space.World);
    }
}
