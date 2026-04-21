using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    public static DamagePopUp Create(Vector3 position, int damage, Color32 color, float size)
    {
        Transform damagePopUpTransform = Instantiate(MenuManager.Instance.DamagePopUp, position, Quaternion.identity);
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.Setup(damage, color, size);
        return damagePopUp;
    }

    public void Setup(int damage, Color32 color, float size)
    {
        Text = transform.GetComponent<TextMeshProUGUI>();
        Text.SetText("-" + damage.ToString());
        Text.color = color;
        TextColor = color;
        Text.fontSize = GameManager.Instance.PopUpSize * size;
        GetComponent<Canvas>().sortingOrder = 20;
    }

    private TextMeshProUGUI Text;
    private float TTK = 1f;
    private float TTD = 2f;
    private Color TextColor;

    private void Update()
    {
        transform.position += new Vector3(0, 1) * Time.deltaTime;
        TTK -= Time.deltaTime;
        if (TTK < 0)
        {
            TextColor.a -= TTD * Time.deltaTime;
            Text.color = TextColor;
            if (TextColor.a < 0) Destroy(gameObject);
        }
    }
}