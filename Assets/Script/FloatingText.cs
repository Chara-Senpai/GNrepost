using UnityEngine;
using TMPro;
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float floatSpeed = 0.5f;
    [SerializeField] private float destroyText = 1.0f;
    

    public void SetText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
        }

        Destroy(gameObject, destroyText);
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        Camera gameCamera = Camera.main;

        if (gameCamera == null)
        {
            gameCamera = GameObject.FindObjectOfType<Camera>();
        }

        if (gameCamera != null)
        {
            transform.LookAt(transform.position + gameCamera.transform.rotation * Vector3.forward,
                            gameCamera.transform.rotation * Vector3.up);
        }
    }
}
