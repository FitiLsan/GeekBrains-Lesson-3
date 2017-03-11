using UnityEngine;
using System.Collections;

public class Enemy2 : MonoBehaviour
{
    public Transform Target;

    public float Speed, TimeCast;
    public float MaxDistance, SpeedRotation, MinDistance, AttackDistance;

    public int Health;
    public int Damage;
    Rigidbody MyBody;
    Transform MyTransform;

    public bool Agressive = false;
    public bool Couldown = false;
    public bool IsAlive = true;

    public NavMeshAgent MyAgent;

    // Use this for initialization
    void Start ()
    {
        MyBody = GetComponent<Rigidbody>();
        MyTransform = transform;

        //Target = GameObject.FindGameObjectWithTag("Player").transform;
        if (IsAlive && MyAgent.enabled == true)
            MyAgent.SetDestination(Target.transform.position);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
       

        //Debug.Log("MaxDistance " + MaxDistance);
        

        if (Vector3.Distance(MyTransform.position, Target.position) < MaxDistance)
        {
           // Debug.Log("Дистанция меньше MaxDistance " + MaxDistance);
            Vector3 rot = Target.position - MyTransform.position;
            
            MyTransform.rotation = Quaternion.Slerp(MyTransform.rotation, Quaternion.LookRotation(rot), SpeedRotation * Time.deltaTime);

            if (Vector3.Distance(MyTransform.position, Target.position) > MinDistance)
                MyTransform.position += MyTransform.forward * Speed * Time.deltaTime;

            if (Vector3.Distance(MyTransform.position, Target.position) < AttackDistance)
                if (!Couldown)
                {
                    Couldown = true;
                    StartCoroutine(Attack());
                }
        }
        
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(TimeCast);

        if (Vector3.Distance(MyTransform.position, Target.position) < AttackDistance)
            Target.GetComponent<Player>().TakeDamage(Damage);

        Couldown = false;
    }

    public void TakeDamage(int Damage, GameObject Agressor)
    {
        if (IsAlive)
        {
            Health -= Damage;

            // Debug.Log(Agressor);

            if (Health <= 0)
            {
                Die();
            }

            StartCoroutine(BeAgressive(Agressor));
        }
    }

    public void Die()
    {
        IsAlive = false;
        Agressive = false;
        Target = null;
        MyAgent.SetDestination(transform.position);
        MyAgent.enabled = false;
        MyBody.constraints = RigidbodyConstraints.None;
        GetComponent<MeshRenderer>().material.color = Color.red;
    }


    IEnumerator BeAgressive(GameObject Agressor)
    {
        Agressive = true;
        Target = Agressor.transform;

        yield return new WaitForSeconds(20);

        Agressive = false;
    }
}
