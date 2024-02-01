using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blockades : MonoBehaviour
{
    enum ColiderType
    {
        BOX,
        SPHERE,
        CAPSULE
    }
    [SerializeField] ColiderType coliderType;
    [SerializeField] float timerCount;
    float timer;
    private BoxCollider box;
    private SphereCollider sphere;
    private CapsuleCollider capsule;
    private void Init()
    {
        timer = timerCount;
        switch (coliderType)
        {
            case ColiderType.BOX:
            {
                box = gameObject.GetComponent<BoxCollider>();
                break;
            }
            case ColiderType.SPHERE:
            {
                sphere = gameObject.GetComponent<SphereCollider>();
                break;
            }
            case ColiderType.CAPSULE:
            {
                    capsule = gameObject.GetComponent<CapsuleCollider>();
                break;
            }
        }
    }
    private void Tick()
    {
        if(timer < timerCount)
        {
            timer += Time.deltaTime;
            if(timer >= timerCount) 
            {
                switch (coliderType)
                {
                    case ColiderType.BOX:
                    {
                        box.enabled = true;
                        break;
                    }
                    case ColiderType.SPHERE:
                    {
                        sphere.enabled = true;
                        break;
                    }
                    case ColiderType.CAPSULE:
                    {
                        capsule.enabled = true;
                        break;
                    }
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        //Player interaction here.

        timer = 0;
        switch (coliderType)
        {
            case ColiderType.BOX:
            {
                 box.enabled = false;
                 return;
            }
            case ColiderType.SPHERE:
            {
                 sphere.enabled = false;
                 return;
            }
            case ColiderType.CAPSULE:
            {
                 capsule.enabled = false;
                 return;
            }
        }
    }
}
