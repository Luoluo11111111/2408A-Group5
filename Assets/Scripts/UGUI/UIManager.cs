//using Cinemachine;
//using ILRuntime.CLR.Method;
//using ILRuntime.CLR.TypeSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI 消息 ID 枚举
/// </summary>
public enum UIMsgID
{
    None = 0,
    StopTipsForBossBattle = 1,
}

/// <summary>
/// UI 管理器类，负责管理所有 UI 窗口的生命周期、显示、隐藏、关闭等操作
/// </summary>
public class UIManager : Singleton<UIManager>
{
    // 公共确认窗口引用
    public GameObject CommonConfirm;
    // UI 节点引用
    public RectTransform m_UiRoot;
    // 窗口节点引用
    public RectTransform m_WndRoot;
    // UI 摄像机引用
    private Camera m_UICamera;
    // EventSystem 节点引用
    private EventSystem m_EventSystem;
    // 屏幕宽高比
    private float m_CanvasRate = 0;

    // UI 预制体路径
    private string m_UIPrefabPath = "Assets/GameData/Prefabs/UI/Panel/";
    // 窗口注册字典，用于存储窗口名称与类型映射关系
    private Dictionary<string, System.Type> m_RegisterDic = new Dictionary<string, System.Type>();
    // 打开的窗口字典，用于存储打开窗口的引用
    private Dictionary<string, Window> m_WindowDic = new Dictionary<string, Window>();
    // 打开的窗口列表，用于窗口更新顺序管理
    private List<Window> m_WindowList = new List<Window>();
    // 存储所有打开过的窗口，用于外部查找窗口管理类
    private Dictionary<string, Window> m_GetWindowDic = new Dictionary<string, Window>();

    /// <summary>
    /// 初始化 UI 管理器
    /// </summary>
    /// <param name="uiRoot">UI 父节点</param>
    /// <param name="wndRoot">窗口父节点</param>
    /// <param name="uiCamera">UI 摄像机</param>
    /// <param name="eventSystem">EventSystem 节点</param>
    public void Init(RectTransform uiRoot, RectTransform wndRoot, Camera uiCamera, EventSystem eventSystem)
    {
        m_UiRoot = uiRoot;
        m_WndRoot = wndRoot;
        m_UICamera = uiCamera;
        m_EventSystem = eventSystem;
        m_CanvasRate = Screen.height / (m_UICamera.orthographicSize * 2);
    }

    /// <summary>
    /// 设置 UI 预制体路径
    /// </summary>
    /// <param name="path">新的 UI 预制体路径</param>
    public void SetUIPrefabPath(string path)
    {
        m_UIPrefabPath = path;
    }

