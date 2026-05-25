using UnityEngine;
using TMPro;
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float destroyText = 0.8f;
    

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

        if(Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, 
                                    Camera.main.transform.rotation * Vector3.up);
        }
    }
}
