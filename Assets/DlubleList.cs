using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DlubleList<T>
{
    public Node<T> head;
    public Node<T> tail;

    public int count;

    public int Count
    {
        get { return count; }
    }


    public void AddFirst(T data)
    {
        Node<T> node = new Node<T>(data);
        if (head == null)
        {
            head = tail = node;
        }
        else
        {
            node.next = head;
            head.pre = node;
            head = node;
        }
    }

    public void AddLast(T data)
    {
        Node<T> node = new Node<T>(data);
        if (tail == null)
        {
            tail = head = node;
        }
        else
        {
            node.pre = tail;
            tail.next = node;
            tail = node;
        }
    }

    public T RemoveFirst()
    {
        if (head == null)
        {
            Exception();
        }

        T data = head.data;
        head = head.next;
        if (head == null)
        {
            tail = null;
        }
        else
        {
            head.pre = null;
        }

        count--;
        return data;
    }
    
    
    public T RemoveLast()
    {
        if (tail == null)
        {
            Exception();
        }
        T data = tail.data;
        tail = tail.pre;
        if (tail == null)
        {
            head = null;
        }
        else
        {
            tail.next = null;
        }

        count--;
        return data;
    }

    private void Exception()
    {
        Debug.Log("链表为空");
    }
}