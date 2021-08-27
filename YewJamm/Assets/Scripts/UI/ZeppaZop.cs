using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZeppaZop : MonoBehaviour
{

    public GameObject intro, gezz;
    public Text textZero, textOne;

    private void Start()
    {
        StartCoroutine(Gezeppazopzopt());
    }

    IEnumerator Gezeppazopzopt()
    {
        intro.SetActive(true);
        float t = 0;
        while(t <= 2.5f)
        {
            textZero.color = Color.Lerp(Color.clear, Color.black, t);
            textOne.color = Color.Lerp(Color.clear, Color.black, t-0.5f);
            t += Time.unscaledDeltaTime/2;
            yield return null;

        }
        intro.SetActive(false);
        gezz.SetActive(true);
    }

    public void Deactivaterino()
    {
        gezz.SetActive(false);
        Destroy(this);
    }

}
