using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SliderScripts : MonoBehaviour
{
    
    private Slider slider; //slider(组件)
    private float startnum = 0; //初始值(正常情况为0)
    
    // Start is called before the first frame update
    void Start()
    {
        slider = transform.GetComponent<Slider>();//获取slider(我是直接把脚本付给自己所以直接获取)
    }

    // Update is called once per frame
    void Update()
    {
        startnum += Time.deltaTime; //每秒加一
        slider.value = startnum; //更新slider的值
        //设置条件
        if (startnum > 10)
        {
            SceneManager.LoadSceneAsync(1); //异步跳转战斗场景(场景的索引也可以写场景名称)
        }
    }
}
