using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public HitPoints hitPoints;
    //создание ссылок на префабы для инспектора
    public HealthBar healthBarPrefab;
    public Inventory inventoryPrefab;
    //создание непосредственно объектов
    HealthBar healthBar;
    Inventory inventory;

    //Метод срабатывания триггера при коллизии
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CanBePickedUp"))
        {
            Item hitObject = collision.gameObject.GetComponent<Consumable>().item;
            if (hitObject!=null)
            {
                print("Hit: " + hitObject.objectName);
                bool shouldDisapear = false;
                switch (hitObject.itemType)
                {
                    case Item.ItemType.COIN:
                        shouldDisapear = inventory.AddItem(hitObject);
                        //shouldDisapear = true;
                        break;
                    case Item.ItemType.HEALTH:
                        shouldDisapear = AdjustHitPoints(hitObject.quantity);
                        break;
                    default:
                        break;
                }
                if (shouldDisapear)
                {
                collision.gameObject.SetActive(false);
                }
            }
        }
    }
    //Метод изменения ХП
    public bool AdjustHitPoints(int amount)
    {
        if (hitPoints.value<maxHitPoints)
        {
            hitPoints.value += amount;
            print($"Adjusted HP by: {amount}. New value: {hitPoints.value}");
            return true;
        }
        return false;
    }

    public override IEnumerator DamageCharacter(int damage, float interval)
    {
        while (true) 
        {
            //метод изменения цвета
            StartCoroutine(FlickerCharacter());

            hitPoints.value -= damage;
            print($"{gameObject.name} was hit by: {damage}HP  ({hitPoints.value}/{maxHitPoints}HP left)");
            if (hitPoints.value <= float.Epsilon) 
            { 
                KillCharacter();
                print($"{gameObject.name} died");
                break; 
            } 
            if (interval > float.Epsilon) 
            { 
                yield return new WaitForSeconds(interval);
            } 
            else 
            { 
                break;
            } 
        }
    }
    public override void KillCharacter()
    {
        base.KillCharacter();
        Destroy(healthBar.gameObject);
        Destroy(inventory.gameObject);
    }
    public override void ResetCharacter()
    {
        //инициализация копией префаба
        inventory = Instantiate(inventoryPrefab);
        //инициализация ХБ присвоением копии префаба 
        healthBar = Instantiate(healthBarPrefab);
        //указание, что этот объект является персонажем для класса ХБ 
        healthBar.character = this;

        hitPoints.value = startingHitPoints;
    }
    private void OnEnable()
    {
        ResetCharacter();
    }


}
