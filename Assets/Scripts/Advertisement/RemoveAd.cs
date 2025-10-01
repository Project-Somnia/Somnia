using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAd : MonoBehaviour
{   
    public static bool isAdRemoved = false;

    public GameObject Button;

    [Header("Advertisement")]

        public Topbanner Topbanner;
        public Bottombanner Bottombanner;

    // Start is called before the first frame update
    void Start()
    {
        if(isAdRemoved)
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {    
        if(isAdRemoved)
        {   
            Destroy(Button);
            Topbanner.DestroyAd();
            Bottombanner.DestroyAd();
            Destroy(this.gameObject);
        }
    }

    public void Removead()
    {
        isAdRemoved = true;
    }
}
