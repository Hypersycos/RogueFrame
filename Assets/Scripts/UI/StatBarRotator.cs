using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class StatBarRotator : MonoBehaviour
    {
        [SerializeField] Camera camera;
        [SerializeField] Transform target;
        void Start()
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(target.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
        }
    }
}
