using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
public class Wander : MonoBehaviour
{
    public float pursuitSpeed;
    public float wanderSpeed;
    float currentSpeed;

    public float directionChangeInterval;
    public bool followPlayer;
    //ссылка на текущий корутин движения
    Coroutine moveCoroutine;

    Rigidbody2D rb2d;
    Animator animator;

    Transform parentTransform = null;
    //трансформ для определения позиции преследуемого объекта
    Transform targetTransform = null;
    Vector3 endPosition;
    //угол для создания нового вектора, указывающего конечную точку
    public float currentAngle;
    private Vector3 oldPosition;
    //ссылка на коллайдер для гизмы
    CircleCollider2D circleCollider;
    // Start is called before the first frame update
    void Start()
    {
        parentTransform = GetComponentInParent<Transform>();
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        currentSpeed = wanderSpeed;
        rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine(WanderRoutine());

    }
    
    public IEnumerator WanderRoutine()
    {
        while (true)
        {
            ChooseNewEndpoint();
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine); 
            }
            moveCoroutine = StartCoroutine(Move(rb2d, currentSpeed));
            yield return new WaitForSeconds(directionChangeInterval);

        }
    }

    private void ChooseNewEndpoint()
    {
        currentAngle = Random.Range(0, 360);
        currentAngle = Mathf.Repeat(currentAngle, 360);
        endPosition = Vector3FromAngle(currentAngle);
        
    }    

    Vector3 Vector3FromAngle(float inputAngleDegrees)
    {
        float inputAngleRadians = inputAngleDegrees * Mathf.Deg2Rad;
        if (parentTransform != null)
        {
            return new Vector3(Mathf.Cos(inputAngleRadians) + parentTransform.position.x, Mathf.Sin(inputAngleRadians) + parentTransform.position.y, 0);
        }
        else
        {
            return new Vector3(Mathf.Cos(inputAngleRadians), Mathf.Sin(inputAngleRadians), 0);
          //  return new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
        }

    }

    public IEnumerator Move(Rigidbody2D rigidBodyToMove,float speed)
    {
        //получаем грубое расстояние между объектами 
        float remainingDistance = (transform.position - endPosition).sqrMagnitude;

        
        while (remainingDistance > float.Epsilon)
        {
            if (targetTransform != null)
            {
                //переназначаем конечную позицию на позицию игрока
                endPosition = targetTransform.position;
            }
            if (rigidBodyToMove != null)
            {
                //вычисляем позицию, в которую требуется двигать объект, не превышая при этом максимальную дистанцию (зависящую от скорости)
                Vector3 newPosition = Vector3.MoveTowards(rigidBodyToMove.position, endPosition, speed * Time.deltaTime);
                //непосредственно перемещение RB
                rb2d.MovePosition(newPosition);
                //обновляем значение оставшейся дистанции до цели
                remainingDistance = (transform.position - endPosition).sqrMagnitude;

            }
            //проверка на то, изменилась ли позиция объекта
            if (transform.position != oldPosition)
            {
                animator.SetBool("isWalking", true);
                oldPosition = transform.position;
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
            //приостановка выполнения до следующего вызова FixedFrame
            yield return new WaitForFixedUpdate();
            //yield return null;

        }

        animator.SetBool("isWalking", false);
        //print($"{this.gameObject.name} has stopped");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && followPlayer)
        {
            currentSpeed = pursuitSpeed;
            targetTransform = collision.gameObject.transform;
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(Move(rb2d, currentSpeed));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("isWalking",false);
            currentSpeed = wanderSpeed;
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            targetTransform = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (circleCollider != null)
        {
            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
        }
    }
    private void Update()
    {
        Debug.DrawLine(rb2d.position, endPosition, Color.red);
    }
}
