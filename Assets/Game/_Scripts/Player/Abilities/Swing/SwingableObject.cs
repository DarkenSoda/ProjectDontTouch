using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingableObject : MonoBehaviour
{
    public Transform target;
    public Transform Visual;

    private void Start() {
        Visual.gameObject.SetActive(false);
    }
}
