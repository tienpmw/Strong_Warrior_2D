﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character2 : MonoBehaviour
{

	[SerializeField] GameObject player;
	[SerializeField] Transform bulletPos;
	[SerializeField] GameObject bullet;
	[SerializeField] float xSpeed;
	[SerializeField] float xSpeedBullet;
	[SerializeField] float xStartPoint;
	[SerializeField] float xEndPoint;
	[SerializeField] float timeFire;
	[SerializeField] Slider sliderBlood;
	[SerializeField] GameObject borderBlood;
	[SerializeField] GameObject fillBlood;
	[SerializeField] List<GameObject> listBoxSecretBullet;

	bool isDie;
	bool isShooting;
	bool isDirectLeft = true;
	int healthPoint = 100;
	Animator anim;
	void Start()
	{
		anim = GetComponent<Animator>();
	}


	void Update()
	{
		if (isDie) return;
		//saw player
		if (Vector3.Distance(player.transform.position, transform.position) <= 11f)
		{
			anim.SetInteger("botAnim", 0);
			if (player.transform.position.x < transform.position.x)
			{
				FlipLeft();
			}
			else
			{
				FlipRight();
			}
			if (!isShooting)
			{
				// direct left
				if (isDirectLeft)
				{
					GameObject bulletObject = Instantiate(bullet, bulletPos.transform.position, Quaternion.Euler(0, 0, 0));
					bulletObject.GetComponent<Rigidbody2D>().velocity = -1 * bulletObject.transform.right * xSpeedBullet;
					Destroy(bulletObject, 2f);
				}
				// direct right
				else
				{
					GameObject bulletObject = Instantiate(bullet, bulletPos.transform.position, Quaternion.Euler(0, 0, 180));
					bulletObject.GetComponent<Rigidbody2D>().velocity = -1 * bulletObject.transform.right * xSpeedBullet;
					Destroy(bulletObject, 2f);
				}

				isShooting = true;
				StartCoroutine(TimeToNextFire(timeFire));
			}
		}
		// auto run when not see player
		else
		{
			if (xStartPoint >= transform.position.x)
			{
				FlipRight();
			}
			else if (xEndPoint <= transform.position.x)
			{
				FlipLeft();
			}
			if (!isDirectLeft)
			{
				transform.position += Vector3.right * Time.deltaTime * xSpeed;
			}
			else
			{
				transform.position += Vector3.left * Time.deltaTime * xSpeed;
			}
			anim.SetInteger("botAnim", 1);
		}
	}
	IEnumerator TimeToNextFire(float timeFire)
	{
		yield return new WaitForSeconds(timeFire);
		isShooting = false;
	}

	public void FlipLeft()
	{
		isDirectLeft = true;
		Vector3 Scaler = transform.localScale;
		Scaler.x *= Scaler.x > 0 ? 1 : -1;
		transform.localScale = Scaler;
	}
	public void FlipRight()
	{
		isDirectLeft = false;
		Vector3 Scaler = transform.localScale;
		Scaler.x *= Scaler.x > 0 ? -1 : 1;
		transform.localScale = Scaler;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("BulletPlayer") && !isDie)
		{
			TakeDamage();
			// destroy bullet player
			Destroy(collision.gameObject);
			
			// destroy enemy
			if (healthPoint <= 0)
			{
				isDie = true;
				StartCoroutine(StartCountDie());
			}
		}
		else if (collision.gameObject.CompareTag("DeathZone"))
		{
			Destroy(gameObject);
		}
	}

	private IEnumerator StartCountDie()
	{
		anim.SetInteger("botAnim", 2);
		yield return new WaitForSeconds(1f);
		Destroy(gameObject);
		// random box
		bool isDropBox = Random.Range(0, 2) == 0 ? false : true;
		if (isDropBox)
		{
			int rand = Random.Range(0, 3);
			GameObject box = Instantiate(listBoxSecretBullet[rand], new Vector3(transform.position.x + 2, transform.position.y + 2, transform.position.z), Quaternion.identity);
			Destroy(box, 60f);
		}
	}

	public void TakeDamage()
	{
		borderBlood.SetActive(true);
		fillBlood.SetActive(true);
		Invoke("HideHealthPoint", 1.5f);
		healthPoint -= 50;
		sliderBlood.value = healthPoint / (float)100;
	}

	public void HideHealthPoint()
	{
		borderBlood.SetActive(false);
		fillBlood.SetActive(false);
	}
}
