using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.GSerialize;

namespace Utilities.GRandom {
	[Serializable]
	[GSerializeInline]
	public class PseudorandomBagRange {

		[GSerializeSkipIfDefault(2)] [Range(1, 5)]
		public int duplicates = 2;

		public int         min;
		public int         max;
		SerializableRandom random;

		List<int> bag;
		
		public void SetRandom(SerializableRandom newRandom) {
			random = newRandom;
		}

		public int Roll() {
			if (bag == null || bag.Count == 0) Reset();
			var value = bag![0];
			bag.RemoveAt(0);
			return value;
		}

		public void Reset() {
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			if (duplicates < 1) duplicates = 1;

			if (max < min) max = min;
			var range          = max - min;
			var numItems = range + 1;
			
			bag = new List<int>(numItems);
			for (var ii = 0; ii < duplicates; ii++) {
				for (var i = min; i <= max; i++) bag.Add(i);
			}
			
			SerializableRandom.Shuffle(bag, random);
		}
	}
	
	[Serializable]
	[GSerializeInline]
	public class PseudorandomBag<T> {

		[GSerializeSkipIfDefault(2)]
		public int duplicates = 2;
		SerializableRandom random;

		[GSerializeSkip] public T[] options;
		[GSerializeInclude]     List<T> bag;
		
		public PseudorandomBag() { }
		public PseudorandomBag(T[] options) => SetOptions(options);
		public PseudorandomBag(SerializableRandom random, T[] options) {
			SetRandom(random);
			SetOptions(options);
		}

		public void SetOptions(T[] newOptions) => options = newOptions;
		public void SetRandom(SerializableRandom newRandom) => random = newRandom;

		public T Roll() {
			if (bag == null || bag.Count == 0) Reset();
			var value = bag![0];
			bag.RemoveAt(0);
			return value;
		}

		public void Reset() {
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			duplicates = Mathf.Max(1, duplicates);
			var numItems = options.Length * duplicates;
			
			if (bag == null || bag.Count != numItems) 
				bag = new List<T>(numItems);
			
			for (var i = 0; i < numItems; i++) 
				bag.Add(options[i % options.Length]);
			
			SerializableRandom.Shuffle(bag, random);
		}
	}
}