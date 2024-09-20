using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private GameObject muzzleFlashGO;

    public void ActivateMuzzleFlash()
    {
        muzzleFlashGO.SetActive(true);
    }
    public void DeactivateMuzzleFlash()
    {
        muzzleFlashGO.SetActive(false);
    }
}
