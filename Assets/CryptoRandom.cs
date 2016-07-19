using System;
using System.Security.Cryptography;

class CryptoRandom : RandomNumberGenerator {
	private static RandomNumberGenerator r;

	public CryptoRandom(){ 
  		r = RandomNumberGenerator.Create();
 	}

 	public override void GetBytes(byte[] buffer){
   		r.GetBytes(buffer);
 	}

 	public override void GetNonZeroBytes(byte[] data){
		r.GetNonZeroBytes(data);
	}

 	public double NextDouble(){
  		byte[] b = new byte[4];
  		r.GetBytes(b);
  		return (double)BitConverter.ToUInt32(b, 0) / UInt32.MaxValue;
 	}
 
 	public int Next(int minValue, int maxValue){
 		//Debug.Log("next int");
 		double a = NextDouble();
 		int d = maxValue - minValue - 1;
 		//int d = maxValue – minValue – 1;
 		double c = a * d;
 		int b = (int) Math.Floor(c);
  		return b + minValue;
 	}

 	public int Next(){
  		return Next(0, Int32.MaxValue);
 	}
 
 	public int Next(int maxValue){
 		return Next(0, maxValue);
 	}
}