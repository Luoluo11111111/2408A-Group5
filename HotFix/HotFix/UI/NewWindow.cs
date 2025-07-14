using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HotFix
{
    public class NewWindow : Window
    {
        private Button allButton;
        private Button fen1Button;
        private Button fen2Button;
        private Button closebtn;
        private Transform contentParent;

        private GameObject contentFen1;
        private GameObject contentFen2;

        private enum TabType { All, Fen1, Fen2 }
        private TabType currentTab = (TabType)(-1); // 初始化为非法值，确保第一次点击有效

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            base.Awake(param1, param2, param3);
            ComponentInit();
            SwitchTab(TabType.All); // 默认打开 ALL
        }

        private void ComponentInit()
        {
            allButton = m_Transform.Find("ALL")?.GetComponent<Button>();
            fen1Button = m_Transform.Find("Fen1")?.GetComponent<Button>();
            fen2Button = m_Transform.Find("Fen2")?.GetComponent<Button>();
            closebtn = m_Transform.Find("CLOSE")?.GetComponent<Button>();

            contentParent = m_Transform.Find("Scroll View/Viewport/Content");

            contentFen1 = contentParent.Find("Content_Fen1").gameObject;
            contentFen2 = contentParent.Find("Content_Fen2").gameObject;
            
            contentFen1.GetComponent<Button>().onClick.RemoveAllListeners();
            contentFen1.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("fen1");
                //UIManager.Instance.PopUpWnd(FilesName.ITEMPANEL, true, false);
            });
            contentFen2.GetComponent<Button>().onClick.RemoveAllListeners();
            contentFen2.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("fen2");
                //UIManager.Instance.PopUpWnd(FilesName.ITEMPANEL, true, false);
            });
            
            closebtn.onClick.RemoveAllListeners();
            closebtn.onClick.AddListener(() =>
            {
                Debug.LogError("退");
                UIManager.Instance.CloseWnd(FilesName.NEWPANEL);
            });
            
            if (allButton != null) allButton.onClick.AddListener(() =>
            {
                SwitchTab(TabType.All);
                
            });
            if (fen1Button != null) fen1Button.onClick.AddListener(() =>
            {
                SwitchTab(TabType.Fen1);
                
            });
            if (fen2Button != null) fen2Button.onClick.AddListener(() =>
            {
                SwitchTab(TabType.Fen2);

            });
        }

        private void SwitchTab(TabType tab)
        {
            if (tab == currentTab)
                return;

            switch (tab)
            {
                case TabType.All:
                    contentFen1?.SetActive(true);
                    contentFen2?.SetActive(true);
                    break;
                case TabType.Fen1:
                    contentFen1?.SetActive(true);
                    contentFen2?.SetActive(false);
                    break;
                case TabType.Fen2:
                    contentFen1?.SetActive(false);
                    contentFen2?.SetActive(true);
                    break;
            }

            currentTab = tab;
            
            
        }
    }
}
