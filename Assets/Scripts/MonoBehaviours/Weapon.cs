using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Weapon : MonoBehaviour
{

    public GameObject ammoPrefab;
    [HideInInspector]
    public Animator animator;
    public float weaponVelocity;
    //пул для хранения нескольких экземпляров ammo
    public int poolSize;
    //размер дуги
    public float curveSize = 1.0f;

    static List<GameObject> ammoPool;
    Camera localCamera;
    bool isFiring;
    float positiveSlope;
    float negativeSlope;

    //перечисление с направлениями стрельбы
    enum Quadrant
    {
        East,South,West,North
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        isFiring = false;
        localCamera = Camera.main;

        //определяем гранные точки
        Vector2 lowerLeft = localCamera.ScreenToWorldPoint(new Vector2(0, 0));
        Vector2 upperRight = localCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Vector2 upperLeft = localCamera.ScreenToWorldPoint(new Vector2(0, Screen.height)); 
        Vector2 lowerRight = localCamera.ScreenToWorldPoint(new Vector2(Screen.width, 0));
       
        //срез каждой из линий, делящих мир на 4 части
        positiveSlope = GetSlope(lowerLeft, upperRight);
        negativeSlope = GetSlope(upperLeft, lowerRight);
    }

    private void Awake()
    {
        GameObject ammoContainer = new GameObject("AmmoContainer");
        if (ammoPool == null)
        {
            //инициализация
            ammoPool = new List<GameObject>();
        }
        //заполнение
        for(int i=0;i<poolSize;i++)
        {
            GameObject ammoObject = Instantiate(ammoPrefab,ammoContainer.transform);
            ammoObject.SetActive(false);
            ammoPool.Add(ammoObject);
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isFiring = true;
            FireAmmo();
        }
        UpdateState();
    }
    //(y2 – y1) / (x2 – x1) = m, где m = уклон.
    private float GetSlope(Vector2 pointOne,Vector2 pointTwo)
    {
        return (pointTwo.y - pointOne.y) / (pointTwo.x - pointOne.x);
    }
    //y = mx + b, (x,y) - координаты точки, m - уклон, b - пересечение с y
    private void UpdateState()
    {
        if (isFiring)
        {
            Vector2 quadrantVector;
            Quadrant quadEnum = GetQuadrant();
            switch (quadEnum)
            {
                case Quadrant.East:
                    quadrantVector = new Vector2(1.0f, 0.0f);
                    break;
                case Quadrant.South:
                    quadrantVector = new Vector2(0.0f, -1.0f);
                    break;
                case Quadrant.West: 
                    quadrantVector = new Vector2(-1.0f, 0.0f); 
                    break;
                case Quadrant.North: 
                    quadrantVector = new Vector2(0.0f, 1.0f);                    
                    break;
                default: 
                    quadrantVector = new Vector2(0.0f, 0.0f); 
                    break;
            }
            animator.SetBool("isFiring", true);
            animator.SetFloat("fireXDir", quadrantVector.x);
            animator.SetFloat("fireYDir", quadrantVector.y);
            isFiring = false;
        }
        else
        {
            animator.SetBool("isFiring", false);
        }
    }
    //метод для определения, выше ли нажатие линии позитивного/негативного наклона
    private bool HigherThanSlopeLine(Vector2 inputPosition, bool isPositiveSlope)
    {
        Vector2 playerPosition = gameObject.transform.position;
        Vector2 mousePosition = localCamera.ScreenToWorldPoint(inputPosition);
        float yIntercept, inputIntercept;
        if (isPositiveSlope)
        {
            //b = y – mx
            yIntercept = playerPosition.y - (positiveSlope * playerPosition.x);
            inputIntercept = mousePosition.y - (positiveSlope * mousePosition.x);
        }
        else
        {
            yIntercept = playerPosition.y - (negativeSlope * playerPosition.x);
            inputIntercept = mousePosition.y - (negativeSlope * mousePosition.x);
        }
        return inputIntercept > yIntercept;
    }
    ////метод для определения, выше ли нажатие линии отрицательного наклона
    //private bool HigherThanNegativeSlopeLine(Vector2 inputPosition)
    //{
    //    Vector2 playerPosition = gameObject.transform.position;
    //    Vector2 mousePosition = localCamera.ScreenToWorldPoint(inputPosition);
    //    //b = y – mx
    //    float yIntercept = playerPosition.y - (negativeSlope * playerPosition.x);
    //    float inputIntercept = mousePosition.y - (negativeSlope * mousePosition.x);
    //    return inputIntercept > yIntercept;
    //}
    //метод для определения, на какую из 4 зон нажал игрок
    private Quadrant GetQuadrant()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 playerPosition = transform.position;
        bool higherThanPositiveSlopeLine = HigherThanSlopeLine(Input.mousePosition,true);
        bool higherThanNegativeSlopeLine = HigherThanSlopeLine(Input.mousePosition,false);
        if (!higherThanPositiveSlopeLine && higherThanNegativeSlopeLine)
        {
            return Quadrant.East;
        }
        else if (!higherThanPositiveSlopeLine && !higherThanNegativeSlopeLine)
        {
            return Quadrant.South;
        }
        else if (higherThanPositiveSlopeLine && !higherThanNegativeSlopeLine)
        {
            return Quadrant.West;
        }
        else
        {
            return Quadrant.North;
        }
    }
    public GameObject SpawnAmmo(Vector3 location)
    {
        foreach (GameObject ammo in ammoPool)
        {
            if (ammo.activeSelf == false)
            {
                ammo.SetActive(true);
                ammo.transform.position = location;
                return ammo;
            }
        }
        return null;
    }
    private void FireAmmo()
    {
        //получаем позицию курсора, трансформируем положение на экране в положение в мире
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //активируем объект из пула в указанной позиции и получем на него ссылку
        GameObject ammo = SpawnAmmo(transform.position);
        if (ammo != null)
        {
            //получем дугу, привязанную к ammo
            Arc arcScript = ammo.GetComponent<Arc>();
            arcScript.curveSize = curveSize;
            //инициализация поля продолжительности перемещения ammo при помощи параметра скорости
            float travelDuration = 1.0f / weaponVelocity;
            
            StartCoroutine(arcScript.TravelArc(mousePosition, travelDuration));
        }
    }
    private void OnDestroy()
    {
        ammoPool = null;
    }
}
