using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour
{
    public GameObject Bullet, StartBullet; // шаблон пули
    public Rigidbody MyBody;  // Ссылка на свой Rigidboody
    public float Speed; // Скорость игрока
    private Vector3 Movement; 

    public bool CanAttack = true, Reload = false;

    public const int Magazine = 30;
    public int Ammo = 90, CurMagazine = 30;

    public int Health;
    public float JumpForce;

    public Text DisHealth, DisMagazine, DisAmmo;

    float GroundDis;
    Collider Col;

    public GameObject Cam;
    public GameObject Weapon;
    public float RotSpeedWeapon;

    Ray ray;
    RaycastHit hit;

    // Use this for initialization
    void Start()
    {
        DisHealth.text = "Health: " + Health;
        DisAmmo.text = "Ammo: " + Ammo;
        DisMagazine.text = "Magazine: " + CurMagazine;
        Col = GetComponent<Collider>();

        Cursor.lockState = CursorLockMode.Locked;
        MyBody = GetComponent<Rigidbody>();

        GroundDis = Col.bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            StartCoroutine(Fire());

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(StartReload());

        ray = new Ray(Cam.transform.position, Cam.transform.forward);

        Physics.Raycast(ray, out hit);

        Vector3 rot;

        if (hit.collider == null)
            rot = Weapon.transform.forward;

        else
            rot = hit.point - Weapon.transform.position;

      //  Debug.DrawLine(Cam.transform.position, hit.point, Color.red);

        Weapon.transform.rotation = Quaternion.Slerp(Weapon.transform.rotation, Quaternion.LookRotation(rot), RotSpeedWeapon * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            MyBody.AddForce(Vector3.up * JumpForce * 20, ForceMode.Impulse);

        float Right = Input.GetAxisRaw("Horizontal");
        float Forward = Input.GetAxisRaw("Vertical");

        Movement.Set(Forward, 0f, Right);

        MyBody.AddForce(transform.forward * Forward * Speed, ForceMode.Impulse);
        MyBody.AddForce(transform.right * Right * Speed, ForceMode.Impulse);
    }

    IEnumerator Fire()
    {
        if (CanAttack && CurMagazine > 0 && !Reload)
        {
            CanAttack = false;

            CurMagazine--;

            DisMagazine.text = "Magazine: " + CurMagazine;

            GameObject MyBullet = Instantiate(Bullet) as GameObject;

            MyBullet.transform.position = StartBullet.transform.position;
            MyBullet.transform.rotation = StartBullet.transform.rotation;

            MyBullet.GetComponent<Bullet2>().MyShooter = gameObject;



            //Instantiate(Bullet, StartBullet.transform.position, StartBullet.transform.rotation);



            if (CurMagazine <= 0)
            {
                StartCoroutine(StartReload());
                Reload = true;
            }

            yield return new WaitForSeconds(0.05f);

            CanAttack = true;
        }
    }

    IEnumerator StartReload()
    {        
        yield return new WaitForSeconds(1f);

        if(Ammo > Magazine)
        {
            int Num = Magazine;
            Num = Num - CurMagazine;
            Ammo -= Num;
            CurMagazine = Magazine;
        }

        else
        {
            CurMagazine = Ammo;
            Ammo = 0;
        }

        DisAmmo.text = "Ammo: " + Ammo;

        Reload = false;
    }

    public void TakeDamage(int Damage)
    {
        Health -= Damage;

        DisHealth.text = "Health: " + Health;

        if (Health <= 0)
        {
            //Условно смерть
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, GroundDis + 0.1f);
    }
}