    /// <summary>
    /// 显示或隐藏所有 UI
    /// </summary>
    /// <param name="show">是否显示</param>
    public void ShowOrHideUI(bool show)
    {
        if (m_UiRoot != null)
        {
            m_UiRoot.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// 设置默认选择对象
    /// </summary>
    /// <param name="obj">要设置为默认选择的对象</param>
    public void SetNormalSelectObj(GameObject obj)
    {
        if (m_EventSystem == null)
        {
            m_EventSystem = EventSystem.current;
        }
        m_EventSystem.firstSelectedGameObject = obj;
    }

    /// <summary>
    /// 窗口更新方法，调用所有打开窗口的更新逻辑
    /// </summary>
    public void OnUpdate()
    {
        for (int i = 0; i < m_WindowList.Count; i++)
        {
            Window window = m_WindowList[i];
            if (window != null)
            {
                if (window.IsHotFix)
                {
                    // 如果是热更新窗口，通过 ILRuntime 调用更新方法
                    ILRuntimeManager.Instance.ILRunAppDomain.Invoke(window.HotFixClassName, "OnUpdate", window);
                }
                else
                {
                    // 如果是原生窗口，直接调用更新方法
                    window.OnUpdate();
                }
            }
        }
    }

    /// <summary>
    /// 窗口注册方法，将窗口名称与类型进行关联注册
    /// </summary>
    /// <typeparam name="T">窗口泛型类</typeparam>
    /// <param name="name">窗口名称</param>
    public void Register<T>(string name) where T : Window
    {
        m_RegisterDic[name] = typeof(T);
    }

    /// <summary>
    /// 发送消息给指定窗口
    /// </summary>
    /// <param name="name">窗口名称</param>
    /// <param name="msgID">消息 ID</param>
    /// <param name="param">消息参数数组</param>
    /// <returns>消息发送是否成功</returns>
    public bool SendMessageToWnd(string name, UIMsgID msgID = 0, params object[] param)
    {
        Window wnd = FindWndByName<Window>(name);
        if (wnd != null)
        {
            return wnd.OnMessage(msgID);
        }
        return false;
    }

    /// <summary>
    /// 根据窗口名称查找窗口
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    /// <param name="name">窗口名称</param>
    /// <returns>找到的窗口对象，未找到返回 null</returns>
    private T FindWndByName<T>(string name) where T : Window
    {
        if (m_WindowDic.TryGetValue(name, out Window wnd))
        {
            return (T)wnd;
        }

        return null;
    }

    /// <summary>
    /// 根据名称查找窗口管理类，供外部调用
    /// </summary>
    /// <param name="name">窗口名称</param>
    /// <returns>找到的窗口对象，未找到返回 null</returns>
    public Window GetWndByName(string name)
    {
        return m_GetWindowDic.ContainsKey(name) ? m_GetWindowDic[name] : null;
    }

    /// <summary>
    /// 打开窗口方法
    /// </summary>
    /// <param name="wndName">窗口名称</param>
    /// <param name="bTop">是否置于最上层</param>
    /// <param name="resource">是否从资源加载</param>
    /// <param name="param1">参数 1</param>
    /// <param name="param2">参数 2</param>
    /// <param name="param3">参数 3</param>
    /// <returns>打开的窗口对象</returns>
    public Window PopUpWnd(string wndName, bool bTop = true, bool resource = false, object param1 = null, object param2 = null, object param3 = null)
    {
        Window wnd = FindWndByName<Window>(wndName);
        if (wnd == null)
        {
            // 如果窗口未注册，尝试创建窗口实例
            if (m_RegisterDic.TryGetValue(wndName, out System.Type tp))
            {
                if (resource)
                {
                    wnd = System.Activator.CreateInstance(tp) as Window;
                }
                else
                {
                    string hotName = "HotFix." + wndName.Replace("Panel.prefab", "Window");
                    wnd = ILRuntimeManager.Instance.ILRunAppDomain.Instantiate<Window>(hotName);
                    wnd.IsHotFix = true;
                    wnd.HotFixClassName = hotName;
                }
            }

            string path = m_UIPrefabPath + wndName;

            GameObject wndObj;
            if (resource)
            {
                // 从资源加载预制体
                wndObj = Object.Instantiate(Resources.Load<GameObject>(wndName.Replace(".prefab", "")));
            }
            else
            {
                // 从对象管理器加载预制体
                wndObj = ObjectManager.Instance.InstantiateObject(path, false, false);
            }

            if (wndObj == null)
            {
                Debug.Log("创建窗口Prefab失败：" + wndName);
            }

            if (wnd == null)
            {
                string hotName = "HotFix." + wndName.Replace("Panel.prefab", "Window");
                Debug.LogError(hotName + "   ::::::::::::::::::::");
                wnd = ILRuntimeManager.Instance.ILRunAppDomain.Instantiate<Window>(hotName);
                wnd.IsHotFix = true;
                wnd.HotFixClassName = hotName;
            }

            // 初始化窗口属性
            if (!m_WindowDic.ContainsKey(wndName))
            {
                m_WindowList.Add(wnd);
                m_WindowDic.Add(wndName, wnd);
            }
            if (!m_GetWindowDic.ContainsKey(wndName))
                m_GetWindowDic.Add(wndName, wnd);

            wnd.m_GameObject = wndObj;
            wnd.m_Transform = wndObj.transform;
            if (wnd.m_Transform.GetComponent<Button>())
            {
                wnd.m_Transform.GetComponent<Button>().onClick.RemoveAllListeners();
                wnd.m_Transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    CloseWnd(wndName);
                });
            }
            wnd.m_RectTransform = wndObj.GetComponent<RectTransform>();
            wnd.Name = wndName;

            wnd.Resource = resource;
            wndObj.transform.SetParent(m_WndRoot, false);

            if (!wnd.m_GameObject.activeInHierarchy)
                wnd.m_GameObject.SetActive(true);

            // 设置窗口锚点和比例
            wnd.m_RectTransform.anchorMin = new Vector2(0, 0);
            wnd.m_RectTransform.anchorMax = new Vector2(1, 1);
            wnd.m_RectTransform.localScale = new Vector3(1, 1, 1);
            wnd.m_RectTransform.pivot = new Vector2(0.5f, 0.5f);
            wnd.m_RectTransform.offsetMin = new Vector2(0.0f, 0.0f);
            wnd.m_RectTransform.offsetMax = new Vector2(0.0f, 0.0f);

            if (wnd.IsHotFix)
            {
                // 如果是热更新窗口，调用热更新的初始化方法
                ILRuntimeManager.Instance.ILRunAppDomain.Invoke(wnd.HotFixClassName, "Awake", wnd, param1, param2, param3);
            }
            else
            {
                // 如果是原生窗口，调用原生的初始化方法
                wnd.Awake(param1, param2, param3);
            }

            if (bTop)
            {
                // 置于最上层
                wndObj.transform.SetAsLastSibling();
            }
            else
            {
                // 置于最下层
                wndObj.transform.SetAsFirstSibling();
            }

            if (wnd.IsHotFix)
            {
                // 如果是热更新窗口，调用热更新的显示方法
                ILRuntimeManager.Instance.ILRunAppDomain.Invoke(wnd.HotFixClassName, "OnShow", wnd, param1, param2, param3);
            }
            else
            {
                // 如果是原生窗口，调用原生的显示方法
                wnd.OnShow(param1, param2, param3);
            }
        }
        else
        {
            // 如果窗口已打开，调用显示方法
            ShowWnd(wndName, bTop, param1, param2, param3);
        }

        return wnd;
    }

