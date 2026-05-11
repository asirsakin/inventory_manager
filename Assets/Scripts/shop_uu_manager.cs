using UnityEngine;
using UnityEngine.UI;

public class shop_uu_manager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InputField[] inp;
    public Text[] tx,tx1;
    public Text extra;
    public InputField EXTT;
    public string[] defolt;
    public string extra_data;
    public Image image_back;
    // Update is called once per frame
    public void change_name()
    {
        for (int i = 0; i < inp.Length; i++)
        {
            if(!string.IsNullOrEmpty(inp[i].text))
            {
                PlayerPrefs.SetString("col_name" + i, inp[i].text);

            }

        }
        if (!string.IsNullOrEmpty(EXTT.text))
        {
            PlayerPrefs.SetString("col_ext", EXTT.text);

        }

    }
    public void FixedUpdate()
    {
        for (int i = 0; i < inp.Length; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("col_name" + i)))
           {
                tx1[i].text= tx[i].text= PlayerPrefs.GetString("col_name" + i);

            }
        }
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("col_ext")))
        {
            extra.text = PlayerPrefs.GetString("col_ext");

        }


    }
    public void defalter()
    {
        for (int i = 0; i < inp.Length; i++)
        {
            
                PlayerPrefs.SetString("col_name" + i, defolt[i]);

            

        }
        PlayerPrefs.SetString("col_ext", extra_data);

    }
    private void Start()
    {
        InvokeRepeating("color_up", 0, 2f);
    }
    void color_up()
    {
        image_back.color = Random.ColorHSV();
    }
}
