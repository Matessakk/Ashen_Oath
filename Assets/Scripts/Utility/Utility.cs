using UnityEngine;

public class Utility : MonoBehaviour
{

    public float hold = 0.8f;
    
    void Update()
    {
        if (Input.GetKey("escape"))  
        {
            Application.Quit();
        }    
    }
}
