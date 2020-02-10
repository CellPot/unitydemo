using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject slotPrefab;
    public const int numSlots = 5;
    Image[] itemImages = new Image[numSlots];
    Item[] items = new Item[numSlots];
    GameObject[] slots = new GameObject[numSlots];
    public void Start()
    {
        CreateSlots();
    }
    public void CreateSlots()
    {
        //проверяем, присвоен ли скрипту префаб
        if (slotPrefab!=null)
        {
            for (int i=0;i< numSlots;i++)
            {
                //инициализация нового слота при помощи получения КОПИИ префаба
                GameObject newSlot = Instantiate(slotPrefab);
                //именование слота
                newSlot.name = "ItemSlot_" + i;
                //установлка для слота родительским объекта Inventory (первый в объекте InventoryObject)
                newSlot.transform.SetParent(gameObject.transform.GetChild(0).transform);
                //присвоение полученного слота соответствующей ячейке массива слотов
                slots[i] = newSlot;
                //присвоение 2 child-объекта слота (изображение) массиву изображений предметов
                itemImages[i] = newSlot.transform.GetChild(1).GetComponent<Image>();
            }
        }
    }
    
    public bool AddItem(Item itemToAdd)
    {
        for (int i=0;i<items.Length;i++)
        {
            if (items[i] != null && items[i].itemType == itemToAdd.itemType 
                && itemToAdd.stackable == true)
            {
                items[i].quantity += 1;
                //получаем ссылку на объект Слот с прикрепленным к нему скриптом (содержит текстовое поле с кол-вом) 
                Slot slotScript = slots[i].gameObject.GetComponent<Slot>();
                //ссылка на поле с текстом кол-ва
                Text quantityText = slotScript.qtyText;
                //показываем текст со значением кол-ва
                quantityText.enabled = true;
                quantityText.text = items[i].quantity.ToString();
                return true;
            }
            //если дошли до свободного слота
            if (items[i]==null)
            {
                //присваиваем КОПИЮ предмета соответствующему индексу массива
                items[i] = Instantiate(itemToAdd);
                //изменяем кол-во на 1
                items[i].quantity = 1;
                //присваиваем спрайт массиву изображений и активируем (по умолчанию деакт)
                itemImages[i].sprite = itemToAdd.sprite;
                itemImages[i].enabled = true;
                return true;
            }
        }
        return false;
    }
}
