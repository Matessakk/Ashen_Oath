using UnityEngine;

public class WeaponChange : MonoBehaviour
{

    public GameObject sword;
    public GameObject bow;

    private enum WeaponType { sword, bow };
    private WeaponType currentWeapon = WeaponType.sword;
    
    void Start()
    {
        EquipWeapon(WeaponType.sword);
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            EquipWeapon(WeaponType.sword);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(WeaponType.bow);
        }
    }

    void EquipWeapon(WeaponType weaponType)
    {
        currentWeapon = weaponType;

        sword.SetActive(weaponType == WeaponType.sword);
        bow.SetActive(weaponType == WeaponType.bow);
    }
}
