using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LRUList<T>
{
    public DlubleList<T> list = new DlubleList<T>();

    private int MaxCount = 10;

    StringBuilder sb = new StringBuilder();

    public void AddData(T data)
    {
        if (list.Count >= MaxCount)
        {
            for (int i = 0; i < MaxCount * 1 / 5; i++)
            {
                Debug.Log($"移除了{list.RemoveFirst()}");
            }

            list.AddLast(data);
            Debug.Log($"添加了{data},当前长度为{list.Count}");
        }
    }

    public StringBuilder GetData()
    {
        sb.Clear();
        var node = list.head;
        while (node != null)
        {
            sb.Append(node.data + ",");
            node = node.next;
        }
        return sb;
    }
}