    /// <summary>
    /// 关闭窗口方法（通过名称）
    /// </summary>
    /// <param name="name">窗口名称</param>
    /// <param name="destory">是否销毁</param>
    public void CloseWnd(string name, bool destory = false)
    {
        Window wnd = FindWndByName<Window>(name);
        CloseWnd(wnd, destory);
    }

    /// <summary>
    /// 关闭窗口方法（通过窗口对象）
    /// </summary>
    /// <param name="window">窗口对象</param>
    /// <param name="destory">是否销毁</param>
    public void CloseWnd(Window window, bool destory = false)
    {
        if (window != null)
        {
            if (window.IsHotFix)
            {
                // 如果是热更新窗口，调用热更新的禁用方法
                ILRuntimeManager.Instance.ILRunAppDomain.Invoke(window.HotFixClassName, "OnDisable", window);
            }
            else
            {
                // 如果是原生窗口，调用原生的禁用方法
                window.OnDisable();
            }

            if (window.IsHotFix)
            {
                // 如果是热更新窗口，调用热更新的关闭方法
                ILRuntimeManager.Instance.ILRunAppDomain.Invoke(window.HotFixClassName, "OnClose", window);
            }
            else
            {
                // 如果是原生窗口，调用原生的关闭方法
                window.OnClose();
            }

            if (m_WindowDic.ContainsKey(window.Name))
            {
                m_WindowDic.Remove(window.Name);
                m_WindowList.Remove(window);
            }

            if (!window.Resource)
            {
                if (destory)
                {
                    // 如果需要销毁，释放对象并立即销毁
                    ObjectManager.Instance.RealaseObject(window.m_GameObject, 0, true);
                }
                else
                {
                    // 如果不需要销毁，释放对象但不立即销毁
                    ObjectManager.Instance.RealaseObject(window.m_GameObject, recycleParent: false);
                }
            }
            else
            {
                // 如果是资源加载的窗口，直接销毁
                GameObject.Destroy(window.m_GameObject);
            }
            window.m_GameObject = null;
            window = null;
        }
    }

