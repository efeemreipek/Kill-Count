using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponManagerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    private WeaponManager weaponManager;

    private void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();

        weaponManager.OnAmmoChanged += WeaponManager_OnAmmoChanged;
    }
    private void OnDisable()
    {
        weaponManager.OnAmmoChanged -= WeaponManager_OnAmmoChanged;
    }

    private void WeaponManager_OnAmmoChanged(int obj)
    {
        ammoText.text = $"{obj.ToString("D2")}/{weaponManager.GetMaxAmmo()}";
    }
}
