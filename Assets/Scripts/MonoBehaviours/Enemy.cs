using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public int damageStrength;
    //ссылка на корутин для возможности его остановки
    Coroutine damageCoroutine;
    float hitPoints;
    //метод-корутин
    public override IEnumerator DamageCharacter(int damage, float interval)
    {
        while(true)
        {
            StartCoroutine(FlickerCharacter());
            hitPoints -= damage;
            print($"{gameObject.name} was hit by: {damage}HP  ({hitPoints}/{maxHitPoints}HP left)");
            if (hitPoints<=float.Epsilon)
            {
                KillCharacter();                
                print($"{gameObject.name} died");
                break;
            }
            //проверяем интервал нанесения урона (на случай, если урон не единовременный)
            if (interval>float.Epsilon)
            {
                //задержка выполнения на указанный интервал
                yield return new WaitForSeconds(interval);

                //yield return null;
            }
            //в случае отсутствия/истечения интервала выходим из тела цикла
            else
            {
                break;
            }
        }
    }
    public override void ResetCharacter()
    {
        hitPoints = startingHitPoints;
    }

    private void OnEnable()
    {
        ResetCharacter();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if(damageCoroutine==null)
            {
                damageCoroutine = StartCoroutine(player.DamageCharacter(damageStrength, 1.0f));
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

}
