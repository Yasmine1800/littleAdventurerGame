using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    
    public float HorizontalInput;
    public float VerticalInput;

    public bool MouseButtonDown;
    public bool SpaceKeyDown;

    // Update is called once per frame
    void Update()
    {
        if( !MouseButtonDown && Time.timeScale != 0)
        {
            MouseButtonDown = Input.GetMouseButtonDown(0);
        }

        if( !SpaceKeyDown && Time.timeScale != 0)
        {
            SpaceKeyDown = Input.GetKeyDown(KeyCode.Space);
        }

        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
    }

    private void onDisable()
    {
        ClearCache();
    }

    public void ClearCache(){
        MouseButtonDown = false;
        SpaceKeyDown = false;
        HorizontalInput = 0;
        VerticalInput = 0;
    }
}
