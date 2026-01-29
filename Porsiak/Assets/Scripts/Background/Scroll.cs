using UnityEngine;

public class Scroll : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * GameManager.Instance.GetScrollSpeed() * Time.deltaTime);
    }
}
