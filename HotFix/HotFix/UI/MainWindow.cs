using System;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    class MainWindow : Window
    {
        Text m_Money; //金币
        Text m_Coupon; //点券
        Button m_ExitBtn;
        Button m_BuyBtn;
        Button Open;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            var goodDatas = JsonConfigManager.GetGoodData();
            foreach (var item in goodDatas)
            {
                var sprite = ResourceManager.Instance.LoadSpriteBySpriteAtlas(item.ImgPath, item.ImgName);
                Debug.Log(sprite);
            }

            FindAllComponent();
            AddAllBtnsListener();
            InitData();
        }

        public void InitData()
        {
            m_Money.text = UserInfoManager.money.ToString();
            m_Coupon.text = UserInfoManager.coupon.ToString();
        }

        private void AddAllBtnsListener()
        {
            AddButtonClickListener(m_ExitBtn, ExitGame);
            AddButtonClickListener(m_BuyBtn, OpenBuyPanel);
            AddButtonClickListener(Open, OpenNewPanel);
        }

        private void OpenNewPanel()
        {
            //Debug.LogError("123");
            UIManager.Instance.PopUpWnd(FilesName.NEWPANEL, true, false);
            //UIManager.Instance.PopUpWnd(FilesName.ITEMPANEL, true, false);
        }

        private void OpenBuyPanel()
        {
            UIManager.Instance.PopUpWnd(FilesName.MONTHCARDPANEL, true, false);
        }

        

        private void ExitGame()
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        private void FindAllComponent()
        {
            m_Money = m_Transform.Find("Top/Money").GetComponent<Text>();
            m_Coupon = m_Transform.Find("Top/Gold").GetComponent<Text>();
            m_ExitBtn = m_Transform.Find("Top/Exit").GetComponent<Button>();
            m_BuyBtn = m_Transform.Find("Right/BuyBtn").GetComponent<Button>();
            Open = m_Transform.Find("Button (Legacy)").GetComponent<Button>();
            Debug.LogError("123");
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            //Debug.LogError("111111111111111111111111111111111111111111");
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
            }
        }
    }
}