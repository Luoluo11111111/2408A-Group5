using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node<T>
{
    public Node<T> next;
    public Node<T> pre;

    public T data;

    public Node(T data)
    {
        this.data = data;
    }
}