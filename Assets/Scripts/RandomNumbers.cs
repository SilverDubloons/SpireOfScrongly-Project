using UnityEngine;
using System;

public class RandomNumbers : MonoBehaviour
{
    private System.Random rng;
	
	public void ChangeSeed(int seed)
	{
		rng =  new System.Random(seed);
	}
	
	public int Range(int min, int max)
	{
		return rng.Next(min, max);
	}
	
	public float Range(float min, float max)
	{
		return Mathf.Lerp(min, max, (float)rng.NextDouble());
	}
}