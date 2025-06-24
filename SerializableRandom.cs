using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using Utilities.GSerialize;

// https://stackoverflow.com/a/46241672/914
namespace Utilities.GRandom {
	public sealed class SerializableRandom {
		int   _seed;
		int   _inext;
		int   _inextp;
		int[] _seedArray = new int[56];

		/// <summary>
		/// The current seed of this instance.
		/// </summary>
		public int Seed {
			get => _seed;
			set {
				_seed = value;
				var subtraction = (_seed == int.MinValue) ? int.MaxValue : Math.Abs(_seed);
				var mj          = 0x9a4ec86 - subtraction;
				_seedArray[0x37] = mj;
				var mk = 1;
				for (var i = 1; i < 0x37; i++) {
					var ii = (0x15 * i) % 0x37;
					_seedArray[ii] = mk;
					mk             = mj - mk;
					if (mk < 0x0) {
						mk += int.MaxValue;
					}

					mj = _seedArray[ii];
				}

				for (var k = 1; k < 0x5; k++) {
					for (var i = 1; i < 0x38; i++) {
						_seedArray[i] -= _seedArray[1 + (i + 0x1e) % 0x37];
						if (_seedArray[i] < 0) {
							_seedArray[i] += int.MaxValue;
						}
					}
				}

				_inext  = 0;
				_inextp = 21;
			}
		}

		public SerializableRandom() {
			// used for deserializing
		}
		
		public SerializableRandom(int seed) {
			Seed = seed;
		}

		public int Next() {
			return NextSample();
		}

		public int Next(int minValue, int maxValue) {
			return (int) (NextSample() * (1.0D / int.MaxValue) * (maxValue - minValue)) + minValue;
		}

		public int Next(int maxValue) {
			return Next(0, maxValue);
		}

		public double NextDouble() {
			return NextSample() * (1.0D / int.MaxValue);
		}
		
		public float NextFloat() {
			return (float) NextDouble();
		}

		public double NextDouble(double minValue, double maxValue) {
			return NextDouble() * (maxValue - minValue) + minValue;
		}

		public byte NextByte() {
			return (byte) Next(0, 256);
		}

		public void NextBytes(byte[] buffer) {
			for (var i = 0; i < buffer.Length; i++) {
				buffer[i] = NextByte();
			}
		}
		
		public float NextRange(float minValue, float maxValue) {
			return NextFloat() * (maxValue - minValue) + minValue;
		}
		
		///   Return a random int within [minInclusive..maxExclusive) 
		public int NextRange(int minInclusive, int maxExclusive) {
			return (int)(NextSample() * (1.0D / int.MaxValue) * (maxExclusive - minInclusive)) + minInclusive;
		}

		int NextSample() {
			var locINext  = _inext;
			var locINextp = _inextp;
			if (++locINext >= 56) {
				locINext = 1;
			}

			if (++locINextp >= 56) {
				locINextp = 1;
			}

			var retVal = _seedArray[locINext] - _seedArray[locINextp];
			if (retVal == int.MaxValue) {
				retVal--;
			}

			if (retVal < 0) {
				retVal += int.MaxValue;
			}

			_seedArray[locINext] = retVal;
			_inext               = locINext;
			_inextp              = locINextp;
			return retVal;
		}

		string GetState() {
			var s = new int[59];
			s[0] = _seed;
			s[1] = _inext;
			s[2] = _inextp;
			for (var i = 3; i < _seedArray.Length; i++) {
				s[i] = _seedArray[i - 3];
			}

			var bytes = new byte[s.Length * sizeof(int)];
			Buffer.BlockCopy(s, 0, bytes, 0, bytes.Length);
			return Convert.ToBase64String(bytes);
		}

		void LoadState(string serialized) {
			var s = new int[59];
			var base64EncodedBytes = Convert.FromBase64String(serialized);
			Buffer.BlockCopy(base64EncodedBytes, 0, s, 0, base64EncodedBytes.Length);
			
			_seed      = s[0];
			_inext     = s[1];
			_inextp    = s[2];
			_seedArray = new int[59];
			for (var i = 3; i < _seedArray.Length; i++) {
				_seedArray[i - 3] = s[i];
			}
		}
		
		[GSerializeInclude] [Preserve] string state {
			get => GetState();
			set => LoadState(value);
		}
		
		public static void Shuffle<T>(IList<T> array, SerializableRandom random) {
			var n = array.Count;
			for (var i = 0; i < n; i++) {
				// Use Next on random instance with an argument.
				// ... The argument is an exclusive bound.
				//     So we will not go past the end of the array.
				var r = i + random.Next(n - i);
				(array[r], array[i]) = (array[i], array[r]);
			}
		}
	}
}