    /// <summary>
    /// 关闭所有窗口
    /// </summary>
    public void CloseAllWnd()
    {
        for (int i = m_WindowList.Count - 1; i >= 0; i--)
        {
            CloseWnd(m_WindowList[i]);
        }
    }

    /// <summary>
    /// 切换到唯一窗口（关闭所有其他窗口并打开指定窗口）
    /// </summary>
    /// <param name="name">窗口名称</param>
    /// <param name="bTop">是否置于最上层</param>
    /// <param name="resource">是否从资源加载</param>
    /// <param name="param1">参数 1</param>
    /// <param name="param2">参数 2</param>
    /// <param name="param3">参数 3</param>
    public void SwitchStateByName(string name, bool bTop = true, bool resource = false, object param1 = null, object param2 = null, object param3 = null)
    {
        CloseAllWnd();
        PopUpWnd(name, bTop, resource, param1, param2, param3);
    }

    /// <summary>
    /// 隐藏窗口（通过名称）
    /// </summary>
    /// <param name="name">窗口名称</param>
    public void HideWnd(string name)
    {
        Window wnd = FindWndByName<Window>(name);
        HideWnd(wnd);
    }

    /// <summary>
    /// 隐藏窗口（通过窗口对象）
    /// </summary>
    /// <param name="wnd">窗口对象</param>
    public void HideWnd(Window wnd)
    {
        if (wnd != null)
        {
            wnd.m_GameObject.SetActive(false);
            wnd.OnDisable();
        }
    }

    /// <summary>
    /// 显示窗口（通过名称）
    /// </summary>
    /// <param name="name">窗口名称</param>
    /// <param name="bTop">是否置于最上层</param>
    /// <param name="param1">参数 1</param>
    /// <param name="param2">参数 2</param>
    /// <param name="param3">参数 3</param>
    public void ShowWnd(string name, bool bTop = true, object param1 = null, object param2 = null, object param3 = null)
    {
        Window wnd = FindWndByName<Window>(name);
        ShowWnd(wnd, bTop, param1, param2, param3);
    }

    /// <summary>
    /// 显示窗口（通过窗口对象）
    /// </summary>
    /// <param name="wnd">窗口对象</param>
    /// <param name="bTop">是否置于最上层</param>
    /// <param name="param1">参数 1</param>
    /// <param name="param2">参数 2</param>
    /// <param name="param3">参数 3</param>
    public void ShowWnd(Window wnd, bool bTop = true, object param1 = null, object param2 = null, object param3 = null)
    {
        if (wnd != null)
        {
            if (wnd.m_GameObject != null && !wnd.m_GameObject.activeSelf) wnd.m_GameObject.SetActive(true);
            if (bTop) wnd.m_Transform.SetAsLastSibling();
            if (wnd.IsHotFix)
            {
                // 如果是热更新窗口，调用热更新的显示方法
                //ILRuntimeManager.Instance.ILRunAppDomain.Invoke(wnd.HotFixClassName, "OnShow", wnd, param1, param2, param3);
            }
            else
            {
                // 如果是原生窗口，调用原生的显示方法
                wnd.OnShow(param1, param2, param3);
            }
        }
    }
}

